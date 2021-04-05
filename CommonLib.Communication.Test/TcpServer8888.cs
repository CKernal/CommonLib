using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication.Test
{
    public class TcpServer8888 : TcpServer
    {
        public TcpServer8888(int port = 8888) : base(port)
        {
        }

        protected override byte[] ReceiveDataProcess(TcpClient tcpClient, int num)
        {
            return tcpClient.ReceiveBuffer.Take(num).ToArray();
        }
    }
}
