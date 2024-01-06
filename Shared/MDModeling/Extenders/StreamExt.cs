using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MDModeling.Extenders
{
    /// <summary>
    /// A tiny extension class to make writing byte arrays to a 
    /// stream more readable.
    /// </summary>
    public static class StreamExt
    {
        public static void WriteByteArray(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}
