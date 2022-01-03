using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MetaDataModeling
{
    public abstract class PropertyBase : IdType, IMDSerialize
    {
        public PropertyBase()
            : base()
        {
        }

        public PropertyBase(ushort id, ushort typeId, string name)
            : base(id, typeId, name)
        {
        }

        public PropertyBase(PropertyBase propbase)
            : base(propbase)
        {
        }

        public abstract bool SetValue(object value);

        public abstract object GetValue();

        public override byte[] ToByteArray()
        {
            return base.ToByteArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            return base.FromByteArray(data, offset);
        }
    }
}