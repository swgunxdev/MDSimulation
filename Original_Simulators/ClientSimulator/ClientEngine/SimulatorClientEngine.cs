using System;
using System.Threading;
using DistributedCommand.Framework;
using ClientEngine.EngineStates;
using SimTaskMgr;
using Slf;
using TimpInterfaces;
using TimpInterfaces.Implementations;
using Networking.Interfaces;

namespace ClientEngine
{
    public class SimulatorClientEngine : IThorSimulatorStateMethods, ITaskCommandReciever, ICommandExecutor
    {
        #region Private Variables
        AbstractClientState _state = null;
        IDSStorageModel _model = null;
        IClientCommsMgr _commMgr;
        ILogger _logger = null;
        SimulationTaskMgr taskMgr = null;
        Thread _workerThread = null;

        // This is the great command bus 
        private CommandBus _MyBus = null;

        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public SimulatorClientEngine()
        {
            ILogger[] loggers = new ILogger[] { LoggerService.GetLogger("debuglogger"), LoggerService.GetLogger() };
            _logger = new Slf.CompositeLogger(loggers);
            _state = StartState.Instance as AbstractClientState;
            _state.ChangeState(this, StartState.Instance);
            // 1. Create the command bus and attach myself as one of the command executors
            this._MyBus = new CommandBus(true);
            this._MyBus.AddExecutor(this);
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
                    _state = value as AbstractClientState;
                }
            }
        }
        #endregion // Properties

        #region Public Methods
 

        #region ITaskCommandReciever

        /// <summary>
        /// Connect to a network device identified by id
        /// </summary>
        /// <param name="id">network device id</param>
        /// <param name="userid">user id used to authenticate with</param>
        /// <param name="password">password used to authenticate with.</param>
        /// <returns>returns an error code. Zero === success.</returns>
        public long Connect(int id, string userid, string password)
        {
            if (_commMgr.Connect(id, userid, password))
                return 1;
            return 0;
        }

        public long Disconnect(int id)
        {
            if (_commMgr.Disconnect(id))
                return 1;
            return 0;
        }

        public long SetProperty(IObjAddress objAddress, uint propertyId, object value)
        {
            throw new NotImplementedException();
        }

        public long CompareValue(IObjAddress objAddress, uint propertyId, object value)
        {
            throw new NotImplementedException();
        }

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

        public long NewModel(string modelName)
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
            _workerThread = new Thread(RunStateWorkerThread);
            _workerThread.Start();
        }

        private void RunStateWorkerThread()
        {
            // setup timer to wait for 30 seconds
            DateTime startTime = DateTime.Now;
            int timeout = 100000;
            int counter = 0;
            TimeSpan delta = DateTime.Now - startTime;

            while (counter < timeout)
            {
		if(_commMgr == null || taskMgr == null)
		{
			break;
		}

                if (this.taskMgr.Count > 0 && _commMgr.DeviceList.Count > 0)
                    break;

                delta = DateTime.Now - startTime;
                counter += delta.Seconds;
                System.Threading.Thread.Sleep(500);
            }
	    if(_commMgr != null)
            {
		// for each of the connected devices
            	// do something.
            	foreach (int id in _commMgr.DeviceList.Keys)
            	{
                	ProcessCommands(id);
            	}
	    }
            ToStop();
        }

        public void Start()
        {
            ToInit();
        }

        public void Init()
        {
            _model = new SimulationModel();
            _commMgr = new ClientCommMgr();
            taskMgr = new SimulationTaskMgr(this);
            taskMgr.CollectCommands();

            ToStartServers();
        }

        public void StartServers()
        {
            _commMgr.StartDiscovery();
            _commMgr.OnVDeviceArrival += new VDeviceArrival(_commMgr_OnVDeviceArrival);
            _commMgr.OnVDeviceLeaving += new VDeviceLeave(_commMgr_OnVDeviceLeaving);
            ToRun();
        }

        void _commMgr_OnVDeviceLeaving(ulong id)
        {
            // remove client from purpose and clean up.
            this._logger.Debug("Device leaving on connection {0} from engine", id);
        }

        void _commMgr_OnVDeviceArrival(ulong id)
        {
            // schedule the device to be used for whatever purpose
            // of this client is.
            this._logger.Debug("Device arrival on connection id {0} from engine", id);
        }

        public void StopServers()
        {
            _commMgr.OnVDeviceArrival -= new VDeviceArrival(_commMgr_OnVDeviceArrival);
            _commMgr.OnVDeviceLeaving -= new VDeviceLeave(_commMgr_OnVDeviceLeaving);
            _commMgr.StopDiscovery();
            _commMgr.StopAll();
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
			//_workerThread.Join();
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
        #endregion // IThorSimulatorStateMethods
        #endregion // Public Methods

        #region Private Methods
        private void ProcessCommands(int id)
        {
            foreach (ITaskCommand cmd in taskMgr.Commands)
            {
                CommunicationCmd commCmd = cmd as CommunicationCmd;
                if (commCmd != null)
                {
                    commCmd.ConnId = id;
                }
                commCmd.Execute();
            }
        }
        /// <summary>
        /// Perform the operation in UI thread. This is needed, because the 
        /// command is created in the thread of the transporter. Normally 
        /// transporters are multi-threaded, one thread for listenining from socket 
        /// without blocking the main thread. So, then a command is received, 
        /// the command object is created on the transporter's thread, and this 
        /// method is also called from transporters thread. This is why the command 
        /// needs to be executed on the UI thread, as UI operations can only be 
        /// performed from the UI thread.
        /// </summary>
        /// <param name="command">The command object to execute</param>
        /// <param name="source">The transporter ID which received this command</param>
        private void ExecuteCommand(ICommand command)
        {
            ICommand actualCommand = command;

            // 1. If this is a remote command, then the actual command is packed inside
            if (command is RemoteCommand)
            {
                RemoteCommand remoteCommand = command as RemoteCommand;
                actualCommand = remoteCommand.ActualCommand;
            }

            //// 2. Check if the command is one type of UI command which 
            //// requires some UI elements
            //UICommand uiCommand = actualCommand as UICommand;

            //if (null != uiCommand)
            //{
            //    // The command is a UI command, so give it the UI elements
            //    uiCommand.UI = this.MakeUIElements();

            //    // Also pass the transporter ID of the remote command, some
            //    // commands need to know which transporter is associated with
            //    // the command, for example, NewClient command. It also needs
            //    // to remember which transporter is associated with this new client
            //    // so that when a transporter is closed, it can identity the
            //    // client which was associated with the transporter
            //    if (command is RemoteCommand)
            //        uiCommand.TransporterID = (command as RemoteCommand).Source;
            //}

            //// Call the execute method on UI thread
            //if (this.InvokeRequired)
            //    this.Invoke(new MethodInvoker(command.Execute));
            //else
                command.Execute();
        }
        #endregion // Private Methods
        #region enums
        #endregion

        public void Execute(ICommand command)
        {
            this.ExecuteCommand(command);
        }

        public void AddListener(ICommandExecutorListener callback)
        {
            // No need to preserver the reference to the command bus, I already
            // have it
        }

        #region ICommandExecutor Members

        /// <summary>
        /// Executes each command in local context. Basically this command
        /// provides each command object with required UI components and
        /// the command objects do the rest.
        /// 
        /// So, when a command is received from a remote client, the
        /// command is given with local UI components. The commands
        /// execute their operation on local UI component.
        /// </summary>
        /// <param name="command"></param>
        void ICommandExecutor.Execute(ICommand command)
        {
            this.ExecuteCommand(command);
        }

        void ICommandExecutor.AddListener(ICommandExecutorListener callback)
        {
            // No need to preserver the reference to the command bus, I already
            // have it
        }

        #endregion


    }
}
