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
