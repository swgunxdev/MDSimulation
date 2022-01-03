//
// File Name: SrvrCommPorts
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimulationInterface;
using System.IO;

namespace SimulationInterface.Implementations
{
    /// <summary>
    /// SrvrCommPorts
    /// </summary>
    public class SrvrCommPorts : ISerializeByteArray
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>

        /// <summary>
        /// Constructor accepting integers.
        /// </summary>
        /// <param name="commandPort"></param>
        /// <param name="firmwarePort"></param>
        /// <param name="meterPort"></param>
        /// <param name="statusPort"></param>
        public  SrvrCommPorts(int authenticationPort, int commandPort, int firmwarePort, int meterPort)
        {
            Authentication = authenticationPort;
            Command = commandPort;
            Firmware = firmwarePort;
            Meter = meterPort;
        }

        public SrvrCommPorts(byte[] data, int offset)
        {
            this.FromByteArray(data, offset);
        }
        #endregion // Constructor / Dispose

        #region Properties
        public int Authentication { get; protected set; }

        public int Command { get; protected set; }

        public int Firmware { get; protected set; }

        public int Meter { get; protected set; }

        public string Address { get; set; }

        #endregion // Properties

        #region Public Methods
        public byte[] ToByteArray()
        {
            int offset = 0;
            MemoryStream ms = new MemoryStream();

            // serialize the command port value
            byte[] data = System.BitConverter.GetBytes(Command);
            ms.Write(data, offset, data.Length);

            // serialize the authentication port value
            data = System.BitConverter.GetBytes(Authentication);
            ms.Write(data, offset, data.Length);

            // serialize the firmware port value
            data = System.BitConverter.GetBytes(Firmware);
            ms.Write(data, offset, data.Length);

            // serialize the meter port value
            data = System.BitConverter.GetBytes(Meter);
            ms.Write(data, offset, data.Length);

            data = Encoding.UTF8.GetBytes(Address);

            byte[] lenData = null;
            // serialize the address length
            lenData = System.BitConverter.GetBytes(data.Length);
            ms.Write(lenData, offset, lenData.Length);

            ms.Write(data, offset, data.Length);

            return ms.ToArray();
        }

        public int FromByteArray(byte[] data, int offset)
        {
            int curPos = offset;
            Command = System.BitConverter.ToInt32(data, curPos);
            curPos += sizeof(Int32);
            Authentication = System.BitConverter.ToInt32(data, curPos);
            curPos += sizeof(Int32);
            Firmware = System.BitConverter.ToInt32(data, curPos);
            curPos += sizeof(Int32);
            Meter = System.BitConverter.ToInt32(data, curPos);
            curPos += sizeof(Int32);

            // address string length
            int addrLen = System.BitConverter.ToInt32(data, curPos);
            curPos += sizeof(Int32);

            Address = Encoding.UTF8.GetString(data, curPos, addrLen);
            curPos += addrLen;
            return curPos;

        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
    }
}
