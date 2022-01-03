using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling.Utils
{
    public static class GenRadom
    {
        public static byte GenRandomByte(int minNumber, int maxNumber)
        {
            if (maxNumber < minNumber)
                throw new System.Exception("The maxNumber value should be greater than minNumber");
            System.Random r = CreateRandom();
            return (byte) r.Next(minNumber, maxNumber);
        }

        public static int GenRandomInt(int maxNumber)
        {
            return GenRandomInt(1, maxNumber);
        }

        public static int GenRandomInt(int minNumber, int maxNumber)
        {
            if (maxNumber < minNumber)
                throw new System.Exception("The maxNumber value should be greater than minNumber");
            System.Random r = CreateRandom();
            return r.Next(minNumber, maxNumber);
        }

        public static ushort GenRandomUshort(ushort maxNumber)
        {
            return GenRandomUshort(1, maxNumber);
        }

        public static ushort GenRandomUshort(ushort minNumber, ushort maxNumber)
        {
            if (maxNumber < minNumber)
                throw new System.Exception("The maxNumber value should be greater than minNumber");
            System.Random r = CreateRandom();
            return Convert.ToUInt16(r.Next(minNumber, maxNumber));
        }

        private static System.Random CreateRandom()
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            int seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            System.Random r = new System.Random(seed);
            return r;
        }

        public static float NextFloat()
        {
            return NextFloat(CreateRandom());
        }

        private static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public static string GenRandomString(int length)
        {
            string[] array = new string[54]
			{
				"0","2","3","4","5","6","8","9",
				"a","b","c","d","e","f","g","h","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
				"A","B","C","D","E","F","G","H","J","K","L","M","N","P","R","S","T","U","V","W","X","Y","Z"
			};
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++) sb.Append(array[GenRandomInt(53)]);
            return sb.ToString();
        }

        public static IdType GenIdTypeNameObj<T>()
            where T : IdType, new()
        {
            T t = new T();
            t.Id = (ushort)GenRadom.GenRandomInt(65343);
            t.TypeId = (ushort)GenRadom.GenRandomInt(254);
            t.Name = GenRadom.GenRandomString(128);
            return t;
        }

        public static ObjVersion GenObjVersion()
        {
            return new ObjVersion(GenRadom.GenRandomByte(0,255),
                                    GenRadom.GenRandomByte(0,255),
                                    GenRadom.GenRandomByte(0,255),
                                    GenRadom.GenRandomByte(0,255));
        }
    }
}