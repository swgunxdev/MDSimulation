//
// File Name: ArrayOf.cs
// ----------------------------------------------------------------------
using System.IO;
using MetaDataModeling.Utils;
using Earlz.BareMetal;
using System;
using MetaDataModeling.Providers;

namespace MetaDataModeling
{
	/// <summary>
	/// This class provides the array of a value type in a 
	/// metadata object.
	/// </summary>
	/// <typeparam name="T">The value type used as the type of the 
	/// array.</typeparam>
	public class ArrayOfProperty<T> : PropertyBase
		where T : struct
	{
		protected T [] _data = null;
        protected int _maxSize = 0;

		public ArrayOfProperty()
			: base()
		{
            _data = new T[0];
		}

		public ArrayOfProperty(ushort id, ushort type, string name, T[] defaultValue, int maxSize = 0)
			: base(id, type, name)
		{
            _maxSize = maxSize;
            SetArrayValue(defaultValue);
		}

		public T this[int index]
		{
			get { return _data[index]; }
			set 
			{
				_data[index] = value;
				RaisePropertyChanged(() => Value);
			}
		}

		public T [] Value
		{
			get { return _data; }
			set 
			{
				if (value != null)
				{
                    SetArrayValue(value);
					RaisePropertyChanged(() => Value);
				}
			}
		}

		public override bool SetValue(object value)
		{
			T[] temp = value as T[];
			if (temp != null)
			{
				Value = temp;
				return true;
			}
			return false;
		}

        public override object GetValue()
        {
            return Value;
        }
		
		public override byte [] ToByteArray()
		{
			MemoryStream ms = new MemoryStream();

            // write the id type data
            byte [] baseData = base.ToByteArray();
            ms.Write(baseData, 0, baseData.Length);
            
			// write the number of elements in the array.
			byte [] data = BytesProvider<int>.Default.GetBytes(_data.Length);
			ms.Write(data, 0, data.Length);

			// write the array elements
			for (int index = 0; index < _data.Length; index++ )
			{
				data = BytesProvider<T>.Default.GetBytes(_data[index]);
				ms.Write(data, 0, data.Length);
			}
			return ms.ToArray();
		}

		public override int FromByteArray(byte [] data, int offset)
		{
            // read out the id/type/name/version
			int curPos = base.FromByteArray(data, offset);

            // The following line was put in to test the versioning.
            if (Version != 1) throw new InvalidDataException(string.Format("Bad StorageVersion {0}   {1}", Version,1));

			int elementCount = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
			curPos += BareMetal.SizeOf<int>();

			int size = BareMetal.SizeOf<T>();
			_data = new T [elementCount];
			for (int index = 0; index < elementCount; index++)
			{
				T singleValue = ByteArryTypeProvider<T>.Default.Convert(data, curPos);
				_data[index] = singleValue;
				curPos += size;
			}
			return curPos;
		}

		public override ushort MetaDataType { get { return (ushort)MetadataEnum.ArrayOfProperty; ; } }

		public override ushort IntegralType { get { return (ushort)CLRIntegralTypeProvider.TypeToTypeEnum<T>(); } }

        private void SetArrayValue( T [] value )
        {
            if (value != null)
            {
                if (_maxSize > 0)
                {
                    if (value.Length > _maxSize)
                    {
                        throw new ArgumentOutOfRangeException("defaultValue", "The default array value is longer then the stated maximum size of the array.");
                    }
                    else
                    {
                        _data = new T[_maxSize];
                        value.CopyTo(_data, 0);
                    }
                }
                else
                {
                    _data = new T[value.Length];
                    value.CopyTo(_data, 0);
                }
            }
            else
            {
                _data = new T[0];
            }
        }
	}
}
