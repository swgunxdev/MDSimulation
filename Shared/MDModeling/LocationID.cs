using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;


#if MSGPACK
using MsgPack;
#endif
using System.IO;
using System.Diagnostics;
using MDModeling.Providers;
using MDModeling.Utils;

namespace MDModeling
{

    public class LocationID : IEqualityComparer<LocationID>, IComparable<LocationID>
    {
        List<IdNode> _nodes = null;

        public LocationID()
        {
            _nodes = new List<IdNode>();
        }

        public LocationID(LocationID parent)
            : this()
        {
            if (parent != null)
            {
                _nodes = new List<IdNode>(parent._nodes);
            }
        }

        public LocationID(ushort id, ushort type, LocationID parent)
            : this(parent)
        {
            _nodes.Add(new IdNode(id, type));
        }

        public LocationID(IdNode[] nodeData)
            : this()
        {
            if (nodeData != null)
            {
                _nodes = new List<IdNode>(nodeData);
            }
        }

        public ushort NodeId
        {
            get { return _nodes.Last().Id; }
            set
            {
                _nodes.Last().Id = value;
            }
        }

        public ushort NodeType
        {
            get { return _nodes.Last().Type; }
            set { _nodes.Last().Type = value; }
        }


        public List<IdNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value;  }
        }

        public int Count { get { return _nodes.Count;  } }

        /// <summary>
        /// This is the indexer for the location id object
        /// it gets rid of the need to have a location id property.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IdNode this[int index]
        {
            get
            {
                if (index > _nodes.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return _nodes[index];
            }
         }

        /// <summary>
        /// This method changes the parent location data for this
        /// prop node.
        /// </summary>Mar
        /// <param name="parent"></param>
        public void SetParent(LocationID parent)
        {
            // copy the node data for this object
            IdNode me = new IdNode(NodeId, NodeType);

            // remove all nodes (parent and prop)
            _nodes.Clear();

            // Copy the new parents location nodes in
            foreach (IdNode n in parent.Nodes) _nodes.Add(new IdNode(n));

            // copy back in this node data
            _nodes.Add(me);
        }

        /// <summary>
        /// This method changes the parent location data for this
        /// prop node.
        /// </summary>
        /// <param name="parent"></param>
        public LocationID GetParent()
        {
            if (_nodes.Count < 2)
            {
                return null;
            }
            // get all of the nodes minus the last node
            return new LocationID(_nodes.GetRange(0, _nodes.Count - 1).ToArray());
        }

        public int GetHashCode(LocationID obj)
        {
            if (obj == null) return 0;

            return HashHelper.GetHashCode<IdNode>(obj._nodes);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(LocationID x, LocationID y)
        {
            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public bool Equals(LocationID other)
        {
            if (other == null)
                return false;

            if (this._nodes.Count != other._nodes.Count) return false;

            for (int index = 0; index < this._nodes.Count; index++)
            {
                IdNode nodes1 = this._nodes[index];
                IdNode nodes2 = other._nodes[index];

                if (nodes1.Id != nodes2.Id &&
                    nodes1.Type != nodes2.Type)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            LocationID locationObj = obj as LocationID;
            if (locationObj == null)
                return false;
            else
                return Equals(locationObj);
        }

        public static bool operator ==(LocationID location1, LocationID location2)
        {
            if ((object)location1 == null || ((object)location2) == null)
                return Object.Equals(location1, location2);

            return location1.Equals(location2);
        }

        public static bool operator !=(LocationID location1, LocationID location2)
        {
            if (location1 == null || location2 == null)
                return !Object.Equals(location1, location2);

            return !(location1.Equals(location2));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i == 0)
                {
                    sb.AppendFormat("{0}", _nodes[i].Id);
                }
                else
                {
                    sb.AppendFormat(".{0}", _nodes[i].Id);
                }
            }
            return sb.ToString();
        }

        public string ToIdTypeString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i == 0)
                {
                    sb.AppendFormat("{0},{1}", _nodes[i].Id, _nodes[i].Type);
                }
                else
                {
                    sb.AppendFormat(".{0},{1}", _nodes[i].Id, _nodes[i].Type);
                }
            }
            return sb.ToString();
        }

        public byte[] ToByteArray()
        {
            MemoryStream strm = new MemoryStream();

            strm.Write(BytesProvider<int>.Default.GetBytes(_nodes.Count * 2), 0, Marshaller.SizeOf<int>());
            foreach (IdNode node in _nodes)
            {
                byte[] nd = node.ToByteArray();
                strm.Write(nd, 0, nd.Length);
            }
            return strm.ToArray();
        }

        public int FromByteArray(byte[] data, int offset)
        {
            _nodes.Clear();
#if MSGPACK
            MemoryStream strm = new MemoryStream(data);
            MsgPackReader reader = new MsgPackReader(strm);
            FromReader(reader);
            strm.Close();
#else
            int curPos = offset;
            int nodeCount = ByteArryTypeProvider<int>.Default.Convert(data, curPos) / 2;
            curPos += Marshaller.SizeOf<int>();
            while (nodeCount > 0)
            {
                IdNode n = new IdNode();
                curPos = n.FromByteArray(data, curPos);
                _nodes.Add(n);
                nodeCount--;
            }
            return curPos;
#endif
        }

#if MSGPACK
        public void ToWriter(MsgPackWriter writer)
        {
            writer.WriteArrayHeader(_nodes.Count * 2);
            foreach (IdNode node in _nodes)
            {
                writer.Write(node.Id);
                writer.Write(node.Type);
            }
        }

        public void FromReader(MsgPackReader reader)
        {
            uint count = 0;
            uint nodeCount = 1;

            while (reader.Read() && count < nodeCount)
            {
                Debug.Write(string.Format("{0}: ", reader.Type));
                if (reader.IsArray())
                {
                    nodeCount = reader.Length / 2;
                }
                else if (reader.IsUnsigned())
                {
                    _nodes.Add(new IdNode(reader));
                    count++;
                }
            }
        }
#endif

        public int CompareTo(LocationID other)
        {
            int commonEndPoint = 0;
            //
            if (_nodes.Count == other._nodes.Count)
            {
                commonEndPoint = _nodes.Count;
            }

            else if (this._nodes.Count < other._nodes.Count)
            {
                commonEndPoint = _nodes.Count;
            }
            else
            {
                commonEndPoint = other._nodes.Count;
            
            }
            return CompareNodesToIndex(this, other, commonEndPoint);
        }

        private int CompareNodesToIndex(LocationID loc1, LocationID loc2, int stopAtIndex)
        {
            // simply compare node addresses
            for (int index = 0; index < stopAtIndex; index++)
            {
                if (loc1._nodes[index].Id < loc2._nodes[index].Id)
                {
                    return 1;
                }
                else if (loc1._nodes[index].Id > loc2._nodes[index].Id)
                {
                    return -1;
                }
                // otherwise continue
            }
            return 0; // the node ids are the same 
        }
    }
}
