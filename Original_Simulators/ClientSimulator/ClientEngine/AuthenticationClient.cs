//
// File Name: AuthenticationClient
// ----------------------------------------------------------------------
using System;
using System.Net;
using Networking;
using TimpInterfaces.Implementations;
using MetaDataProtocol;
using MetaDataModeling;

namespace ClientEngine
{
    public class AuthenticatedEventArgs : EventArgs
    {
        public AuthenticatedEventArgs(IPAddress address)
        {
            this.Address = address;
        }
        public AuthenticatedEventArgs(IPAddress address, ushort firmware, ushort command, ushort meter)
            : this(address)
        {
            this.FirmwarePort = firmware;
            this.CommandPort = command;
            this.MeterPort = meter;
        }
        public IPAddress Address { get; set; }
        public ushort FirmwarePort { get; set; }
        public ushort CommandPort { get; set;}
        public ushort MeterPort { get; set;}
    }

    public delegate void AuthenticatedDelegate(AuthenticatedEventArgs args);
    public delegate void AuthenticationFailedDelegate(string errString);

    /// <summary>
    /// AuthenticationClient
    /// </summary>
    public class AuthenticationClient : TCPAsyncClient
    {
        #region Private Variables
        private Guid requestGUID;
        #endregion // Private Variables00

        #region Public Events
        public event AuthenticatedDelegate Authenticated;
        public event AuthenticationFailedDelegate AuthenticationFailed;
        #endregion // Public Events

        #region Public properties
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Working { get; private set; }
        #endregion // Public Properties

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public AuthenticationClient(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            this.Connected += new EventHandler(OnConnected);
            this.ClientConnectException += new EventHandler<ExceptionEventArgs>(AuthenticationClient_ClientConnectException);
            this.ClientReadException += new EventHandler<ExceptionEventArgs>(AuthenticationClient_ClientReadException);
            this.ClientWriteException += new EventHandler<ExceptionEventArgs>(AuthenticationClient_ClientWriteException);
            this.DataRead += new EventHandler<DataReadEventArgs>(AuthenticationClient_DataRead);
            this.DataWritten += new EventHandler<DataWrittenEventArgs>(AuthenticationClient_DataWritten);
        }

        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public void Start()
        {
            Working = true;
            Connect();
        }
        #endregion // Public Methods

        #region Private Methods
        private void OnConnected(object sender, EventArgs args)
        {
            try
            {
                _clientLogger.Debug("Connected creating authentication request");

                // send a connection query
                MDAuthenticateMsg request = new MDAuthenticateMsg(UserName, Password);
                Packet authPacket = new Packet(1, new Payload((ushort)NetMessages.AuthConnectMsg, 1, request.ToByteArray()));
                requestGUID = Write(authPacket.ToByteArray());

                _clientLogger.Debug("Sent authentication request  to {0} write GUID: {1}", 
                                    this.client.Client.RemoteEndPoint.ToString(),
                                    requestGUID.ToString());

            }
            catch (Exception ex)
            {
                _clientLogger.Error("DANGER DANGER Will Robinson exception thrown {0}", ex.Message);
                throw ex;
            }
        }

        private void AuthenticationClient_ClientConnectException(object sender, ExceptionEventArgs e)
        {
            this.Working = false;
            Disconnect();
            if (AuthenticationFailed != null)
            {
                _clientLogger.Error("AuthenticationClient_ClientConnectException Exiting on error {0}", e.ToString());
                AuthenticationFailed(e.ToString());
            }
        }

        private void AuthenticationClient_DataRead(object sender, DataReadEventArgs e)
        {
            uint index = 0;
            Packet authPacket = new Packet(e.Data);
            MDAuthenticateReply response = new MDAuthenticateReply(authPacket.Payload.PayloadData);
            if (authPacket.IsValid == false)
            {
                // failed
                _clientLogger.Error("Failed to set packet on MDAuthenticaionReply from authentication");
            }
            _clientLogger.Debug("Received authentication reply with data of {0}", response.ToString());
            if (response.Return == AuthenticationValues.SUCCESS && Authenticated != null)
            {
                AuthenticatedEventArgs args = new AuthenticatedEventArgs(this.addresses[0],
                                                                        (ushort)response.FirmwarePort,
                                                                        (ushort)response.CommandPort,
                                                                        (ushort)response.MeterPort);
                _clientLogger.Info("Successfully authentication");
                _clientLogger.Info("FirmwarePort:{0}  CommandPort:{1}  MeterPort:{2}", args.FirmwarePort, args.CommandPort, args.MeterPort);
                Authenticated(args);
            }
            if (
                AuthenticationFailed != null)
            {
                _clientLogger.Info("Failed to authenticate");
                AuthenticationFailed("Authentication Failed");
            }
            Disconnect();
            Working = false;
        }

        private string TranslateError(AuthenticationReply reply)
        {
            return string.Empty;
        }

        private void AuthenticationClient_DataWritten(object sender, DataWrittenEventArgs e)
        {
            if (e.Guid != requestGUID)
            {
                _clientLogger.Error("AuthenticationClient_DataWritten: Wrong write request GUID, throwing exception {0}", e.ToString());
                throw new System.Net.Sockets.SocketException();
            }
            _clientLogger.Info("Data write for request {0} successful.", e.Guid.ToString());
        }

        private void AuthenticationClient_ClientWriteException(object sender, ExceptionEventArgs e)
        {
            this.Working = false;
            if (AuthenticationFailed != null)
            {
                _clientLogger.Error("Exiting on error {0}", e.ToString());
                AuthenticationFailed(e.ToString());
            }
        }

        private void AuthenticationClient_ClientReadException(object sender, ExceptionEventArgs e)
        {
            this.Working = false;
            if (AuthenticationFailed != null)
            {
                _clientLogger.Error("Exiting on error {0}", e.ToString());
                AuthenticationFailed(e.ToString());
            }
        }

        #endregion // Private Methods

        #region enums
        #endregion

    }
}
