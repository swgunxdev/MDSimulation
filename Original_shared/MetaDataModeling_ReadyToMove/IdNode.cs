using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Earlz.BareMetal;
using System.IO;
using MetaDataModeling.Providers;


#if MSGPACK
using MsgPack;
#endif
using System.Diagnostics;
using MetaDataModeling.Utils;

namespace MetaDataModeling
{
    public class IdNode : IEqualityComparer<IdNode>, IComparable<IdNode>
    {
        ushort _id;
        ushort _type;

        public IdNode()
        {
            _id = 0;
            _type = 0;
        }

        public IdNode(ushort id, ushort type)
        {
            _id = id;
            _type = type;
        }

        public IdNode(IdNode n)
        {
            _id = n._id;
            _type = n._type;
        }


#if MSGPACK
        public IdNode(MsgPackReader reader)
        {
            FromReader(reader);
        }
#endif

        public ushort Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public ushort Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public int GetHashCode(IdNode obj)
        {
            if (obj == null) return 0;

            return HashHelper.GetHashCode<ushort, ushort>(obj.Id, obj.Type);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(IdNode other)
        {
            if (other == null)
                return false;

            if (this._id != other.Id
                || this._type != other.Type)
            {
                return false;
            }

            return true;
        }

        public bool Equals(IdNode x, IdNode y)
        {
            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            IdNode idNodeObj = obj as IdNode;
            if (idNodeObj == null)
                return false;
            else
                return Equals(idNodeObj);
        }

        public static bool operator ==(IdNode id1, IdNode id2)
        {
            if ((object)id1 == null || ((object)id2) == null)
                return Object.Equals(id1, id2);

            return id1.Equals(id2);
        }

        public static bool operator !=(IdNode id1, IdNode id2)
        {
            if (id1 == null || id2 == null)
                return !Object.Equals(id1, id2);

            return !(id1.Equals(id2));
        }

        public byte[] ToByteArray()
        {
            MemoryStream strm = new MemoryStream();
#if MSGPACK
            MsgPackWriter writer = new MsgPackWriter(strm);
            writer.Write(Id);
            writer.Write(Type);
            return strm.ToArray();
#else
            byte [] id = BytesProvider<ushort>.Default.GetBytes(this._id);
            strm.Write(id, 0, id.Length);
            byte[] type = BytesProvider<ushort>.Default.GetBytes(this._type);
            strm.Write(type, 0, type.Length);
#endif
            return strm.ToArray();
        }

        public int FromByteArray(byte[] data, int offset)
        {
#if MSGPACK
            MemoryStream strm = new MemoryStream(data);
            MsgPackReader reader = new MsgPackReader(strm);
            FromReader(reader);
#else
            int sizeOfData = BareMetal.SizeOf<ushort>();
            int curPos = offset;
            Id = ByteArryTypeProvider<ushort>.Default.Convert(data, curPos);
            curPos += sizeOfData;
            Type = ByteArryTypeProvider<ushort>.Default.Convert(data, curPos);
            curPos += sizeOfData;
            return curPos;
#endif
        }

#if MSGPACK
        public void FromReader(MsgPackReader reader)
        {
            // the reader is a stream reader and it should be
            // on the id already no need to read the stream
            this._id = (ushort)reader.ValueUnsigned;

            // move the stream to the next value
            reader.Read();
            this._type = (ushort)reader.ValueUnsigned;
        }
#endif

        public int CompareTo(IdNode other)
        {
            throw new NotImplementedException();
        }
    }
}
