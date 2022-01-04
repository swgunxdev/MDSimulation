using System;
using ClientEngine.EngineStates;
using Slf;
using TimpInterfaces;
using TimpInterfaces.Implementations;
using Networking.Interfaces;

namespace ClientEngine
{
    public class SimulatorEngine : IThorSimulator
    {
        #region Private Variables
        AbstractEngineState _state = null;
        IAuthenticatorSimple authenticator = null;
        IDSStorageModel _model = null;
        IDeviceCommMgr _commMgr;
        ILogger _logger = null;
        bool _engineActive = false;
//        SimulationTaskMgr taskMgr = null;
        private bool _shutdownRequested = false;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public SimulatorEngine()
        {
            _logger = LoggerService.GetLogger();
            _state = StartState.Instance as AbstractEngineState;
            _state.ChangeState(this, StartState.Instance);
        }
        #endregion // Constructor / Dispose

        #region Properties
        /// <summary>
        /// Gets or sets the state 
        /// </summary>
        public AbstractState State
        {
            get { return _state; }
            set
            {
                if (value != null)
                {_logger.Debug("Transitioning from {0} to {1}", _state.GetType().Name, value.GetType().Name);
                    _state = value as AbstractEngineState;
                }
            }
        }
        #endregion // Properties

        #region Public Methods
 

        //#region ITaskCommandReciever

        //public long SetProperty(IObjAddress objAddress, uint propertyId, object value)
        //{
        //    throw new NotImplementedException();
        //}

        //public long CompareValue(IObjAddress objAddress, uint propertyId, object value)
        //{
        //    throw new NotImplementedException();
        //}

        public long SendCommand(int id, IClrOneCommand cmd)
        {
            _logger.Debug("Sending Command {0} to connection id {1}", cmd.ToString(), id);
            return 0;
        }

        public long ReadModelConfiguration(int id, string fileName)
        {
            throw new NotImplementedException();
        }

        public long SendFirmwareUpdate(int id, string fileName)
        {
            throw new NotImplementedException();
        }

        public long DownloadLogFileTo(int id, string fileName)
        {
            throw new NotImplementedException();
        }

        public long ClearLogFiles(int id)
        {
            throw new NotImplementedException();
        }

        public long PrintMessage(int id, string msg)
        {
            _logger.Info(msg);
            // HACK This is done to demonstrate a command working 
            //CCommand command = new CCommand();
           // command.CommandID = 
            return 0;
        }

        public long OpenModel(string filename)
        {
            throw new NotImplementedException();
        }

        public long CloseModel()
        {
            throw new NotImplementedException();
        }

        #endregion // ITaskCommandReciever

        #region IThorSimulatorStateMethods
        public void NextState()
        {
            _state.ToNextState(this);
        }

        public void Run()
        {
                // check for new commands
                // check for new glink connections
                // etc.....
        }

        public void Start()
        {
            ToInit();
        }

        public void Init()
        {
            _model = new SimulationModel();
            _commMgr = new SimDeviceCommMgr(new SimAuthenticator());//new NullDeviceCommMgr();
			_commMgr.DataModel = _model;
            ToStartServers();
        }

        public void StartServers()
        {
            _commMgr.StartGLinkDiscovery();
            _commMgr.StartCommandServer();
            _commMgr.StartAuthenticationServer();
            _commMgr.StartDiscoveryServer();

            // switch to the run state
            ToRun();
        }

        public void StopServers()
        {
            _commMgr.StopDiscoveryServer();
            _commMgr.StopAuthenticationServer();
            _commMgr.StopCommandServer();
            _commMgr.StopGLinkDiscovery();

            // switch to the shutdown state
            ToShutdown();
        }

        public void Shutdown()
        {
            _model = null;
            _commMgr = null;

            ToEnd();
        }

        public void Stop()
        {
            // stop working for a moment
            this._shutdownRequested = true;

            ToStopServers();
        }

        public void End()
        {
        }
        
        private void ToStart()
        {
            _state.ToStart(this);
        }

        private void ToInit()
        {
            _state.ToInit(this);
        }

        private void ToStartServers()
        {
            _state.ToStartServers(this);
        }

        private void ToRun()
        {
            _state.ToRun(this);
        }

        public void ToStop()
        {
            _state.ToStop(this);
        }

        private void ToStopServers()
        {
            _state.ToStopServers(this);
        }

        private void ToShutdown()
        {
            _state.ToShutdown(this);
        }

        private void ToEnd()
        {
            _state.ToEnd(this);
        }

        public void ChangeState(IObjSate state)
        {
            _logger.Debug("Changing state from {0} to {1}", this._state.GetType().Name, state.GetType().Name);
            this.State = state as AbstractState;
        }
//        #endregion // IThorSimulatorStateMethods
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

        public bool SimulatorRunning
        {
            get { throw new NotImplementedException(); }
        }

        public int LoadModel(string modelFileName)
        {
            throw new NotImplementedException();
        }

        public int UnloadModel()
        {
            throw new NotImplementedException();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
