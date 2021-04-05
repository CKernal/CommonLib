using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    public class Tools
    {
        public static string Byte2HexString(byte[] comByte)
        {
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            for (int i = 0; i < comByte.Length; i++)
            {
                builder.Append(Convert.ToString(comByte[i], 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return builder.ToString().ToUpper();
        }

        public static byte XOR_Check(byte[] buffer, int count, int index = 0)
        {
            byte b = 0;
            for (int i = 0; i < count; i++)
            {
                b ^= buffer[index + i];
            }
            return b;
        }
    }
}
