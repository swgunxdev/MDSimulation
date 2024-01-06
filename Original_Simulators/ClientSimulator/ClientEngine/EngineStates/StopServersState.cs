//
// File Name: ToStopServers
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimpInterfaces.Implementations;
using TimpInterfaces;

namespace ClientEngine.EngineStates
{
    /// <summary>
    /// ToStopServers
    /// </summary>
    public class StopServersState : AbstractClientState
    {
        #region Private Variables
        private static AbstractState _instance = new StopServersState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public StopServersState()
        {
            ToNextState = ToShutdown;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public static AbstractState Instance
        {
            get { return _instance; }
        }
        #endregion // Properties

        #region Public Methods
        public override void ToStartServers(IStateContext context)
        {
            context.ChangeState(StartServersState.Instance);
        }
        public override void ToShutdown(IStateContext context)
        {
            IThorSimulatorStateMethods simulator = context as IThorSimulatorStateMethods;
            if (simulator != null)
            {
                simulator.Shutdown();
            }
            context.ChangeState(ShutdownState.Instance);
        }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
