using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Earlz.BareMetal;
using MetaDataModeling;
using System.Runtime.InteropServices;
using MetaDataModeling.Providers;
using MetaDataModeling.Utils;

namespace MetaDataModeling
{
    public class ObjVersion : EqualityComparer<ObjVersion>, IComparable<ObjVersion>
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ObjVersion()
        {
            Major = 0;
            Minor = 0;
            Revision = 0;
            Build = 1;
        }

        public ObjVersion(byte major, byte minor, byte revision, byte build)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
        }

        public ObjVersion(ObjVersion target)
        {
            this.Major = target.Major;
            this.Minor = target.Minor;
            this.Revision = target.Revision;
            this.Build = target.Build;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public byte Major { get; set; }
        public byte Minor { get; set; }
        public byte Revision { get; set; }
        public byte Build { get; set; }
        #endregion // Properties

        #region Public Methods
        public byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();

            // serialize the Major
            byte[] value = BytesProvider<Byte>.Default.GetBytes(this.Major);
            ms.Write(value, 0, value.Length);

            // serialize the Minor
            value = BytesProvider<Byte>.Default.GetBytes(this.Minor);
            ms.Write(value, 0, value.Length);

            // serialize the Revision
            value = BytesProvider<Byte>.Default.GetBytes(this.Revision);
            ms.Write(value, 0, value.Length);

            // serialize the Build
            value = BytesProvider<Byte>.Default.GetBytes(this.Build);
            ms.Write(value, 0, value.Length);

            return ms.ToArray();
        }

        public int FromByteArray(byte[] data, int offset = 0)
        {
            int curPos = offset;

            // deserialize the Major value
            Major = ByteArryTypeProvider<Byte>.Default.Convert(data, curPos);
            curPos += sizeof(Byte);
            // deserialize the Minor value
            Minor = ByteArryTypeProvider<Byte>.Default.Convert(data, curPos);
            curPos += sizeof(Byte);
            // deserialize the Revision
            Revision = ByteArryTypeProvider<Byte>.Default.Convert(data, curPos);
            curPos += sizeof(Byte);
            // deserialize the Build
            Build = ByteArryTypeProvider<Byte>.Default.Convert(data, curPos);
            curPos += sizeof(Byte);

            return curPos;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}.{1}.{2}.{3}", Major, Minor, Revision, Build);
            return builder.ToString();
        }

        public override int GetHashCode(ObjVersion obj)
        {
            if (obj == null) return 0;

            return HashHelper.GetHashCode<byte, byte, byte, byte>(obj.Major, obj.Minor, obj.Revision, obj.Build);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(ObjVersion x, ObjVersion y)
        {
            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public bool Equals(ObjVersion other)
        {
            if (other == null)
                return false;

            if (this.Major == other.Major
                && this.Minor == other.Minor
                && this.Revision == other.Revision
                && this.Build == other.Build)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            ObjVersion objVer = obj as ObjVersion;
            if (objVer == null)
                return false;
            else
                return Equals(objVer);
        }

        public static bool operator == (ObjVersion ver1, ObjVersion ver2)
        {
            if ((object)ver1 == null || ((object)ver2) == null)
                return Object.Equals(ver1, ver2);

            return ver1.Equals(ver2);
        }

        public static bool operator != (ObjVersion ver1, ObjVersion ver2)
        {
            if (ver1 == null || ver2 == null)
                return !Object.Equals(ver1, ver2);

            return !(ver1.Equals(ver2));
        }

        public int CompareTo(ObjVersion other)
        {
            
            if (this.Equals(other)) return 0;

            if (this.Major < other.Major)
                return -1;
            else if (this.Major > other.Major)
                return 1;

            if (this.Minor < other.Minor)
                return -1;
            else if (this.Minor > other.Minor)
                return 1;

            if (this.Revision < other.Revision)
                return -1;
            else if (this.Revision > other.Revision)
                return 1;

            if (this.Build < other.Build)
                return -1;
            else if (this.Build > other.Build)
                return 1;

            return 0;
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods
    }
}
