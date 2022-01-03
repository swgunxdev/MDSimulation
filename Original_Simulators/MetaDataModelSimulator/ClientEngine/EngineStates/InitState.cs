//
// File Name: ToInit
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
    /// ToInit
    /// </summary>
    public class InitState : AbstractEngineState
    {
        #region Private Variables
        private static AbstractState _instance = new InitState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public InitState()
        {
            ToNextState = ToStartServers;
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
            IThorSimulatorStateMethods simulator = context as IThorSimulatorStateMethods;
            if (simulator != null)
            {
                simulator.StartServers();
            }
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

        public override void ToEnd(IStateContext context)
        {
            // bad
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
