// ----------------------------------------------------------------------
//       Copyright (C) 4/22/2013 1:22:06 PM ClearOne Communications, Inc. All rights reserved.
//              CONFIDENTIAL AND PROPRIETARY INFORMATION OF
//                  CLEARONE COMMUNICATIONS, INC.
//                    DO NOT DISTRIBUTE
//
// File Name: FirmwareServer
// ----------------------------------------------------------------------
using System.Net;
using Networking;

namespace ClientEngine.NetServers
{
    /// <summary>
    /// FirmwareServer
    /// </summary>
    public class FirmwareServer : TCPAsyncServer
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public FirmwareServer()
            : base(new IPEndPoint(IPAddress.Any, 0))
        {
            PerformDataOperation = ProcessFirmwareUpdate;
            Name = "Firmware Server";
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        private byte [] ProcessFirmwareUpdate(SrvClient client)
        {
            byte[] reply = null;
            return reply;
        }
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
