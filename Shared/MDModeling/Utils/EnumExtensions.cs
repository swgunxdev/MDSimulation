//
// File Name: EnumExtensions.cs
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling.Utils
{
    /// <summary>
    /// This static class will convert from an enumeration to an
    /// integer and back.
    /// </summary>
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        public static uint ToUInt(this Enum enumValue)
        {
            return Convert.ToUInt32(enumValue);
        }

        public static TEnum ToEnum<TInput, TEnum>(this TInput value)
        {
            Type type = typeof(TEnum);

            if (value == null)
            {
                throw new ArgumentException("Value is null or empty.", "value");
            }

            if (!type.IsEnum)
            {
                throw new ArgumentException("Enum expected.", "TEnum");
            }

            return (TEnum)Enum.Parse(type, value.ToString(), true);
        }
    }
}
