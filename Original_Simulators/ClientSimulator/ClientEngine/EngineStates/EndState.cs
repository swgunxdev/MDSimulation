//
// File Name: ToEnd
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimpInterfaces.Implementations;

namespace ClientEngine.EngineStates
{
    /// <summary>
    /// ToEnd
    /// </summary>
    public class EndState : AbstractClientState
    {
        #region Private Variables
        private static AbstractState _instance = new EndState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public EndState()
        {
            ToNextState = ToStart;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public static AbstractState Instance
        {
            get { return _instance; }
        }
        #endregion // Properties

        #region Public Methods
        public override void ToStart(IStateContext context)
        {
            context.ChangeState(StartState.Instance);
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
    }
}
