//
// File Name: ClientCommMgr
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using ClientEngine.NetClients;
using Networking;
using Networking.Interfaces;
using Slf;
using TimpInterfaces;

namespace ClientEngine
{
    public class OnlineSystem : IMDModel
    {
        private  byte [] _configuration;

        public OnlineSystem(uint deviceType, string name, string connectString, object data)
        {
            DeviceType = deviceType;
            Name = name;
            ConnectString = connectString;
            Data = data;
            //CVTable vtable = data as CVTable;
            //if (vtable != null)
            //{
            //    AuthenticationPort = vtable.AuthenticationPort;
            //}
            Id = GenId(connectString);
        }

        public OnlineSystem(DiscoveredModelEventArgs eventArgs)
        {
            DeviceType = 0;
            Name = string.Empty;
            AuthenticationPort = eventArgs.AuthenticationPort;
            ConnectString = eventArgs.Address.ToString();
            Id = eventArgs.Address.GetHashCode();
            Configuration = eventArgs.DiscoveryData;
        }

        public OnlineSystem(OnlineSystem system)
        {
            DeviceType = system.DeviceType;
            Name = system.Name;
            ConnectString = system.ConnectString;
            Data = system.Data;
            AuthenticationPort = system.AuthenticationPort;
            Configuration = system.Configuration;
            Id = system.Id;
        }

        public uint DeviceType { get; protected set; }
        public string Name { get; protected set; }
        public string ConnectString { get; protected set; }
        public object Data { get; protected set; }
        public int Id { get; protected set; }

        public static int GenId(string connectString)
        {
            
            IPAddress address = IPAddress.Parse(connectString);
            return address.GetHashCode();
        }

        public int AuthenticationPort { get; set; }


        public List<IClrOneChannel> Channels
        {
            get { throw new NotImplementedException(); }
        }

        ushort IMDModel.AuthenticationPort
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasConfiguration
        {
            get
            {
                if(_configuration != null && 
                    _configuration.Length > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public byte[] Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                _configuration = new byte [value.Length];
                if(value.Length > 0) Buffer.BlockCopy(value, 0,_configuration,0, value.Length);
            }
        }
    }

    /// <summary>
    /// ClientCommMgr
    /// </summary>
    public class ClientCommMgr : IClientCommsMgr
    {
        #region Private Variables
//        Locator _locator = null;
        DiscoveryClient _locator = null;
        FakeDiscovery _fakeLocator = null;
        ILogger _logger = null;

        Dictionary<int, IMDModel> _onlineSystems = null;
        Dictionary<int, ConnectedClient> _connectedClients = null;
        #endregion // Private Variables

        #region Public Events
        public event VDeviceArrival OnVDeviceArrival;

        public event VDeviceLeave OnVDeviceLeaving;

        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ClientCommMgr()
        {
            _onlineSystems = new Dictionary<int, IMDModel>();
            _connectedClients = new Dictionary<int, ConnectedClient>();
            ILogger[] loggers = new ILogger[] { LoggerService.GetLogger("debuglogger"), LoggerService.GetLogger() };
            _logger = new Slf.CompositeLogger(loggers);
        }
        #endregion // Constructor / Dispose

        #region Properties
        public Dictionary<int, IMDModel> DeviceList
        {
            get { return _onlineSystems; }
        }
        #endregion // Properties

        #region Public Methods
        public bool Connect(int id, string username, string password)
        {
            if (this._onlineSystems.ContainsKey(id) == false)
            {
                _logger.Debug("FAILED to find device id {0} in online systems", id);
                return false;
            }

            OnlineSystem device = _onlineSystems[id] as OnlineSystem;
            _connectedClients.Add(id, new ConnectedClient(device));
            _connectedClients[id].Connect(username, password);
            return true;
        }
        public bool Disconnect(int connectionId)
        {
            _logger.Debug("ClientCommMgr::Disconnect");
            // send the disconnect command?
            if (_connectedClients.ContainsKey(connectionId))
            {
                _connectedClients[connectionId].Disconnect();
            }
            return true;
        }

        public bool SubscribeToCommands(ulong connectionId, ISendReceiveCmds cmdInterface)
        {
            throw new NotImplementedException();
        }

        public bool SubscribeToData(ulong connectionid, ISendRecieveData dataInterface)
        {
            throw new NotImplementedException();
        }

        public void StartDiscovery()
        {
            _logger.Info("Starting Discovery");

            _locator = new DiscoveryClient(3488);
            if (_locator != null)
            {
                _locator.MDModelArrival += new MDModelArrivalDelegate(_locator_DeviceArrival);
                _locator.MDModelDeparture += new MDModelDepartureDelegate(_locator_DeviceRemoval);
                _locator.Start();
            }
            this._onlineSystems.Clear();
        }

        public void StopDiscovery()
        {
            _logger.Info("Stopping Discovery");
            if (_locator != null)
            {
                _locator.Stop();
                _locator.MDModelArrival -= new MDModelArrivalDelegate(_locator_DeviceArrival);
                _locator.MDModelDeparture -= new MDModelDepartureDelegate(_locator_DeviceRemoval);
                _locator = null;
            }
        }
        #endregion // Public Methods

        #region Private Methods
        private void _locator_DeviceRemoval(IPAddress modelAddress)
        {
            _logger.Debug("Device Leaving: {0}", modelAddress.ToString());
            // Figure out which device in the list it is.
            int id = modelAddress.GetHashCode();
            // If there is a client connection tell someone to drop the use
            // of the connection add to the dead open connections. Then remove from the list of devices.
            // exit, while waiting to be told that the connection is no longer in use as part of this client.
            if (_onlineSystems.ContainsKey(id))
            {
                _logger.Debug("Found device {0} in online system list removing", modelAddress.ToString());
                _onlineSystems.Remove(id);
            }
            if (_connectedClients.ContainsKey(id))
            {
                _logger.Debug("Found device {0} in connected clients list removing", modelAddress.ToString());
                _connectedClients[id].Disconnect();
            }
        }

        private void _locator_DeviceArrival(DiscoveredModelEventArgs args)
        {
            _logger.Debug("Device Arrived: {0} ", args.Address.ToString());
            // Add new device to the list
            OnlineSystem newSystem = new OnlineSystem(args);
            if (_onlineSystems.ContainsKey(newSystem.Id))
            {
                _logger.Debug("Already found {0}", args.Address.ToString());
                return;
            }
            this._onlineSystems.Add(newSystem.Id, newSystem);
            _logger.Debug("Added device {0} to online system list", args.Address.ToString());
            // tell the world about the new arrival
        }
        #endregion // Private Methods

        #region enums
        #endregion

        public void StopAll()
        {
            foreach (KeyValuePair<int, ConnectedClient> kvp in _connectedClients)
            {
                kvp.Value.Disconnect();
            }
        }
    }
}
