//
// File Name: AuthenticationClient
// ----------------------------------------------------------------------
using System;
using System.Net;
using Networking;
using TimpInterfaces.Implementations;

namespace ClientEngine.NetClients
{
    public class AuthenticatedEventArgs : EventArgs
    {
        public AuthenticatedEventArgs(IPAddress address, ushort firmware, ushort command, ushort meter)
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
                //_clientLogger.Debug("Connected creating authentication request");
                //// send a connection query
                //CConnectQuery request = new CConnectQuery();
                //request.Connection.UserName = UserName;
                //request.Connection.Password = Password;
                //Constants.SPacket packet = new Constants.SPacket();
                //request.GetPacketData(out packet);
                //requestGUID = Write(Constants.PacketToByteArray(packet));
                //_clientLogger.Debug("Sent authentication request  to {0} ", this.client.Client.RemoteEndPoint.ToString());

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private void AuthenticationClient_ClientConnectException(object sender, ExceptionEventArgs e)
        {
            this.Working = false;
            if (AuthenticationFailed != null)
            {
                _clientLogger.Error("Exiting on error {0}", e.ToString());
                AuthenticationFailed(e.ToString());
            }
        }

        private void AuthenticationClient_DataRead(object sender, DataReadEventArgs e)
        {
            //uint index = 0;
            //Constants.SPacket packet = Constants.ByteArrayToPacket(e.Data, ref index);
            //CConnectResponse response = new CConnectResponse();
            //if (response.SetPacketData(packet) == false)
            //{
            //    // failed
            //    _clientLogger.Error("Failed to set packet on CConnectResponse from authentication");
            //}
            //AuthenticatedEventArgs args = new AuthenticatedEventArgs(this.addresses[0],
            //                                                        (ushort)response.Connection.FirmwarePort,
            //                                                        (ushort)response.Connection.CommandPort,
            //                                                        (ushort)response.Connection.MeterPort);
            //_clientLogger.Debug("Received authentication reply with data of {0}", response.ToString());

            //if (response.Connection.SuccessStatus && Authenticated != null)
            //{
            //    _clientLogger.Info("Successfully authentication");
            //    _clientLogger.Info("FirmwarePort:{0}  CommandPort:{1}  MeterPort:{2}", args.FirmwarePort, args.CommandPort, args.MeterPort);
            //    Authenticated(args);
            //}
            //if (response.Connection.SuccessStatus == false && AuthenticationFailed != null)
            //{
            //    _clientLogger.Info("Failed to authenticate");
            //    AuthenticationFailed("Authentication Failed");
            //}
            Working = false;
        }

        private string TranslateError(AuthenticationReply reply)
        {
            return string.Empty;
        }

        private void AuthenticationClient_DataWritten(object sender, DataWrittenEventArgs e)
        {
            if (e.Guid != requestGUID) throw new System.Net.Sockets.SocketException();
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
