using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaDataModeling.Utils
{
    public static class PassthroughConverter
    {
        public static Byte[] Passthrough(byte b)
        {
            return new byte[] { b };
        }

        public static byte Passthrough(Byte[] d, int startIndex = 0)
        {
            return d[startIndex];
        }

        public static byte[] Passthrough(sbyte sb)
        {
            return new byte[] { (byte)sb };
        }

        public static sbyte SignedPassthrough(byte [] d, int startIndex = 0)
        {
            return (sbyte)d[startIndex];
        }
    }
}
