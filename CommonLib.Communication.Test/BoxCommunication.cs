using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Communication.Test
{
    public class BoxCommunication
    {
        private static BoxCommunication m_instance;
        private static readonly object m_locker = new object();

        private TcpServer8888 m_server;
        private Thread m_worker;
        private bool m_isRunning;

        private SetLight m_setLightCommand = new SetLight();

        public event Action<string> OnLogMsg;

        private void LogMsg(string log)
        {
            if (OnLogMsg != null)
            {
                OnLogMsg(log);
            }
        }

        public static BoxCommunication Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BoxCommunication();
                }
                return m_instance;
            }
        }

        public List<TcpClient> ExistedClientList { get { return m_server.ClientList; } }

        private BoxCommunication()
        {
        }

        public void Start()
        {
            m_server = new TcpServer8888();
            m_server.Started += m_server_Started;
            m_server.Accept += m_server_Accept;
            m_server.Disconnected += m_server_Disconnected;
            m_server.DataReceive += m_server_DataReceive;
            //m_server.LogMsg += m_server_LogMsg;

            m_server.Start();

            //m_isRunning = true;
            //m_worker = new Thread(WorkerThread);
            //m_worker.IsBackground = true;
            //m_worker.Start();
        }

        public void Stop()
        {
            if (this.m_server != null)
            {
                this.m_server.Stop();
            }

            this.m_isRunning = false;
            if (this.m_worker != null)
            {
                this.m_worker.Abort();
            }
        }

        public bool ClientExists(string ip)
        {
            return m_server.ClientList.Exists(d => d.RemoteIpAddress == ip);
        }


        public bool SetLight(byte lightType, byte lightNumber, string ip = null)
        {
            var list = m_server.ClientList.FindAll(d =>
            {
                if (string.IsNullOrEmpty(ip))
                {
                    return d.Socket.Connected;
                }
                else
                {
                    return d.RemoteIpAddress == ip && d.Socket.Connected;
                }
            });

            if (list == null
                || list.Count == 0)
            {
                return false;
            }

            m_setLightCommand.LightType = lightType;
            m_setLightCommand.LightNumber = lightNumber;
            for (int i = 0; i < list.Count; i++)
            {
                var tcpClient = list[i];
                tcpClient.Send(m_setLightCommand);

                LogMsg(string.Format("客户端{0}: Server发送数据: {1}", tcpClient, Tools.Byte2HexString(m_setLightCommand.Serialization())));
            }

            return true;
        }

        private void WorkerThread()
        {
            while (m_isRunning)
            {

                Thread.Sleep(1000);
            }
        }

        private void m_server_Started(System.Net.Sockets.Socket obj)
        {
            LogMsg(string.Format("服务端{0}已启动", obj.LocalEndPoint));
        }

        private void m_server_Accept(ITcpClient client)
        {
            LogMsg(string.Format("客户端{0}已连接，总连接数:{1}", client, this.m_server.ClientList.Count));
        }

        private void m_server_Disconnected(ITcpClient client)
        {
            LogMsg(string.Format("客户端{0}已断开，总连接数:{1}", client, this.m_server.ClientList.Count));
        }

        private void m_server_DataReceive(ITcpClient client, byte[] data)
        {
            LogMsg(string.Format("客户端{0}: Server接收数据: {1}", client, Tools.Byte2HexString(data)));
        }

        private void m_server_LogMsg(string arg1, string log)
        {
            LogMsg(log);
        }
    }
}
