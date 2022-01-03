using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MetaDataModeling;
using MetaDataProtocol;
using System.Net;


namespace MetadataUnitTesting
{
    [TestClass]
    public class MetaDataProtocolTests
    {
        [TestMethod]
        public void PayloadSerilization()
        {
            byte[] data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, };
            UInt16 pType = 4;
            UInt16 pVer = 43;

            Payload p = new Payload(pType, pVer, data);
            Assert.AreEqual(pType, p.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p.Header.PayloadLength);
            Assert.AreEqual(pVer, p.Header.PayloadVersion);

            byte[] buf = p.ToByteArray();
            int index = 0;

            Payload p2 = new Payload();
            index = p2.FromByteArray(buf, index);

            Assert.AreEqual(pType, p2.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p2.Header.PayloadLength);
            Assert.AreEqual(pVer, p2.Header.PayloadVersion);
            CollectionAssert.Equals(data, p2.PayloadData);
        }

        [TestMethod]
        public void PacketSerilization()
        {
            byte[] data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, };
            UInt16 pType = 4;
            UInt16 pVer = 43;

            Payload p = new Payload(pType, pVer, data);
            Assert.AreEqual(pType, p.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p.Header.PayloadLength);
            Assert.AreEqual(pVer, p.Header.PayloadVersion);

            UInt16 packetType = 45;
            Packet packet = new Packet(pVer, p);

            
            byte[] buf = packet.ToByteArray();
            int index = 0;

            Packet p2 = new Packet();
            index = p2.FromByteArray(buf, index);

            Assert.AreEqual(packet.Header.PacketLength, p2.Header.PacketLength);
            Assert.AreEqual(pVer, p2.Header.PacketVersion);
            CollectionAssert.Equals(data, p2.Payload.PayloadData);
        }

        [TestMethod]
        public void MDDiscoveryPingSerilization()
        {
            MDDiscoveryPing ping = new MDDiscoveryPing();
            UInt16 pType = 4;
            UInt16 pVer = 43;

            byte[] data = ping.ToByteArray();
            Payload p = new Payload(pType, pVer, data);
            Assert.AreEqual(pType, p.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p.Header.PayloadLength);
            Assert.AreEqual(pVer, p.Header.PayloadVersion);

            UInt16 packetType = 45;
            Packet packet = new Packet( pVer, p);


            byte[] buf = packet.ToByteArray();
            int index = 0;

            Packet p2 = new Packet();
            index = p2.FromByteArray(buf, index);

            Assert.AreEqual(packet.Header.PacketLength, p2.Header.PacketLength);
            Assert.AreEqual(pVer, p2.Header.PacketVersion);
            MDDiscoveryPing ping2 = new MDDiscoveryPing(p2.Payload.PayloadData);
            Assert.AreEqual(ping.CmdId, ping2.CmdId);
        }

        [TestMethod]
        public void MDDiscoveryReplySerilization()
        {
            MDDiscoveryReply reply = new MDDiscoveryReply(new IPAddress(0x2414188f),50);
            UInt16 pType = 4;
            UInt16 pVer = 43;

            byte[] data = reply.ToByteArray();
            Payload p = new Payload(pType, pVer, data);
            Assert.AreEqual(pType, p.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p.Header.PayloadLength);
            Assert.AreEqual(pVer, p.Header.PayloadVersion);

            UInt16 packetType = 45;
            Packet packet = new Packet(pVer, p);


            byte[] buf = packet.ToByteArray();
            int index = 0;

            Packet p2 = new Packet();
            index = p2.FromByteArray(buf, index);

            Assert.AreEqual(packet.Header.PacketLength, p2.Header.PacketLength);
            Assert.AreEqual(pVer, p2.Header.PacketVersion);
            MDDiscoveryReply reply2 = new MDDiscoveryReply(p2.Payload.PayloadData);
            Assert.IsTrue(reply2.ipAddress.Equals(new IPAddress(0x2414188f)));
            Assert.AreEqual(50, reply2.AuthenticationPort);
            Assert.AreEqual(reply.CmdId, reply2.CmdId);
            Assert.AreEqual(0, reply2.ExtraData.Length);
        }
        [TestMethod]
        public void MDDiscoveryReplyExtraDataSerilization()
        {
            byte [] extraData = new byte [] {0x0,0x1,0x2,0x3,0x4,0x5};
            MDDiscoveryReply reply = new MDDiscoveryReply(new IPAddress(0x2414188f), 50, extraData);
            UInt16 pType = 4;
            UInt16 pVer = 43;

            byte[] data = reply.ToByteArray();
            Payload p = new Payload(pType, pVer, data);
            Assert.AreEqual(pType, p.Header.PayloadType);
            Assert.AreEqual((UInt32)data.Length, p.Header.PayloadLength);
            Assert.AreEqual(pVer, p.Header.PayloadVersion);

            UInt16 packetType = 45;
            Packet packet = new Packet(pVer, p);


            byte[] buf = packet.ToByteArray();
            int index = 0;

            Packet p2 = new Packet();
            index = p2.FromByteArray(buf, index);

            Assert.AreEqual(packet.Header.PacketLength, p2.Header.PacketLength);
            Assert.AreEqual(pVer, p2.Header.PacketVersion);
            MDDiscoveryReply reply2 = new MDDiscoveryReply(p2.Payload.PayloadData);
            Assert.IsTrue(reply2.ipAddress.Equals(new IPAddress(0x2414188f)));
            Assert.AreEqual(50, reply2.AuthenticationPort);
            Assert.AreEqual(reply.CmdId, reply2.CmdId);
            CollectionAssert.AreEqual(extraData, reply2.ExtraData);
        }
    }
}
