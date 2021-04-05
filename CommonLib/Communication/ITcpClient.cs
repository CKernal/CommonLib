using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication
{
    public interface ITcpClient
    {
        Socket Socket { get; set; }
        DateTime StartTime { get; set; }
        byte[] ReceiveBuffer { get; set; }
        string RemoteIpAddress { get; set; }
        string RemoteEndPoint { get; set; }
        int ReceiveDataCount { get; set; }
        byte[] Data { get; set; }
        bool SendNeed { get;}
        void SendCommand();
        void ReceiveCompleted(int receiveCount);
    }
}
