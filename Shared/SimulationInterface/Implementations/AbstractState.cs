//
// File Name: AbstractState
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface.Implementations
{
    public interface IObjSate
    {
        void ChangeState(IStateContext context, IObjSate state);
    }

    public interface IStateContext
    {
        void ChangeState(IObjSate state);
    }


    /// <summary>
    /// AbstractState base class for all state pattern states
    /// </summary>
    public abstract class AbstractState  : IObjSate
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public AbstractState()
        {
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        /// <summary>
        /// Change the state of an object
        /// </summary>
        /// <param name="context">The object to which the state applies</param>
        /// <param name="state">The new state to switch to.</param>
        public void ChangeState(IStateContext context, IObjSate state)
        {
            context.ChangeState(state);
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
