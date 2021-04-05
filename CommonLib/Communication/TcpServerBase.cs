using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Communication
{
    public abstract class TcpServerBase<TTcpClient> where TTcpClient : ITcpClient, new()
    {
        private Socket m_serverSocket;
        private Thread m_sendThread;

        private int m_port;
        private bool m_isSendWorking;

        protected List<TTcpClient> m_clientSocketList;
        protected object m_locker;

        public event Action<Socket> Started;
        public event Action<ITcpClient> Disconnected;

        public List<TTcpClient> ClientList
        {
            get { return m_clientSocketList; }
        }

        public TcpServerBase(int port = 6000)
        {
            m_port = port;
            m_clientSocketList = new List<TTcpClient>();
            m_locker = new object();
        }

        public void Start()
        {
            uint num = 0;
            byte[] array = new byte[Marshal.SizeOf(num) * 3];
            BitConverter.GetBytes(1u).CopyTo(array, 0);//启用Keep-Alive
            BitConverter.GetBytes(10000u).CopyTo(array, Marshal.SizeOf(num));//在这个时间间隔内没有数据交互，则发探测包 毫秒
            BitConverter.GetBytes(1000u).CopyTo(array, Marshal.SizeOf(num) * 2);//发探测包时间间隔 毫秒

            EndPoint localEP = new IPEndPoint(IPAddress.Any, m_port);
            m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //m_serverSocket.IOControl(IOControlCode.KeepAliveValues, array, null);
            m_serverSocket.Bind(localEP);
            m_serverSocket.Listen(500);
            m_serverSocket.BeginAccept(new AsyncCallback(AsyncAcceptCallback), m_serverSocket);

            if (Started != null)
            { 
                Started(m_serverSocket); 
            }

            m_isSendWorking = true;
            m_sendThread = new Thread(delegate (object state)
            {
                while (m_isSendWorking)
                {
                    lock (m_locker)
                    {
                        Parallel.ForEach(m_clientSocketList, delegate (TTcpClient client, ParallelLoopState loopState)
                        {
                            try
                            {
                                if (client.SendNeed
                                && client.Socket.Connected
                                && client.Socket.Poll(100, SelectMode.SelectWrite))
                                {
                                    client.SendCommand();
                                }
                            }
                            catch
                            {
                            }
                        });

                        for (int i = m_clientSocketList.Count - 1; i > -1; i--)
                        {
                            TTcpClient tTcpClient = m_clientSocketList[i];
                            if (!tTcpClient.Socket.Connected)
                            {
                                tTcpClient.Socket.Close();
                                m_clientSocketList.RemoveAt(i);

                                if (Disconnected != null)
                                {
                                    Disconnected(tTcpClient);
                                }
                            }
                        }

                        if (!m_isSendWorking)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
            });

            m_sendThread.IsBackground = true;
            m_sendThread.Start(null);
        }

        public void Stop()
        {
            m_isSendWorking = false;
            m_sendThread.Abort();
            m_serverSocket.Close(500);
        }

        protected abstract void AsyncAcceptCallback(IAsyncResult ar);
        protected abstract void AsyncReceiveCallback(IAsyncResult ar);
    }
}