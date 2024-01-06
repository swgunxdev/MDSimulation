using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using MDModeling.Providers;
using MDModeling.Utils;

namespace MDModeling
{
    public class StringProperty : PropertyBase
    {
        int _minLength = 0;
        int _maxLength = Int32.MaxValue;
        string _value = string.Empty;

        public StringProperty()
            : base()
        {
        }

        public StringProperty(ushort id, ushort typeId, string name)
            : base(id, typeId, name)
        {
        }

        public StringProperty(ushort id, ushort typeId, string name, string value)
            : base(id, typeId, name)
        {
            _value = value;
        }
        public StringProperty(ushort id, ushort typeId, string name, string value, int min, int max)
            : base(id, typeId, name)
        {
            _value = value;
            _minLength = min;
            _maxLength = max;
        }

        public StringProperty(StringProperty rhs)
            : this(rhs.Id, rhs.TypeId, rhs.Name, rhs.Value, rhs._minLength, rhs._maxLength)
        {
            _value = rhs.Value;
        }

        protected bool IsValidValue(string value)
        {
            return (value.Length >= _minLength && value.Length <= _maxLength);
        }

        public bool IsValid()
        {
            return IsValidValue(Value);
        }

        public string Value
        {
            get { return _value; }
            set
            {
                string tmp = value as string;
                if (tmp != null)
                {
                    if (IsValidValue(tmp))
                    {
                        _value = value as string;
                        RaisePropertyChanged(() => Value);
                    }
                    else
                    {
                        throw new InvalidValueException(string.Format("The set value failed because {0} is an invalid.",tmp));
                    }
                }
            }
        }

        public override bool SetValue(object value)
        {
            string str = value as string;
            if(str != null && IsValidValue(str))
            {
                Value = str;
                return true;
            }

            return false;
        }

        public override object GetValue()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public bool Equals(StringProperty x, StringProperty y)
        {
            if (x == null || y == null) return false;

            if (!base.Equals(x, y)) return false;
            return (x.Value.Equals(y.Value));
        }

        public bool Equals(StringProperty other)
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

            StringProperty strObj = obj as StringProperty;
            if (strObj == null)
                return false;
            else
                return Equals(strObj);
        }

        public static bool operator ==(StringProperty string1, StringProperty string2)
        {
            if ((object)string1 == null || ((object)string2) == null)
                return Object.Equals(string1, string2);

            return string1.Equals(string2);
        }

        public static bool operator !=(StringProperty string1, StringProperty string2)
        {
            if (string1 == null || string2 == null)
                return !Object.Equals(string1, string2);

            return !(string1.Equals(string2));
        }


        public override ushort MetaDataType { get { return (ushort)MetadataEnum.StringProperty; ; } }

        public override ushort IntegralType { get { return (ushort)ClrIntegralType.None; } }

        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte[] baseData = base.ToByteArray();
            ms.Write(baseData, 0, baseData.Length);

            // serialize the length of the string
            byte[] value = Encoding.UTF8.GetBytes(this._value);

            byte[] lengthBytes = BytesProvider<int>.Default.GetBytes(value.Length);

            ms.Write(lengthBytes, 0, lengthBytes.Length);

            // serialize the string
            ms.Write(value, 0, value.Length);

            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset = 0)
        {

            int curPos = base.FromByteArray(data, offset);

            // deserialize the string length
            int strLen = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += Marshaller.SizeOf<int>();
            // deserialize the string
            string str = ByteArryTypeProvider<string>.Default.Convert(data, curPos);
            Value = str.Substring(0, strLen);
            curPos += strLen;

            return curPos;
        }

    }
}
