//
// File Name: ClrPayload.cs
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Earlz.BareMetal;
using System.IO;
//using ClearOneMetadata.Extenders;

namespace MetaDataModeling
{
    /// <summary>
    /// This extension class contains the business logic to turn
    /// metadata objects into byte arrays and back.
    /// </summary>
    public static class MetaDataPayload
    {
        /// <summary>
        /// Convert a metadata object that supports the IClrOneSerialize interface into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The byte array representing the serialized object.</returns>
        public static byte[] ToByteArray(IdType obj)
        {
            IMDSerialize encoder = obj as IMDSerialize;
            if(encoder == null)
            {
                throw new InvalidOperationException(string.Format("Unable to encode object id:{0} type:{1}",obj.Id,obj.TypeId));
            }
            byte[] objData = encoder.ToByteArray();

            MemoryStream ms = new MemoryStream();

            MDHeader hdr = new MDHeader(obj, (UInt32)objData.Length);
            byte[] value = MDHeaderExt.ToByteArray(hdr);
            // write the header
            ms.Write(value, 0, value.Length);
            // write the payload
            ms.Write(objData, 0, objData.Length);
            // return a byte array
            return ms.ToArray();
        }

        /// <summary>
        /// Convert a byte array into an actual metadata object.
        /// </summary>
        /// <param name="data">The byte array containing the serialized data.</param>
        /// <param name="offset">How far into the array the object is located.</param>
        /// <returns>A tuple containing the metadata object and the new offset to the next object in the byte array.</returns>
        public static Tuple<IdType, int> FromByteArray( byte [] data, int offset=0)
        {
            MDHeader header = MDHeaderExt.FromByteArray(data, offset);
            int curPos = offset + BareMetal.SizeOf<MDHeader>();

            // Create an instance of the metadata object type
            IdType mdInstance = MetaDataFactory.CreateMDInstance(
                                            header.mdInstanceType,
                                            header.mdIntegralType,
                                            header.mdObjId,
                                            header.mdObjType);

            // Did the creation fail??
            if (mdInstance == null)
            {
                // Scream loudly if the object could not be created.
                throw new InvalidDataException("Could not create an instance of metadata object.");
            }

            // Make sure that the type implements the serialization interface
            IMDSerialize decoder = mdInstance as IMDSerialize;
            if (decoder == null)
            {
                // Scream loudly if the object does not implement 
                // the interface.
                throw new InvalidCastException(string.Format("Instance of {0} does not implement serialization interface.", mdInstance.GetType().ToString()));
            }

            // Populate the object with the data from the byte array.
            curPos = decoder.FromByteArray(data, curPos);

            // Return the object and the new offset.
            return new Tuple<IdType, int>(mdInstance as IdType, curPos);
        }
    }
}
