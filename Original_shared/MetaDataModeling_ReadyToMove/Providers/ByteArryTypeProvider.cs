using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaDataModeling.Utils;

namespace MetaDataModeling.Providers
{
    public class ByteArryTypeProvider<TResult> : IByteArrayConverter<TResult>
    {
        public static ByteArryTypeProvider<TResult> Default
        {
            get { return DefaultTypeProviders.GetDefaultProvider<TResult>(); }
        }

        Func<byte[], int, TResult> _conversion;

        internal ByteArryTypeProvider(Func<byte[], int, TResult> conversion)
        {
            _conversion = conversion;
        }

        public TResult Convert(byte[] data, int startIndex = 0)
        {
            return _conversion(data, startIndex);
        }
    }

    static class DefaultTypeProviders
    {
        static Dictionary<Type, object> _providers;


        static DefaultTypeProviders()
        {
            Utils.BitConverter.IsLittleEndian = false;
            // Here are a couple for illustration. Yes, I am suggesting that
            // in reality you would add a BytesProvider<T> for each T
            // supported by the BitConverter class.
            _providers = new Dictionary<Type, object>
            {
                { typeof(byte), new ByteArryTypeProvider<byte>(PassthroughConverter.Passthrough) },
                { typeof(sbyte), new ByteArryTypeProvider<sbyte>(PassthroughConverter.SignedPassthrough) },
                { typeof(bool), new ByteArryTypeProvider<bool>(Utils.BitConverter.ToBoolean) },
                { typeof(short), new ByteArryTypeProvider<short>(Utils.BitConverter.ToInt16) },
                { typeof(ushort), new ByteArryTypeProvider<ushort>(Utils.BitConverter.ToUInt16) },
                { typeof(int), new ByteArryTypeProvider<int>(Utils.BitConverter.ToInt32) },
                { typeof(uint), new ByteArryTypeProvider<uint>(Utils.BitConverter.ToUInt32) },
                { typeof(long), new ByteArryTypeProvider<long>(Utils.BitConverter.ToInt64) },
                { typeof(ulong), new ByteArryTypeProvider<ulong>(Utils.BitConverter.ToUInt64) },
                { typeof(float), new ByteArryTypeProvider<float>(Utils.BitConverter.ToSingle) },
                { typeof(double), new ByteArryTypeProvider<double>(Utils.BitConverter.ToDouble) },
                { typeof(string), new ByteArryTypeProvider<string>(EncodeString.FromBytes) },
            };
        }

        public static ByteArryTypeProvider<T> GetDefaultProvider<T>()
        {
            return (ByteArryTypeProvider<T>)_providers[typeof(T)];
        }
    }

    public static class EncodeString
    {
        public static byte[] GetBytes(string s)
        {
            Encoding utf8 = Encoding.UTF8;

            return utf8.GetBytes(s);
        }

        public static string FromBytes(byte[] data, int startIndex = 0)
        {
            Encoding utf8 = Encoding.UTF8;
            // TODO:  HACK HACK HACK the math at the end assumes that the string is at the
            // end of the buffer.
            return utf8.GetString(data, startIndex, (data.Length - startIndex));
        }
    }

}
