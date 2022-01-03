using Earlz.BareMetal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MetaDataModeling.Utils;
using MetaDataModeling.Providers;


#if MSGPACK
using MsgPack;
#endif
namespace MetaDataModeling
{
    /// <summary>
    /// Generic Property -- this class encapsulates a property with a name and default value.
    /// </summary>
    /// <typeparam name="T">The integral type used in this class.</typeparam>
    public class GenericProperty<T> : PropertyBase, IEqualityComparer<GenericProperty<T>>
    {
        protected T _value;
        protected T _defaultValue;

        public GenericProperty()
            : base()
        {
        }

        public GenericProperty(ushort id, ushort typeId, string name, T defaultValue)
            : base(id, typeId, name)
        {
            _defaultValue = defaultValue;
            _value = _defaultValue;
        }

        public GenericProperty(GenericProperty<T> rhs)
            : base(rhs)
        {
            _defaultValue = rhs._defaultValue;
            Value = rhs.Value;
        }

        public virtual T Value
        {
            get { return _value; }
            set
            {
                if (IsValidValue(value))
                {
                    _value = value;
                    RaisePropertyChanged(() => Value);
                }
            }
        }

        public virtual T DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public override bool SetValue(object value)
        {
            try
            {
                Value = (T)value;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override object GetValue()
        {
            return Value;
        }

        public virtual bool IsValid()
        {
            return IsValidValue(_value);
        }

        protected virtual bool IsValidValue(T v)
        {
            try
            {
                T tmp = (T)v;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            if (v != null) return true;

            // check to see if the default value is null
            T defValue = default(T);
            if (defValue == null) return true;

            return false;
        }

        public int GetHashCode(GenericProperty<T> obj)
        {
            return HashHelper.CombineHashCode<T>(obj.locationId.GetHashCode(), obj.Value);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(GenericProperty<T> x, GenericProperty<T> y)
        {
            if (x == null || y == null) return false;

            if (!base.Equals(x, y)) return false;
            return (x.Value.Equals(y.Value));
        }

        public bool Equals(GenericProperty<T> other)
        {
            if (other == null)
                return false;

            if (base.Equals(this, other)
                && this.Value.Equals(other.Value))
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            GenericProperty<T> genricObj = obj as GenericProperty<T>;
            if (genricObj == null)
                return false;
            else
                return Equals(genricObj);
        }

        public static bool operator ==(GenericProperty<T> generic1, GenericProperty<T> generic2)
        {
            if ((object)generic1 == null || ((object)generic2) == null)
                return Object.Equals(generic1, generic2);

            return generic1.Equals(generic2);
        }

        public static bool operator !=(GenericProperty<T> generic1, GenericProperty<T> generic2)
        {
            if (generic1 == null || generic2 == null)
                return !Object.Equals(generic1, generic2);

            return !(generic1.Equals(generic2));
        }

        public override byte[] ToByteArray()
		{
            MemoryStream ms = new MemoryStream();
            byte[] baseData = base.ToByteArray();
            ms.Write(baseData, 0, baseData.Length);

            // serialize the default value
            byte[] value = BytesProvider<T>.Default.GetBytes(this._defaultValue);
            ms.Write(value, 0, value.Length);

            // serialize the value
            value = BytesProvider<T>.Default.GetBytes(this._value);
            ms.Write(value, 0, value.Length);


            return ms.ToArray();
        }


        public override int FromByteArray(byte[] data, int offset = 0)
        {
            int sizeOfT = BareMetal.SizeOf<T>();
            int curPos = base.FromByteArray(data, offset);
            
            // deserialize the default value
            _defaultValue = ByteArryTypeProvider<T>.Default.Convert(data, curPos);
            curPos += sizeOfT;
            // deserialize the value
            Value = ByteArryTypeProvider<T>.Default.Convert(data,curPos);
            curPos += sizeOfT;

            return curPos;
        }


#if MSGPACK
        public byte[] ToByteArray2()
        {
            MemoryStream strm = new MemoryStream();
            MsgPackWriter writer = new MsgPackWriter(strm);
            writer.Write(_locId.ToByteArray());
            writer.Write(BytesProvider<T>.Default.GetBytes(this._defaultValue));
            writer.Write(BytesProvider<T>.Default.GetBytes(this._value));
            writer.Write(BytesProvider<T>.Default.GetBytes(this._defaultValue));
            writer.Write(this._name);
            return strm.ToArray();
        }


        public void FromByteArray2(byte [] data, int offset=0)
        {
            MemoryStream strm = new MemoryStream(data,12, data.Length - offset);
            MsgPackReader reader = new MsgPackReader(strm);
            FromReader(reader);
        }

        public void ToWriter(MsgPackWriter writer)
        {
            _locId.ToWriter(writer);
            writer.Write(BytesProvider<T>.Default.GetBytes(this._defaultValue));
            writer.Write(BytesProvider<T>.Default.GetBytes(this._value));
            writer.Write(BytesProvider<T>.Default.GetBytes(this._defaultValue));
            writer.Write(this._name);
        }

        public void FromReader(MsgPackReader reader)
        {
            reader.Read(); // skip past byte array header
            _locId.FromReader(reader);
            // deserialize the default value
            reader.Read();
            if(reader.IsRaw())
            {
                _defaultValue = TypeProivder<T>.Default.Convert(reader.ReadBuffer());
            }

            if(reader.IsRaw())
            {
                Value = TypeProivder<T>.Default.Convert(reader.ReadBuffer());
            }
            Name = reader.ReadRawString();
        }
#endif

        public override ushort MetaDataType { get { return (ushort)MetadataEnum.GenericProperty; ; } }

        public override ushort IntegralType { get { return (ushort)CLRIntegralTypeProvider.TypeToTypeEnum<T>(); } }
    }
}
