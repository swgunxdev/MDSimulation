//
// File Name: ConnectedClient
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slf;

namespace ClientEngine.NetClients
{
    /// <summary>
    /// ConnectedClient
    /// </summary>
    public class ConnectedClient
    {
        #region Private Variables
        OnlineSystem _deviceStack = null;
        ILogger _logger = null;
        AuthenticationClient _authClient = null;
        CommandClient _cmdClient = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ConnectedClient(OnlineSystem system)
        {
            _deviceStack = new OnlineSystem(system);
            ILogger[] loggers = new ILogger[] { LoggerService.GetLogger("debuglogger"), LoggerService.GetLogger() };
            _logger = new Slf.CompositeLogger(loggers);
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public bool Connect(string username, string password)
        {
            _logger.Debug("Attempting authentication to {0} at port {1}", _deviceStack.ConnectString, _deviceStack.AuthenticationPort);
            _authClient = new AuthenticationClient( _deviceStack.ConnectString, _deviceStack.AuthenticationPort);
            _authClient.Authenticated += new AuthenticatedDelegate(authClient_Authenticated);
            _authClient.AuthenticationFailed += new AuthenticationFailedDelegate(authClient_AuthenticationFailed);
            _authClient.UserName = username;
            _authClient.Password = password;
            _authClient.Start();
            while (_authClient.Working)
            {
                System.Threading.Thread.Sleep(500);
            }
            return true;
        }
        public bool Disconnect()
        {
            _logger.Debug("ConnectedClient::Disconnect");
            if(_cmdClient != null) _cmdClient.Disconnect();
            //_firmwareClient.Disconnect();
            //_meterClient.Disconnect();
            return true;
        }
        #endregion // Public Methods

        #region Private Methods
        private void authClient_AuthenticationFailed(string cause)
        {
            _logger.Debug("FAILED to authentication {0}", cause);
        }

        private void authClient_Authenticated(AuthenticatedEventArgs args)
        {
            _logger.Debug("Successful authentication to {0}", args.Address);
            // spawn clients on other device and maintain those connections
            _logger.Debug("Starting other client connections");
            this._cmdClient = new CommandClient(args.Address, args.CommandPort);
            this._cmdClient.Connect();
        }

        #endregion // Private Methods

        #region enums
        #endregion

    }
}
