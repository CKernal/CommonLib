using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication.Command
{
    public abstract class CommandBase
    {
        protected abstract int GetBodyLength();

        protected abstract void SerializationBody(ref byte[] data, ref int offset);

        public virtual byte[] Serialization()
        {
            int bodyLength = this.GetBodyLength();
            byte[] array = new byte[6 + bodyLength + 1];

            int num = 0;
            array[num++] = 0x50;
            array[num++] = 0x00;
            array[num++] = 0x50;
            array[num++] = 0x10;
            array[num++] = 0x11;
            array[num++] = 0x01;

            this.SerializationBody(ref array, ref num);

            array[num] = 0x53;
            return array;
        }


    }
}
