using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Earlz.BareMetal;
using System.IO;
using MetaDataModeling.Utils;
using MetaDataModeling.Providers;

namespace MetaDataModeling
{
    public class RangedProperty<T> : GenericProperty<T>
    {
        T _min;
        T _max;
        T _frequency;

        public RangedProperty()
            : base()
        {
        }

        public RangedProperty(ushort id, ushort typeId, string name, T defaultValue)
            : base(id, typeId, name, defaultValue)
        {
        }

        public RangedProperty(ushort id, ushort typeId, string name, T defaultValue, T min, T max, T frequency)
            : base(id, typeId, name, defaultValue)
        {
            _min = min;
            _max = max;
            _frequency = frequency;
        }

        public T Minimum
        {
            get { return _min; }
            set { _min = value; }
        }

        public T Maximuim
        {
            get { return _max; }
            set { _max = value; }
        }

        public T Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public override bool IsValid()
        {
            return IsValidValue(_value, _min, _max, _frequency);
        }

        public override ushort MetaDataType { get { return (ushort)MetadataEnum.RangedProperty; ; } }

        public override ushort IntegralType { get { return (ushort)CLRIntegralTypeProvider.TypeToTypeEnum<T>(); } }

        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte[] baseData = base.ToByteArray();
            ms.Write(baseData, 0, baseData.Length);
            // serialize the min
            byte [] value = BytesProvider<T>.Default.GetBytes(this._min);
            ms.Write(value, 0, value.Length);

            // serialize the max
            value = BytesProvider<T>.Default.GetBytes(this._max);
            ms.Write(value, 0, value.Length);

            // serialize the frequency
            value = BytesProvider<T>.Default.GetBytes(this._frequency);
            ms.Write(value, 0, value.Length);

            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset = 0)
        {

            int sizeOfT = BareMetal.SizeOf<T>();
            int curPos = base.FromByteArray(data, offset);

            // deserialize the min value
            _min = ByteArryTypeProvider<T>.Default.Convert(data, curPos);
            curPos += sizeOfT;
            // deserialize the max value
            _max = ByteArryTypeProvider<T>.Default.Convert(data, curPos);
            curPos += sizeOfT;
            // deserialize the frequency value
            _frequency = ByteArryTypeProvider<T>.Default.Convert(data, curPos);
            curPos += sizeOfT;

            return curPos;
        }

        #region Private Methods
        /// <summary>
        /// Tests the input value to make sure that is falls between
        /// the min and max. A second test to make sure the value
        /// is a multiple of the increment.
        /// </summary>
        /// <param name="testValue">value to test</param>
        /// <param name="min">the min of this property</param>
        /// <param name="max">the max of this property</param>
        /// <param name="increment">the increment of this property</param>
        /// <returns>returns true if the value is between min/max and is multiple of increment, otherwise false.</returns>
        private bool IsValidValue(T testValue, T min, T max, T increment)
        {
            bool success = true;
            IComparable<T> testValCompare = testValue as IComparable<T>;
            if (testValCompare == null) return false;

            if (testValCompare.CompareTo(min) < 0 ||
                testValCompare.CompareTo(max) > 0)
            {
                success = false;
            }

            if (success)
            {
                success = IsMultipleOfIncrement(testValue, increment);
            }

            return success;
        }

        /// <summary>
        /// Makes sure that the input value is a multiple of the 
        /// increment parameter.
        /// </summary>
        /// <param name="testValue">value to test</param>
        /// <param name="increment">the increment of this property</param>
        /// <returns>returns true if the value is a multiple of the increment, otherwise false.</returns>
        private bool IsMultipleOfIncrement(T testValue, T increment)
        {
            bool success = false;

            // if the object type implements the modulo operation then
            // then use an expression to perform the operation.
            // Otherwise convert the type to an integer and do the
            // modulo operation
            ModuloTypes implmentsModulo = ImplmentsModulo<T>();
            if (implmentsModulo != ModuloTypes.NonNumericType)
            {
                T tdefault = default(T);
                object retValue = tdefault;

                if (implmentsModulo == ModuloTypes.NumericTypeHasModulo)
                {
                    retValue = Modulo(testValue, increment);
                }
                else if (implmentsModulo == ModuloTypes.NumericTypeNoModulo)
                {
                    retValue = IntModulo(testValue, increment);
                }

                IComparable<T> retValCompare = retValue as IComparable<T>;
                if (retValCompare == null) return false;

                success = (retValCompare.CompareTo(tdefault) == 0);
            }
            return success;
        }

        /// <summary>
        /// Using an LINQ expression perform the Modulo operation
        /// for the two parameters.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>returns the value of the modulo operation</returns>
        private T Modulo(T value1, T value2)
        {
            ParameterExpression paramA = Expression.Parameter(typeof(T), "value1"),
                paramB = Expression.Parameter(typeof(T), "value2");
            // add the parameters together
            BinaryExpression body = Expression.Modulo(paramA, paramB);
            // compile it
            Func<T, T, T> modulo = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            T retVal;
            try
            {
                retVal = modulo(value1, value2);
            }
            catch (DivideByZeroException)
            {
                retVal = default(T);
            }
            return retVal;
        }

        private T IntModulo(T testValue, T increment)
        {
            T retValue = default(T);
            //try
            //{
                int convertedTestValue = (int)Convert.ChangeType(testValue, typeof(int));
                int convertedIncrement = (int)Convert.ChangeType(increment, typeof(int));
                retValue = (T)Convert.ChangeType(convertedTestValue % convertedIncrement, typeof(T));
            //}
            //catch (Exception)
            //{
                
            //}
            return retValue;
        }

        private enum ModuloTypes
        {
            NonNumericType,
            NumericTypeNoModulo,
            NumericTypeHasModulo,
        }

        private ModuloTypes ImplmentsModulo<MT>()
        {
            ModuloTypes implementsModulo = ModuloTypes.NumericTypeHasModulo;

            TypeCode tTypeCode = Type.GetTypeCode(typeof(MT));
            switch (tTypeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    implementsModulo = ModuloTypes.NumericTypeNoModulo;
                    break;
                case TypeCode.Object:
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    implementsModulo = ModuloTypes.NonNumericType;
                    break;
                case TypeCode.Single:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    implementsModulo = ModuloTypes.NumericTypeHasModulo;
                    break;
                default:
                    implementsModulo = ModuloTypes.NonNumericType;
                    break;
            }
            return implementsModulo;
        }
        #endregion Private Methods
    }
}
