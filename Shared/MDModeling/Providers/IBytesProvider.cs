using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling.Providers
{
    /// <summary>
    /// This interface is declared to specify serialization of
    /// objects to a byte array.
    /// </summary>
    /// <typeparam name="T">The generic type of the object to be serialized</typeparam>
    public interface IBytesProvider<T>
    {
        byte[] GetBytes(T value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IByteArrayConverter<T>
    {
        T Convert(byte[] data, int startIndex);
    }
}
