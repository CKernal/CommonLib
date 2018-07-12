using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CKernal.CommonLib.Network
{
    public class IpHelper
    {
        public static string GetLocolIpAddress()
        {
            IPAddress localip = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .First();

            return localip.ToString() ;
        }
    }
}
