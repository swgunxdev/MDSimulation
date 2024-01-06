using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDModeling.Utils;

namespace MDModeling.Providers
{


    static class DefaultBytesProviders
    {
        static Dictionary<Type, object> _providers;

        static DefaultBytesProviders()
        {
            Utils.BitConverter.IsLittleEndian = false;
            // Here are a couple for illustration. Yes, I am suggesting that
            // in reality you would add a BytesProvider<T> for each T
            // supported by the BitConverter class.
            _providers = new Dictionary<Type, object>
            {
                { typeof(byte), new BytesProvider<byte>(PassthroughConverter.Passthrough) },
                { typeof(sbyte), new BytesProvider<sbyte>(PassthroughConverter.Passthrough) },
                { typeof(bool), new BytesProvider<bool>(Utils.BitConverter.GetBytes)},
                { typeof(short), new BytesProvider<short>(Utils.BitConverter.GetBytes) },
                { typeof(ushort), new BytesProvider<ushort>(Utils.BitConverter.GetBytes) },
                { typeof(int), new BytesProvider<int>(Utils.BitConverter.GetBytes) },
                { typeof(uint), new BytesProvider<uint>(Utils.BitConverter.GetBytes) },
                { typeof(long), new BytesProvider<long>(Utils.BitConverter.GetBytes) },
                { typeof(ulong), new BytesProvider<ulong>(Utils.BitConverter.GetBytes) },
                { typeof(float), new BytesProvider<float>(Utils.BitConverter.GetBytes) },
                { typeof(double), new BytesProvider<double>(Utils.BitConverter.GetBytes) },
                { typeof(string), new BytesProvider<string>(EncodeString.GetBytes) },
            };
        }

        public static BytesProvider<T> GetDefaultProvider<T>()
        {
            return (BytesProvider<T>)_providers[typeof(T)];
        }
    }



    public class BytesProvider<T> : IBytesProvider<T>
    {
        public static BytesProvider<T> Default
        {
            get { return DefaultBytesProviders.GetDefaultProvider<T>(); }
        }

        Func<T, byte[]> _conversion;

        internal BytesProvider(Func<T, byte[]> conversion)
        {
            _conversion = conversion;
        }

        public byte[] GetBytes(T value)
        {
            return _conversion(value);
        }
    }
}
