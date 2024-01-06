using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Earlz.BareMetal;
using MetaDataModeling.Utils;
using MetaDataModeling.Extenders;
using MetaDataModeling.Providers;

namespace MetaDataModeling
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    public struct MDHeader
    {
        public UInt16 mdInstanceType; // This is the enumeration for the containing class that wraps the value.
        public UInt16 mdIntegralType; // This is the enumeration for the value type of this payload
        public UInt16 mdObjId; // This is the Object Id
        public UInt16 mdObjType; // This is the Object Type
        public UInt16 mdVersion; // This is the serialized version of the payload
        public UInt32 PayloadLength; // This is just the the length of the payload

        /// <summary>
        /// This constructor will populate a metadata header used 
        /// in serializing metadata objects.
        /// </summary>
        /// <param name="mdtype">metadata type</param>
        /// <param name="bdt">integral type</param>
        /// <param name="objId">metadata object id</param>
        /// <param name="objType">metadata object type</param>
        /// <param name="length">length of the byte array</param>
        public MDHeader(IMDSerialize mdObj, UInt32 length)
        {
            mdInstanceType = mdObj.MetaDataType;
            mdIntegralType = mdObj.IntegralType;
            mdObjId = mdObj.Id;
            mdObjType = mdObj.TypeId ;
            mdVersion = mdObj.Version;
            PayloadLength = length;
        }
    }

    public static class MDHeaderExt
    {
        /// <summary>
        /// Convert the structure to a byte array
        /// </summary>
        /// <param name="hdr">the header to convert</param>
        /// <returns>A byte array containing the header data</returns>
        public static byte[] ToByteArray(this MDHeader hdr)
        {
            MemoryStream ms = new MemoryStream();

            // write metadata instance type
            ms.WriteByteArray(BytesProvider<UInt16>.Default.GetBytes(hdr.mdInstanceType));

            // write the CLR integral type used
            ms.WriteByteArray(BytesProvider<UInt16>.Default.GetBytes(hdr.mdIntegralType));

            // write the metadata id
            ms.WriteByteArray(BytesProvider<UInt16>.Default.GetBytes(hdr.mdObjId));

            // write the metadata type
            ms.WriteByteArray(BytesProvider<UInt16>.Default.GetBytes(hdr.mdObjType));

            // write the metadata type
            ms.WriteByteArray(BytesProvider<UInt16>.Default.GetBytes(hdr.mdVersion));

            // write the length of the metadata serialization buffer
            ms.WriteByteArray(BytesProvider<UInt32>.Default.GetBytes(hdr.PayloadLength));

            // return a byte []
            return ms.ToArray();
        }

        /// <summary>
        /// Converts a byte array into a metadata header object.
        /// </summary>
        /// <param name="data">The byte array holding the serialized header.</param>
        /// <param name="offset">The offset into the array that the header exists at.</param>
        /// <returns>A tuple object that contains the header and the offset in the array after the header.</returns>
        public static MDHeader FromByteArray(byte[] data, int offset)
        {
            int curPos = offset;
            MDHeader hdr = new MDHeader();

            // read metadata instance type
            hdr.mdInstanceType = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt16>();

            // read the CLR integral type used
            hdr.mdIntegralType = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt16>();

            // read the metadata id
            hdr.mdObjId = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt16>();

            // read the metadata type
            hdr.mdObjType = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt16>();

            // read the metadata type
            hdr.mdVersion = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt16>();

            // read the length of the metadata serialization buffer
            hdr.PayloadLength = ByteArryTypeProvider<UInt32>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<UInt32>();

            // return the header
            return hdr;
        }
    }
}
