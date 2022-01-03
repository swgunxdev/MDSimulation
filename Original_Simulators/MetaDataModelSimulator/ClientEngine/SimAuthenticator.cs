// ----------------------------------------------------------------------
//       Copyright (C) 4/5/2013 11:31:36 AM ClearOne Communications, Inc. All rights reserved.
//              CONFIDENTIAL AND PROPRIETARY INFORMATION OF
//                  CLEARONE COMMUNICATIONS, INC.
//                    DO NOT DISTRIBUTE
//
// File Name: SimAuthenticator
// ----------------------------------------------------------------------
using Networking.Interfaces;

namespace ClientEngine
{
    /// <summary>
    /// SimAuthenticator
    /// </summary>
    public class SimAuthenticator : IAuthenticatorSimple
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public SimAuthenticator()
        {
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
        
        public bool Authenticate(string userId, string password)
        {
            return true;
        }
    }
}
