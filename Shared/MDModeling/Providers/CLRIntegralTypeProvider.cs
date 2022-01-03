using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling.Providers
{
    public enum ClrIntegralType : ushort
    {
        None,
        Byte,
        Char,
        UsignedChar,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        String
    }

    public static class CLRIntegralTypeProvider
    {
        static Dictionary<Type, TypeCode> _typeToEnum;
        static Dictionary<TypeCode, Type> _enumToType;

        static CLRIntegralTypeProvider()
        {
            // Here are a couple for illustration. Yes, I am suggesting that
            // in reality you would add a BytesProvider<T> for each T
            // supported by the BitConverter class.
            _typeToEnum = new Dictionary<Type, TypeCode>
        {
            { typeof(Boolean), TypeCode.Boolean },
            { typeof(Char), TypeCode.Char },
            { typeof(byte), TypeCode.Byte },
            { typeof(sbyte), TypeCode.SByte },
            { typeof(short), TypeCode.Int16 },
            { typeof(ushort), TypeCode.UInt16 },
            { typeof(int), TypeCode.Int32 },
            { typeof(uint), TypeCode.UInt32 },
            { typeof(long), TypeCode.Int64 },
            { typeof(ulong), TypeCode.UInt64 },
            { typeof(float), TypeCode.Single },
            { typeof(double), TypeCode.Double },
            { typeof(string), TypeCode.String },
        };
            _enumToType = new Dictionary<TypeCode, Type>
        {
            { TypeCode.Boolean, typeof(Boolean) },
            { TypeCode.Char, typeof(char) },
            { TypeCode.Byte, typeof(byte) },
            { TypeCode.SByte, typeof(sbyte) },
            { TypeCode.Int16, typeof(short) },
            { TypeCode.UInt16, typeof(ushort) },
            { TypeCode.Int32, typeof(int)},
            { TypeCode.UInt32, typeof(uint) },
            { TypeCode.Int64, typeof(long)},
            { TypeCode.UInt64, typeof(ulong) },
            { TypeCode.Single, typeof(float) },
            { TypeCode.Double,typeof(double) },
            { TypeCode.String, typeof(string) },
        };

        }

        public static TypeCode TypeToTypeEnum<T>()
        {
            return _typeToEnum[typeof(T)];
        }

        public static Type TypeEnumToType(TypeCode type)
        {
            return _enumToType[type];
        }
    }
}
