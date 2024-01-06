using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Networking;
using System.Net;
using System.Threading;
using MetaDataProtocol;
using MetaDataModeling;

namespace ClientEngine
{
    public class DiscoveredModelEventArgs : EventArgs
    {
        public DiscoveredModelEventArgs(IPAddress address, int authenticationPort, byte [] extra)
        {
            this.Address = address;
            this.AuthenticationPort = authenticationPort;
            DiscoveryData = new byte [extra.Length];
            if(extra.Length > 0) Buffer.BlockCopy(extra,0,DiscoveryData,0,extra.Length);
        }
        public IPAddress Address { get; set; }
        public int AuthenticationPort { get; set; }
        public byte [] DiscoveryData { get; set; }
    }

 	/// <summary>
	/// This delegate is used when a new device has arrived
	/// </summary>
	/// <param name="deviceType">This is the device type of the found device</param>
	/// <param name="connectionString">this is the IP address or other connection string of the new device</param>
	/// <param name="data">this is data specific to the device type that can be used by the subscribing object</param>
	public delegate void MDModelArrivalDelegate(DiscoveredModelEventArgs args);
	/// <summary>
	/// This is the delegate used to notify about device removals
	/// </summary>
	/// <param name="connectionString">this is the IP address or other connection string of the removed device</param>
	public delegate void MDModelDepartureDelegate(IPAddress address);
	/// <summary>
	/// This is the event with which to subscribe for device removals
    
    class DiscoveryClient : UDPAsyncClient
    {
        #region Public Events
        /// <summary>
        /// This is the event with which to subscribe for device arrivals
        /// </summary>
        public event MDModelArrivalDelegate MDModelArrival;
        public event MDModelDepartureDelegate MDModelDeparture;
        #endregion // Public Events

        #region private variables
        protected System.Timers.Timer _timer = null;
        //protected MDDiscoveryPing _discoveryPing = new MDDiscoveryPing();
        protected Packet _discoveryPacket = new Packet(0x0101, new Payload(0x1, 0x0001, new MDDiscoveryPing().ToByteArray()));
        #endregion

        #region Public properties
        public bool Working { get; private set; }
        #endregion // Public Properties

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public DiscoveryClient(int port)
            : base( port)
        {
            this.DataOperation = ProcessDiscovery;
        }

        #endregion // Constructor / Dispose

        #region Public Methods
        public override bool Start()
        {
            _timer = new System.Timers.Timer(5000);
            _timer.Elapsed+=new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
            return base.Start();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Write(_discoveryPacket.ToByteArray());
        }

        public override bool Stop()
        {
            _timer.Stop();
            return base.Stop();
        }
        #endregion


        private void TimerFired(object state)
        {
            Write(_discoveryPacket.ToByteArray());
        }

        private byte [] ProcessDiscovery(byte [] data, IPEndPoint from)
        {
            if (this.MDModelArrival != null)
            {
                //if (_clientEndPoint == from) return null;

                Packet replyPacket = new Packet(data);
                if (replyPacket.Payload != null && replyPacket.IsValid)
                {
                    MDDiscoveryReply reply = new MDDiscoveryReply(replyPacket.Payload.PayloadData);
                    DiscoveredModelEventArgs args = new DiscoveredModelEventArgs(from.Address, reply.AuthenticationPort, reply.ExtraData);
                    if (MDModelArrival != null)
                    {
                        _logger.Info("Found device at {0} has authentication port at {1}", reply.ipAddress.ToString(), reply.AuthenticationPort);
                        MDModelArrival(args);
                    }
                }
                else
                {
                    if (replyPacket.Header != null)
                    {
                        _logger.Info(replyPacket.Header.ToString());
                    }
                }
            }
            return null;
        }
    }
}
