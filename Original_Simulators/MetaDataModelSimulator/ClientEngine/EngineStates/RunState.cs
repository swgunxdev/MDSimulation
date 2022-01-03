// ----------------------------------------------------------------------
//       Copyright (C) 4/10/2013 4:06:08 PM ClearOne Communications, Inc. All rights reserved.
//              CONFIDENTIAL AND PROPRIETARY INFORMATION OF
//                  CLEARONE COMMUNICATIONS, INC.
//                    DO NOT DISTRIBUTE
//
// File Name: RunState
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
    /// RunState
    /// </summary>
    public class RunState : AbstractEngineState
    {
        #region Private Variables
        private static AbstractState _instance = new RunState();
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public RunState()
        {
            ToNextState = ToStop;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public static AbstractState Instance
        {
            get { return _instance; }
        }
        #endregion // Properties

        #region Public Methods
        public override void ToStop(IStateContext context)
        {
            IThorSimulatorStateMethods simulator = context as IThorSimulatorStateMethods;
            if (simulator != null)
            {
                simulator.Stop();
            }
            context.ChangeState(StopState.Instance);
        }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
