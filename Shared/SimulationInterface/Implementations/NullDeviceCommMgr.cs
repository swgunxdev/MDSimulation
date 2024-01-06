//
// File Name: NullDeviceCommMgr
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface.Implementations
{
    /// <summary>
    /// NullDeviceCommMgr
    /// </summary>
    public class NullDeviceCommMgr : IDeviceCommMgr
    {
        #region Private Variables
        bool _authServerRunning = false;
        bool _cmdServerRunning = false;
        bool _glinkServerRunning = false;
        bool _discServerRunning = false;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public NullDeviceCommMgr()
        {
        }
        #endregion // Constructor / Dispose

        #region Properties
        public bool AuthenticationServerRunning
        {
            get { return _authServerRunning; }
        }

        public bool DiscoveryServerRunning
        {
            get { return _discServerRunning; }
        }

        public bool CommandServerRunning
        {
            get { return _cmdServerRunning; }
        }

        public bool GLinkDiscoveryServerRunning
        {
            get { return _glinkServerRunning; }
        }
        #endregion // Properties

        #region Public Methods
        public void StartAuthenticationServer()
        {
            this._authServerRunning = true;
        }

        public void StopAuthenticationServer()
        {
            this._authServerRunning = true;
        }

        public void StartDiscoveryServer()
        {
            this._authServerRunning = true;
        }

        public void StopDiscoveryServer()
        {
            this._authServerRunning = true;
        }

        public void StartGLinkDiscovery()
        {
            this._authServerRunning = true;
        }

        public void StartCommandServer()
        {
            this._authServerRunning = true;
        }

        public void StopCommandServer()
        {
            this._authServerRunning = true;
        }

        public void StopGLinkDiscovery()
        {
            this._authServerRunning = true;
        }

		public IDSStorageModel DataModel { get; set; }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
