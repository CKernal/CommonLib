using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Communication
{
    public abstract class TcpClientBase<TCommandBase, TCommandCallbackBase> : ITcpClient
    {
        private Socket m_socket;

        private long m_active;

        private byte[] m_data = new byte[4096];

        private int m_receiveDataCount;

        private long m_lostCount;

        private int m_sendCount;

        private int m_receiveCount;

        private int m_sendByte;

        private int m_receiveByte;

        private long m_sendReceiveLocker;

        private long m_lastTime;

        private ConcurrentQueue<TCommandCallbackBase> m_commandCallbackList;

        private List<TCommandBase> m_commandList;

        protected object m_locker = new object();

        protected TCommandBase m_sendedCommand;

        public Guid Id { get; private set; }


        public Socket Socket
        {
            get { return m_socket; }
            set
            {
                m_socket = value;
                RemoteIpAddress = ((IPEndPoint)m_socket.RemoteEndPoint).Address.ToString();
                RemoteEndPoint = m_socket.RemoteEndPoint.ToString();
                m_socket.SendTimeout = -1;
                m_socket.ReceiveTimeout = -1;
            }
        }

        public string RemoteIpAddress { get; set; }

        public string RemoteEndPoint { get; set; }

        public byte[] ReceiveBuffer { get; set; }

        public byte[] SendBuffer { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime LastSendTime { get; private set; }

        public DateTime LastReceiveTime { get; private set; }

        public int SendCount { get { return m_sendCount; } }

        public int ReceiveCount { get { return m_receiveCount; } }

        public int ReceiveDataCount
        {
            get { return m_receiveDataCount; }
            set { m_receiveDataCount = value; }
        }

        public byte[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public int SendByte
        {
            get { return m_sendByte; }
        }

        public int ReceiveByte
        {
            get { return m_receiveByte; }
        }

        public int QueryCount { get; set; }

        public bool Active
        {
            get { return Interlocked.Read(ref m_active) != 0L; }
            set { Interlocked.Exchange(ref m_active, value ? 1L : 0L); }
        }

        public bool SendNeed
        {
            get
            {
                long ticks = DateTime.Now.Ticks;
                if (ticks - m_lastTime < 200000L)
                {
                    return false;
                }
                if (ticks - m_lastTime > 2000000L && m_sendReceiveLocker == 1L)
                {
                    Interlocked.Exchange(ref m_lastTime, ticks);
                    Interlocked.Exchange(ref m_sendReceiveLocker, 0L);
                    Interlocked.Increment(ref m_lostCount);
                    if (Interlocked.Read(ref m_lostCount) > 3L && Active)
                    {
                        Active = false;
                        TimeoutProcess();
                    }
                }
                return Interlocked.Read(ref m_sendReceiveLocker) == 0L;
            }
        }

        public TcpClientBase()
        {
            m_lostCount = 0L;
            Active = true;
            Id = Guid.NewGuid();
            ReceiveBuffer = new byte[4096];
            m_commandCallbackList = new ConcurrentQueue<TCommandCallbackBase>();
            m_commandList = new List<TCommandBase>();
        }

        protected void SendCompleted(int dataLength)
        {
            Interlocked.Exchange(ref m_lastTime, DateTime.Now.Ticks);
            Interlocked.Exchange(ref m_sendReceiveLocker, 1L);
            Interlocked.Add(ref m_sendCount, 1);
            Interlocked.Add(ref m_sendByte, dataLength);
            LastSendTime = DateTime.Now;
        }

        public abstract void SendCommand();

        protected abstract void TimeoutProcess();

        protected abstract void SendErrorProcess(Exception ex);

        protected void AsyncSendCallback(IAsyncResult ar)
        {
            try
            {
                ITcpClient tcpClient = (ITcpClient)ar.AsyncState;
                tcpClient.Socket.EndSend(ar);
                lock (m_locker)
                {
                    if (m_sendedCommand != null)
                    {
                        for (int i = m_commandList.Count - 1; i >= 0; i--)
                        {
                            if (m_sendedCommand.Equals(m_commandList[i]))
                            {
                                m_commandList.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorProcess(ex);
            }
        }

        public void ReceiveCompleted(int receiveCount)
        {
            Interlocked.Exchange(ref m_sendReceiveLocker, 0L);
            Interlocked.Exchange(ref m_lostCount, 0L);
            Interlocked.Add(ref m_receiveCount, 1);
            Interlocked.Add(ref m_receiveByte, receiveCount);
            LastReceiveTime = DateTime.Now;
            Active = true;
        }

        public void Push(TCommandCallbackBase commandCallback)
        {
            m_commandCallbackList.Enqueue(commandCallback);
        }

        public TCommandCallbackBase Pop()
        {
            TCommandCallbackBase result;
            if (m_commandCallbackList.TryDequeue(out result))
            {
                return result;
            }
            return default(TCommandCallbackBase);
        }

        public TCommandBase CommandPop()
        {
            TCommandBase tCommandBase = default(TCommandBase);
            if (m_commandList.Count > 0)
            {
                lock (m_locker)
                {
                    if (m_commandList.Count > 0)
                    {
                        tCommandBase = m_commandList[0];
                        m_commandList.Remove(tCommandBase);
                    }
                }
            }
            return tCommandBase;
        }

        public void Send(TCommandBase command)
        {
            lock (m_locker)
            {
                m_commandList.Add(command);
            }
        }

        public void Clear()
        {
            lock (m_locker)
            {
                m_commandList.Clear();
            }
        }

        public override string ToString()
        {
            return RemoteEndPoint;
        }
    }
}