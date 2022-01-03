//
// File Name: ClientConnection
// ----------------------------------------------------------------------
using System;
using Networking;
using Slf;

namespace ClientEngine.NetClients
{
    /// <summary>
    /// ClientConnection
    /// </summary>
    public class ClientConnection
    {
        #region Private Variables
        TCPAsyncClient _commandClient = null;
        TCPAsyncClient _firmwareClient = null;
        //TCPAsyncClient _statusClient = null;
        TCPAsyncClient _meterClient = null;
        TCPAsyncClient _authClient = null;
        ushort _commandPort = 0;
        string _address;
        ILogger _logger = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ClientConnection()
        {
            _logger = LoggerService.GetLogger();
        }

        public ClientConnection(string address, ushort commandPort)
        {
            _address = address;
            _commandPort = commandPort;
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public bool Connect()
        {

            bool success = false;
            _commandClient = new TCPAsyncClient(_address, _commandPort);
            if (_commandClient == null)
            {
                _logger.Error("Failed to allocate TCP client");
                return success;
            }
            _commandClient.Connect();
            _logger.Info("Connected to {0} on port {1}", _address, _commandPort);

            return success;
        }

        public bool Disconnect(int connectionId)
        {
            _commandClient.Disconnect();
            //_commandClient = null; // HACK: I DO NOT LIKE THIS!
            throw new NotImplementedException();
        }

        //public bool SubscribeToCommands(ulong connectionId, ISendReceiveCmds cmdInterface)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool SubscribeToData(ulong connectionid, ISendRecieveData dataInterface)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
