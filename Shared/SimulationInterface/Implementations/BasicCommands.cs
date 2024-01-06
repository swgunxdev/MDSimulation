//
// File Name: BasicCommands.cs
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface.Implementations
{

    public class BaseCmd : ITaskCommand
    {
        protected ITaskCommandReciever reciever = null;

        public BaseCmd(ITaskCommandReciever rcvr)
        {
            reciever = rcvr;
        }

        public virtual long Execute()
        {
            return -1;
        }
    }

    public class CommunicationCmd : BaseCmd
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public CommunicationCmd(ITaskCommandReciever rcvr)
            : base(rcvr)
        {
        }
        #endregion // Constructor / Dispose

        #region Properties
        public int ConnId { get; set; }

        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods
        #region enums
        #endregion
        
    }
    /// <summary>
    /// Class1
    /// </summary>
    public class ConnectCmd : CommunicationCmd
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public ConnectCmd(ITaskCommandReciever rcvr, string userId, string password)
            : base(rcvr)
        {
            UserId = userId;
            Password = password;
        }
        #endregion // Constructor / Dispose

        #region Properties
        public string UserId { get; protected set; }
        public string Password { get; protected set; }
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion


        public override long Execute()
        {
            return reciever.Connect(ConnId, UserId, Password);
        }
    }
    /// <summary>
    /// Class1
    /// </summary>
    public class DisconnectCmd : CommunicationCmd
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public DisconnectCmd(ITaskCommandReciever rcvr)
            : base(rcvr)
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


        public override long Execute()
        {
            return reciever.Disconnect(ConnId);
        }
    }

    public class PrintCmd : CommunicationCmd
    {
        #region Private Variables
        string _msg;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public PrintCmd(ITaskCommandReciever rcvr, string msg)
            : base(rcvr)
        {
            _msg = msg;
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public override long Execute()
        {
            return reciever.PrintMessage(this.ConnId,_msg);
        }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
        
    }
}
