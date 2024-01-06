using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MSGPACK
using MsgPack;
#endif
namespace MetaDataModeling.Utils
{
#if MSGPACK
    public static class MsgPackHelpers
    {
        public static byte[] ReadBuffer(this MsgPackReader reader)
        {
            uint length = reader.Length;
            byte[] b = new byte[length];
            uint count = 0;
            while (reader.Read() && count < length)
            {
                b[count++] = (byte)reader.ValueSigned;
            }
            return b;
        }
    }
#endif
}
