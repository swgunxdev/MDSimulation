//
// File Name: DiscoverableDevice
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimulationInterface;
using System.Net;

namespace Networking
{
    /// <summary>
    /// DiscoverableDevice
    /// </summary>
    public class DiscoverableDevice  : IDiscoverableDevice
    {
        #region Private Variables
        //CVTable _vtable = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public DiscoverableDevice()
        {
            //_vtable = new CVTable();
        }

        public DiscoverableDevice(UInt32 deviceType, string connStr, string name, Int32 id, object data)
        {
            DeviceType = deviceType;
            ConnectString = connStr;
            Name = name;
            Id = id;
            Data = data;
        }

        public DiscoverableDevice(byte[] data, int p)
        {
            //DecodeDiscoveryResponse(data, (uint)p);
        }

        #endregion // Constructor / Dispose

        #region Properties
        public uint DeviceType { get; protected set; }

        public string Name { get; protected set; }

        public string ConnectString { get; protected set; }

        public object Data
        {
            get
            {
                return null;// _vtable;
            }
            protected set
            {
                //CVTable tmp = value as CVTable;
                //if (tmp != null)
                //{
                //    _vtable = new CVTable(tmp);
                //}
            }
        }

        public int Id { get; protected set; }
        #endregion // Properties

        #region Public Methods
        //public byte[] ToByteArray()
        //{
        //    int offset = 0;
        //    MemoryStream ms = new MemoryStream();

        //    // serialize the DeviceType
        //    byte[] data = System.BitConverter.GetBytes(DeviceType);
        //    ms.Write(data, offset, data.Length);

        //    // serialize the id
        //    data = System.BitConverter.GetBytes(Id);
        //    ms.Write(data, offset, data.Length);

        //    // Serialize the connect string
        //    data = Encoding.UTF8.GetBytes(ConnectString);
        //    byte[] lenData = null;
        //    // serialize the ConnectString length
        //    lenData = System.BitConverter.GetBytes(data.Length);
        //    ms.Write(lenData, offset, lenData.Length);
        //    // write the connect string
        //    ms.Write(data, offset, data.Length);


        //    // serialize the name
        //    data = Encoding.UTF8.GetBytes(Name);
        //    // serialize the name length
        //    lenData = System.BitConverter.GetBytes(data.Length);
        //    ms.Write(lenData, offset, lenData.Length);
        //    // write the name string
        //    ms.Write(data, offset, data.Length);

        //    CVTable vtable = new CVTable();
        //    Constants.SPayload payload = new Constants.SPayload();
        //    vtable.GetPayloadData(out payload);
        //    data = Constants.PayloadToByteArray(payload);
        //    lenData = System.BitConverter.GetBytes(data.Length);
        //    // write the data portion 
        //    ms.Write(lenData, offset, lenData.Length);
        //    ms.Write(data, offset, data.Length);

        //    return ms.ToArray();
        //}

        //public int FromByteArray(byte[] data, int offset)
        //{
        //    int curPos = offset;
        //    DeviceType = System.BitConverter.ToUInt32(data, curPos);
        //    curPos += sizeof(Int32);
        //    Id = System.BitConverter.ToInt32(data, curPos);
        //    curPos += sizeof(UInt32);

        //    string tmpStr = String.Empty;
        //    curPos = GetString(data, curPos, ref tmpStr);
        //    ConnectString = tmpStr;

        //    curPos = GetString(data, curPos, ref tmpStr);
        //    Name = tmpStr;

        //    UInt32 tmpPos = (UInt32)curPos;
        //    int dataLen = System.BitConverter.ToInt32(data, curPos);
        //    Constants.SPayload payload = Constants.ByteArrayToPayload(data, ref tmpPos);
        //    CVTable vtable = new CVTable();
        //    vtable.SetPayloadData(payload);
        //    curPos = (Int32)tmpPos;

        //    return curPos;
        //}

        //private int GetString(byte[] data, int curPos, ref string str)
        //{
        //    // string length
        //    int strLen = System.BitConverter.ToInt32(data, curPos);
        //    curPos += sizeof(Int32);
        //    str = Encoding.UTF8.GetString(data, curPos, strLen);
        //    curPos += strLen;
        //    return curPos;
        //}


        private byte[] EncodeDiscoveryResponse(uint authPort)
        {
            //CDiscoveryResponse response = new CDiscoveryResponse();
            //response.VTable.AuthenticationPort = (ushort)authPort;
            //response.VTable.VTableDevice = GenerateSingleTimp8x8VTable();
            //response.VTable.Count = (byte)response.VTable.VTableDevice.Count();

            //Constants.SPacket packet = new Constants.SPacket();
            //response.GetPacketData(out packet);
            //return ClearOneProtocol.Constants.PacketToByteArray(packet);
            return null;
        }

        //private CVTableDevice[] GenerateSingleTimp8x8VTable()
        //{
        //    CVTableDevice device = new CVTableDevice();
        //    device.DeviceType = (byte)ClearOneProtocol.Constants.EDevices.Timpanogos8x8;
        //    device.GLinkOrder = 0;
        //    IPAddress addr = IPAddress.Parse("127.0.0.1");
        //    device.IPAddress = System.BitConverter.ToUInt32(addr.GetAddressBytes(), 0);
        //    device.MacAddress = 0xDEADBEEF;
        //    device.Name = "SimulatedTimp8x8";
        //    CVTableDevice[] devices = new CVTableDevice[1];
        //    devices[0] = device;
        //    return devices;
        //}

        //private uint DecodeDiscoveryResponse(byte[] data, uint offset)
        //{
        //    uint index = offset;
        //    Constants.SPacket packet = Constants.ByteArrayToPacket(data, ref index);
        //    CDiscoveryResponse response = new CDiscoveryResponse();
        //    response.SetPacketData(packet);

        //    this.DeviceType = response.VTable.VTableDevice[0].DeviceType;
        //    IPAddress addr = new IPAddress(response.VTable.VTableDevice[0].IPAddress);
        //    this.ConnectString = addr.ToString();
        //    this.Name = response.VTable.Name;
        //    this.Data = response.VTable;
        //    return index;
        //}

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
