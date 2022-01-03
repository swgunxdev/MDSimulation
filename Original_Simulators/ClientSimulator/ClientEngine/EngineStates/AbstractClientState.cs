//
// File Name: AbstractClientState
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimpInterfaces.Implementations;

namespace ClientEngine.EngineStates
{
    /// <summary>
    /// AbstractClientState
    /// </summary>
    public abstract class AbstractClientState : AbstractState
    {
        public delegate void NextStateDelegate(IStateContext context);
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public AbstractClientState()
        {
            ToNextState = null;
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public virtual void ToStart(IStateContext context) { }
        public virtual void ToInit(IStateContext context) { }
        public virtual void ToStartServers(IStateContext context) { }
        public virtual void ToRun(IStateContext context) { }
        public virtual void ToStop(IStateContext context) { }
        public virtual void ToStopServers(IStateContext context) { }
        public virtual void ToShutdown(IStateContext context) { }
        public virtual void ToEnd(IStateContext context) { }
        public NextStateDelegate ToNextState;
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
    }
}
