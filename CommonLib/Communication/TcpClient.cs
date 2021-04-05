
using CommonLib.Communication.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication
{
    public class TcpClient : TcpClientBase<CommandBase, CommandCallbackBase>
    {
        public event Action<ITcpClient> ErrorProcess;

        public event Action<ITcpClient> ExpTimeOutHandler;

        protected override void SendErrorProcess(Exception ex)
        {
            if (ErrorProcess != null)
            {
                ErrorProcess(this);
            }
        }

        protected override void TimeoutProcess()
        {
            if (ExpTimeOutHandler != null)
            {
                ExpTimeOutHandler(this);
            }
        }

        public override void SendCommand()
        {
            try
            {
                CommandBase commandBase = CommandPop();
                if (commandBase != null)
                {
                    m_sendedCommand = commandBase;
                    byte[] array = commandBase.Serialization();
                    Socket.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(AsyncSendCallback), this);
                    SendCompleted(array.Length);
                }
            }
            catch (Exception ex)
            {
                SendErrorProcess(ex);
            }
        }
    }
}
