using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{
    public interface ISerializeByteArray
    {
        byte[] ToByteArray();
        Int32 FromByteArray(byte[] data, Int32 offset);
    }
}
