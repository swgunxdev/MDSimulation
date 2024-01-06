using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaDataModeling
{
    public interface IToFromByteArray
    {
        byte[] ToByteArray();

        int FromByteArray(byte[] data, int offset);
    }

    public interface IMDSerialize : IToFromByteArray
    {
        ushort Id { get; }
        ushort TypeId { get; }
        ushort MetaDataType { get; }

        ushort IntegralType { get; }

        ushort Version { get; }
    }
}
