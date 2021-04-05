using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication.Command
{
    public abstract class CommandCallbackBase
    {
        protected abstract void DeserializationBody(ref byte[] data, ref int offset);

        public virtual CommandCallbackBase Deserialization(ref byte[] data)
        {
            if (data[0] == 0x50
                && data[2] == 0x10
                && data[3] == 0x50)
            {
                int num = 6;
                DeserializationBody(ref data, ref num);
                return this;
            }
            return null;
        }
    }
}
