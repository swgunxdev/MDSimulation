//
// File Name: ToStop
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
    /// ToStop
    /// </summary>
    public class StopState : AbstractEngineState
    {
        #region Private Variables
        private static AbstractState _instance = new StopState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public StopState()
        {
            ToNextState = ToStopServers;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public static AbstractState Instance
        {
            get { return _instance; }
        }
        #endregion // Properties

        #region Public Methods

        public override void ToRun(IStateContext context)
        {
            context.ChangeState(RunState.Instance);
        }

        public override void ToStopServers(IStateContext context)
        {
            IThorSimulatorStateMethods simulator = context as IThorSimulatorStateMethods;
            if (simulator != null)
            {
                simulator.StopServers();
            }
            context.ChangeState(StopServersState.Instance);
        }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
