using CommonLib.Communication.Command;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Communication.Test
{
    public class SetLight : CommandBase
    {
        private byte[] light_buffer = new byte[16] { 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x10 };
        public byte LightType { get; set; }
        public byte LightNumber { get; set; }
        protected override int GetBodyLength()
        {
            return 0;
        }

        protected override void SerializationBody(ref byte[] data, ref int offset)
        {
        }

        public override byte[] Serialization()
        {
            light_buffer[4] = LightType;
            light_buffer[14] = LightNumber;
            light_buffer[15] = Tools.XOR_Check(light_buffer, 15);

            return light_buffer;
        }
    }
}
