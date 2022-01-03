//
// File Name: IdTypeInfo.cs
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using MDModeling.Utils;
using System.ComponentModel;
using MDModeling.Providers;

namespace MDModeling
{
    /// <summary>
    /// 
    /// </summary>
    ///
    public class IdType : ModelNotifyChanged, IEqualityComparer<IdType>, IMDSerialize
    {
        protected string _name;
        protected LocationID _locId = null;
        protected UInt16 _objVer = 1;

        public IdType()
        {
            _locId = new LocationID(0, 0, null);
            _name = string.Empty;
        }

        public IdType(ushort id, ushort typeId, string name)
        {
            _locId = new LocationID(id, typeId, null);
            _name = name;
        }

        public IdType(IdType idInfo)
            : this(idInfo.Id,idInfo.TypeId, idInfo.Name)
        {
            _locId = new LocationID(idInfo._locId);
        }

        public ushort TypeId
        {
            get { return _locId.NodeType; }
            set
            {
                _locId.NodeType = value;
                RaisePropertyChanged(() => TypeId);
            }
        }

        public ushort Id
        {
            get { return _locId.NodeId; }
            set
            {
                _locId.NodeId = value;
                RaisePropertyChanged(() => Id);
            }
        }

        //public ushort ID
        //{
        //    get { return Id; }
        //    set { Id = value; }
        //}

        public LocationID locationId
        {
            get { return _locId; }
            set { _locId = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public UInt16 Version 
        { 
            get
            {
                return _objVer;
            }

            set
            {
                _objVer = value;
                RaisePropertyChanged(() => Version);
            }
        }

        public virtual void SetParent(LocationID parent)
        {
            _locId.SetParent(parent);
        }

        public virtual int GetHashCode(IdType obj)
        {
            return obj._locId.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public virtual bool Equals(IdType x, IdType y)
        {
            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public bool Equals(IdType other)
        {
            if (other == null)
                return false;

            if (this.Id == other.Id
                && this.TypeId == other.TypeId
                && this.Name == other.Name)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            IdType idTypeObj = obj as IdType;
            if (idTypeObj == null)
                return false;
            else
                return Equals(idTypeObj);
        }

        public static bool operator ==(IdType idType1, IdType idType2)
        {
            if ((object)idType1 == null || ((object)idType2) == null)
                return Object.Equals(idType1, idType2);

            return idType1.Equals(idType2);
        }

        public static bool operator !=(IdType idType1, IdType idType2)
        {
            if (idType1 == null || idType2 == null)
                return !Object.Equals(idType1, idType2);

            return !(idType1.Equals(idType2));
        }

        public virtual ushort MetaDataType { get { return (ushort)MetadataEnum.None; ; } }

        public virtual ushort IntegralType { get { return (ushort)ClrIntegralType.None; } }

        public virtual byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();

            byte[] locationData = _locId.ToByteArray();
            ms.Write(locationData, 0, locationData.Length);

            byte[] nameData = Encoding.UTF8.GetBytes(_name);
            byte [] value = BytesProvider<int>.Default.GetBytes(nameData.Length);
            ms.Write(value, 0, value.Length);

            ms.Write(nameData, 0, nameData.Length);
            return ms.ToArray();
        }


        public virtual int FromByteArray(byte[] data, int offset = 0)
        {
            int curPos = offset;

            curPos = _locId.FromByteArray(data, curPos);

            // deserialize the length of the name string
            int nameLength = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += Marshaller.SizeOf<int>();

            // deserialize the name
            _name = ByteArryTypeProvider<string>.Default.Convert(data, curPos);
            _name = _name.Substring(0, nameLength);
            curPos += nameLength;
            return curPos;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            OnPropertyChanged(this._locId, propertyName);
        }
    }
}
