using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication
{
    public class TcpServer : TcpServerBase<TcpClient>
    {
        public event Action<ITcpClient> Accept;
        public event Action<ITcpClient, byte[]> DataReceive;
        public event Action<string, string> LogMsg;

        public TcpServer(int port = 8000) : base(port)
        {
        }

        protected override void AsyncAcceptCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                Socket socket2 = socket.EndAccept(ar);

                TcpClient tcpClient = new TcpClient();
                tcpClient.Socket = socket2;
                tcpClient.Socket.BeginReceive(tcpClient.ReceiveBuffer, 0, tcpClient.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), tcpClient);
                tcpClient.StartTime = DateTime.Now;

                lock (m_locker)
                {
                    m_clientSocketList.Add(tcpClient);
                }

                if (Accept != null)
                {
                    Accept(tcpClient);
                }

                Log(string.Format("{0} 客户端产生连接正常", tcpClient));
            }
            catch (Exception exception)
            {
                Log(string.Format("客户端连接处理时产生异常,{0}", exception.Message));
            }
            finally
            {
                try
                {
                    socket.BeginAccept(new AsyncCallback(AsyncAcceptCallback), socket);
                }
                catch
                {
                }
            }
        }

        protected override void AsyncReceiveCallback(IAsyncResult ar)
        {
            TcpClient tcpClient = (TcpClient)ar.AsyncState;
            try
            {
                int num = tcpClient.Socket.EndReceive(ar);
                if (num == 0)
                {
                    tcpClient.Socket.Close();
                    Log(string.Format("{0} 客户端接收到的字节数为0，连接断开", tcpClient));
                    return;
                }

                byte[] array = ReceiveDataProcess(tcpClient, num);
                if (array != null)
                {
                    ReceiveHandler(tcpClient, num, array);
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("{0} 客户端接收异常1，{1}", tcpClient, ex.Message));
                if (tcpClient.Socket.Connected)
                {
                    try
                    {
                        tcpClient.Socket.BeginReceive(tcpClient.ReceiveBuffer, 0, tcpClient.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), tcpClient);
                    }
                    catch (Exception ex2)
                    {
                        Log(string.Format("{0} 客户端接收异常2，{1}", tcpClient, ex2.Message));
                    }
                }
            }
        }

        protected virtual byte[] ReceiveDataProcess(TcpClient tcpClient, int num)
        {
            byte[] array = new byte[num];
            Array.Copy(tcpClient.ReceiveBuffer, 0, array, 0, array.Length);
            if (array[0] == 0x50
                && array[2] == 0x10
                && array[3] == 0x50)
            {
                int num2 = array[5];
                if (num2 + 7 > num)
                {
                    Array.Copy(array, 0, tcpClient.Data, 0, array.Length);
                    tcpClient.ReceiveDataCount = num;
                    tcpClient.Socket.BeginReceive(tcpClient.ReceiveBuffer, 0, tcpClient.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), tcpClient);
                }
                else
                {
                    return array;
                }
            }
            else if (tcpClient.ReceiveDataCount == 0)
            {
                tcpClient.Socket.BeginReceive(tcpClient.ReceiveBuffer, 0, tcpClient.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), tcpClient);
            }
            else
            {
                Array.Copy(array, 0, tcpClient.Data, tcpClient.ReceiveDataCount, array.Length);
                tcpClient.ReceiveDataCount += num;
                int num2 = tcpClient.Data[5];
                if (num2 + 7 > tcpClient.ReceiveDataCount)
                {
                    tcpClient.Socket.BeginReceive(tcpClient.ReceiveBuffer, 0, tcpClient.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), tcpClient);
                }
                else
                {
                    array = new byte[tcpClient.ReceiveDataCount];
                    Array.Copy(tcpClient.Data, 0, array, 0, tcpClient.ReceiveDataCount);
                    return array;
                }
            }

            return null;
        }

        protected void ReceiveHandler(TcpClient client, int receiveCount, byte[] data)
        {
            try
            {
                if (DataReceive != null)
                {
                    DataReceive(client, data);
                }
                client.ReceiveCompleted(receiveCount);
                client.ReceiveDataCount = 0;
                Array.Clear(client.Data, 0, client.Data.Length);
                client.Socket.BeginReceive(client.ReceiveBuffer, 0, client.ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), client);
            }
            catch (Exception exception)
            {
                Log(string.Format("{0} 网络接收产生异常,{1}", client, exception.Message));
            }
        }

        private void Log(string log)
        {
            if(LogMsg != null)
            {
                LogMsg("TcpServer", log);
            }
        }

    }
}
