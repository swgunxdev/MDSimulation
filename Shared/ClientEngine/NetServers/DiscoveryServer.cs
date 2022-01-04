//
// File Name: DiscoveryServer
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Networking;
using TimpInterfaces;
using System.Net;
using MetaDataModeling;
using MetaDataProtocol;

namespace ClientEngine.NetServers
{
    /// <summary>
    /// DiscoveryServer
    /// </summary>
    public class DiscoveryServer : UDPAsyncServer
    {
        #region Private Variables
        private const int _ListenPort = 3488;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public DiscoveryServer()
            : base(_ListenPort)
        {
            this.DataOperation = HandleDataReceive;
			Name = "Discovery";
        }

        #endregion // Constructor / Dispose

        #region Properties
        public DiscoveryDataDelegate DiscoveryRespDataCall { get; set; }

        public GetOperatingPortDelegate APort { get; set; }
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        private byte [] HandleDataReceive(byte[] bytes, IPEndPoint from)
        {
            // what kind of packet is this?
            // send to the correct place
            //
            UInt32 index = 0;
            byte[] discoveryData = null;

            Packet discoveryPacket = new Packet(bytes);
            if (discoveryPacket.Payload.Header.PayloadType == (ushort)NetMessages.DiscoveryMsg)
            {
                if (DiscoveryRespDataCall != null)
                {
                    discoveryData = DiscoveryRespDataCall((uint)APort());
                }
                else
                {

                    discoveryData = new byte[0];
                }

            }
            MDDiscoveryReply reply = new MDDiscoveryReply(ReturnOperatingIP(), APort(), discoveryData);            
            Packet  discoveryReplyPacket = new Packet(1,new Payload((ushort)NetMessages.DiscoveryReply, 1, reply.ToByteArray()));
            return discoveryReplyPacket.ToByteArray();
        }
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
