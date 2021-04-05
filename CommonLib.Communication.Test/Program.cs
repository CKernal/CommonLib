using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            BoxCommunication communication = BoxCommunication.Instance;
            communication.OnLogMsg += communication_OnLoging;
            communication.Start();

            while (true)
            {
                var key = Console.ReadKey();
                if (key.KeyChar == 'E'
                    || key.KeyChar == 'e')
                {
                    break;
                }

                //Console.WriteLine("************* ExistedClientList **************");
                var clientList = communication.ExistedClientList;
                //foreach (var item in clientList)
                //{
                //    Console.WriteLine(item.RemoteEndPoint);
                //}

                if (clientList.Count > 0)
                {
                    communication.SetLight(1, 1);
                }
            }

            communication.Stop();
        }

        private static void communication_OnLoging(string obj)
        {
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fff} => {1}", DateTime.Now, obj);
        }
    }
}
