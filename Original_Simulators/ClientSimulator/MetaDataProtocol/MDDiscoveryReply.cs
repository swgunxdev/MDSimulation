using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MetaDataModeling;
using System.IO;
using MetaDataModeling.Providers;
using Earlz.BareMetal;

namespace MetaDataProtocol
{
    public class MDDiscoveryReply : BaseNetMsg, IToFromByteArray
    {
        #region private variables
        IPAddress _ipAddress;
        int _authPort;
        byte[] _extraData = null;
        #endregion

        #region Constructor & Dispose
        public MDDiscoveryReply()
            : base(NetMessages.DiscoverrReply)
        {
            _extraData = new byte[0];
        }

        public MDDiscoveryReply(IPAddress ipAddress, int authPort)
            : this()
        {
            _ipAddress = ipAddress;
            _authPort = authPort;
        }

        public MDDiscoveryReply(IPAddress ipAddress, int authPort, byte [] extraData)
            : this(ipAddress, authPort)
        {
            _extraData = new byte[extraData.Length];
            Buffer.BlockCopy(extraData, 0, _extraData, 0, extraData.Length);
        }

        public MDDiscoveryReply(byte [] data)
        {
            this.FromByteArray(data, 0);
        }

        #endregion

        #region Public Properties
        public IPAddress ipAddress { get { return _ipAddress; } }
        public int AuthenticationPort { get { return _authPort; } }
        public byte[] ExtraData { get { return _extraData; } }
        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte[] b = base.ToByteArray();
            ms.Write(b, 0, b.Length);
            byte [] ip = _ipAddress.GetAddressBytes();
            int ipBytesLenth = ip.Length;
            byte[] iplen = BytesProvider<int>.Default.GetBytes(ipBytesLenth);
            // write the length of the ip address
            ms.Write(iplen, 0, iplen.Length);
            // write the ip address
            ms.Write(ip, 0, ip.Length);
            byte[] p = BytesProvider<int>.Default.GetBytes(_authPort);
            ms.Write(p, 0, p.Length);
            byte[] extLen = BytesProvider<int>.Default.GetBytes(_extraData.Length);
            ms.Write(extLen, 0, extLen.Length);
            if (_extraData.Length > 0)
            {
                ms.Write(_extraData, 0, _extraData.Length);
            }
            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            int sizeOfInt = BareMetal.SizeOf<int>();
            int curPos = offset;
            curPos = base.FromByteArray(data, curPos);

            int ipbyteLength = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += sizeOfInt;
            byte [] ip = new byte[ipbyteLength];
            Buffer.BlockCopy(data, curPos, ip, 0, ipbyteLength);
            curPos += ipbyteLength;
            _ipAddress = new IPAddress(ip);

            _authPort = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += sizeOfInt;
            int extraDataLen = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += sizeOfInt;
            _extraData = new byte[extraDataLen];
            if (extraDataLen > 0)
            {
                Buffer.BlockCopy(data, curPos, _extraData,0, extraDataLen);
                curPos += extraDataLen;
            }
            return curPos;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion


    }
}
