//
// File Name: MainWindowVM
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelSupport;
using ConnectionManager;
using TimpEngine;
using TimpInterfaces;
using Slf;

namespace WindowsTimpsimulator.ViewModels
{
    /// <summary>
    /// MainWindowVM
    /// </summary>
    public class MainWindowVM : ViewModelBase
    {
        #region Private Variables
        private IThorSimulator _simulatorEngine = null;
        private ILogger logger = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public MainWindowVM()
        {
            logger = LoggerService.GetLogger();
            _simulatorEngine = new SimulationEngine();
            _simulatorEngine.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_simulatorEngine_PropertyChanged);
            logger.Info("Starting Simulation Engine");
        }

        void _simulatorEngine_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            logger.Debug("MainWindowVM PropertyChanged {0}", e.PropertyName);
            RaisePropertyChanged(e.PropertyName);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are.
        ~MainWindowVM()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                // free managed resources
                if (this._simulatorEngine != null)
                {
                    _simulatorEngine.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_simulatorEngine_PropertyChanged);
                    _simulatorEngine.Stop();
                }
            }
            // free native resources if there are any.
        }
        #endregion // Constructor / Dispose

        #region Properties
        public string TextData 
        {
            get { return "";  }
        }

        public bool DiscoveryServerRunning 
        {
            get 
            { 
                IThorSimulatorVisual sim = _simulatorEngine as IThorSimulatorVisual;
                if (sim != null)
                {
                    return sim.DiscoveryServerRunning;
                }
                return false;
            }
        }

        public bool AuthenticationServerRunning
        {
            get
            {
                IThorSimulatorVisual sim = _simulatorEngine as IThorSimulatorVisual;
                if (sim != null)
                {
                    return sim.AuthenticationServerRunning;
                }
                return false;
            }
        }
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        public bool CanExecute_StartSimulator()
        {
            return !_simulatorEngine.SimulatorRunning;
        }

        public void Execute_StartSimulator()
        {
            _simulatorEngine.Start();
        }

        public void Execute_StopSimulator()
        {
            _simulatorEngine.Stop();
        }

        public bool CanExecute_StopTCP()
        {
            return _simulatorEngine.SimulatorRunning;
        }
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
