//
// File Name: ToShutdown
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimpInterfaces.Implementations;

namespace ClientEngine.EngineStates
{
    /// <summary>
    /// ToShutdown
    /// </summary>
    public class ShutdownState : AbstractEngineState
    {
        #region Private Variables
        private static AbstractState _instance = new ShutdownState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ShutdownState()
        {
            ToNextState = ToEnd;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public static AbstractState Instance
        {
            get { return _instance; }
        }
        #endregion // Properties

        #region Public Methods
        public override void ToInit(IStateContext context)
        {
            context.ChangeState(InitState.Instance);
        }
        public override void ToEnd(IStateContext context)
        {
            context.ChangeState(EndState.Instance);
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
