//
// File Name: SimDeviceCommMgr
// ----------------------------------------------------------------------
using ClientEngine.NetServers;
using Networking;
using Networking.Interfaces;
using TimpInterfaces;
using TimpInterfaces.Implementations;
using System;
using Slf;

namespace ClientEngine
{
    /// <summary>
    /// SimDeviceCommMgr
    /// </summary>
    public class SimDeviceCommMgr : ModelNotifyChanged, IDeviceCommMgr
    {
        #region Private Variables
        IDSStorageModel _dataModel = null;
        int _authPort = 0;
        int _discoveryPort = 3488;

        bool _glinkDiscoveryRunning = false;
        bool _commandServerRunning = false;
        bool _authServerRunning = false;
        bool _firmwareServerRunning = false;
        bool _discoveryServerRunning = false;
        bool _fakeDiscovery = false;
        
        DiscoveryServer _discoveryServer = null;
        AuthenticationServer _authenticationServer = null;
        CommandServer _commandServer = null;
        
        //private FirmwareServer _firmwareServer = null;
        //private MeterServer _meterServer = null;
        //private StatusServer _statusServer = null;
        ILogger _logger = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public SimDeviceCommMgr()
        {
            _logger = LoggerService.GetLogger();
        }

        public SimDeviceCommMgr(IAuthenticatorSimple auth)
            : this()
        {
            _authenticationServer = new AuthenticationServer(auth);
            _discoveryServer = new DiscoveryServer();
            _discoveryServer.APort = _authenticationServer.ReturnOperatingPort;

            _commandServer = new CommandServer();
            _authenticationServer.CommandPort = _commandServer.ReturnOperatingPort;
            //_authenticationServer.FirmwarePort = _firmwareServer.ReturnOperationPort;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public IDSStorageModel DataModelInterface { get; set; }

        public bool UseFakeDiscovery
        {
            get
            {
                return _fakeDiscovery;
            }
            set
            {
                _fakeDiscovery = value;
                if (_fakeDiscovery)
                {
                    ISimDiscoveryData discoveryInterface = this._dataModel as ISimDiscoveryData;
                    if (discoveryInterface != null)
                    {
                        DiscoveryDataDelegate discoveryDelegate = discoveryInterface.GetDiscoveryData;
                        FakeDiscovery.CreateFakeDiscoveryFile(discoveryDelegate((uint)this._authPort));
                    }
                }
                else
                    FakeDiscovery.DeleteFakeDiscoveryFile();
                RaisePropertyChanged(() => UseFakeDiscovery);
            }
        }

        public int DiscoveryServerPort
        {
            get { return _discoveryPort; }
            protected set
            {
                _discoveryPort = value;
                RaisePropertyChanged(() => DiscoveryServerPort);
            }
        }

        public bool AuthenticationServerRunning
        {
            get { return _authServerRunning; }
            protected set
            {
                _authServerRunning = value;
                RaisePropertyChanged(() => AuthenticationServerRunning);
            }
        }

        public int AuthenticationServerPort
        {
            get { return _authPort; }
            protected set
            {
                _authPort = value;
                RaisePropertyChanged(() => AuthenticationServerPort);
            }
        }

        public bool CommandServerRunning
        {
            get { return _commandServerRunning; }
            protected set
            {
                _commandServerRunning = value;
                RaisePropertyChanged(() => CommandServerRunning);
            }
        }

        public bool DiscoveryServerRunning
        {
            get { return _discoveryServerRunning; }
            protected set
            {
                _discoveryServerRunning = value;
                RaisePropertyChanged(() => DiscoveryServerRunning);
            }
        }

        public bool GLinkDiscoveryServerRunning
        {
            get { return _glinkDiscoveryRunning; }
            protected set
            {
                _glinkDiscoveryRunning = value;
                RaisePropertyChanged(() => GLinkDiscoveryServerRunning);
            }
        }

		public IDSStorageModel DataModel {
			get { return _dataModel; }
			set { 
				_dataModel = value;
			}
		}
        #endregion // Properties

        #region Public Methods
        public bool Start()
        {
            return false;
        }

        public void StartGLinkDiscovery()
        {
            _logger.Info("Starting GLink Discovery");

        }

        public void StopGLinkDiscovery()
        {
            _logger.Info("Starting GLink Discovery");
        }

        public bool SetDataModel(IDSStorageModel dataModel)
        {
            //ISimDiscoveryData discoveryInterface = dataModel as ISimDiscoveryData;
            //_discoveryServer.discoveryInterface.GetDiscoveryData);
            _dataModel = dataModel;
            //_dataModel.CommandInterface.OnReceiveCmd += new ReceiveCmd(CommandInterface_OnReceiveCmd);
            return true;
        }

        public void StartAuthenticationServer()
        {
            _logger.Info("Starting GLink Discovery");
            AuthenticationServerRunning = _authenticationServer.Start();
        }

        public void StopAuthenticationServer()
        {
            _logger.Info("Stopping Authentication Server");
            AuthenticationServerRunning = !_authenticationServer.Stop();
        }

        public void StartDiscoveryServer()
        {
            _logger.Info("Starting GLink Discovery");
            DiscoveryServerRunning = _discoveryServer.Start();

            ISimDiscoveryData discoveryInterface = this._dataModel as ISimDiscoveryData;
            if (discoveryInterface != null)
            {
                _discoveryServer.DiscoveryRespDataCall = discoveryInterface.GetDiscoveryData;
                //FakeDiscovery.CreateFakeDiscoveryFile(discoveryDelegate((uint)_authenticationServer.ReturnOperatingPort()));
            }
            else
            {
                _logger.Debug("No device discovery interface. No way to send a good discovery reply");
            }
            //UseFakeDiscovery = true;
        }

        public void StopDiscoveryServer()
        {
            _logger.Info("Stopping Discovery Server");
            DiscoveryServerRunning = !_discoveryServer.Stop();
        }

        public void StartCommandServer()
        {
            _logger.Info("Starting Command Server");
            CommandServerRunning = _commandServer.Start();
        }

        public void StopCommandServer()
        {
            _logger.Info("Stopping Command Server");
            _commandServer.Stop();
            CommandServerRunning = false;
        }
        #endregion // Public Methods

        #region Private Methods


        uint CommandInterface_OnReceiveCmd(IClrOneCommand cmd)
        {
            // send the data model generated command out to whomever is connected.
            return 0;
        }
        #endregion // Private Methods

        #region enums
        #endregion


    }
}
