//
// File Name: CommandClient
// ----------------------------------------------------------------------
using System;
using System.Net;
using Networking;
using TimpInterfaces;

namespace ClientEngine.NetClients
{
    /// <summary>
    /// CommandClient
    /// </summary>
    public class CommandClient : TCPAsyncClient
    {
        #region Private Variables
        //BlockingCollectionWrapper<IClrOneCommand> _cmdSendQueue = null;
        //private BlockingCollectionWrapper<IClrOneCommand> _cmdRecvQueue = null;
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public CommandClient(IPAddress address, int port)
            : base(address,port)
        {
            //this._cmdSendQueue = new BlockingCollectionWrapper<IClrOneCommand>();
            //this._cmdRecvQueue = new BlockingCollectionWrapper<IClrOneCommand>();

            //// set up queue consumer methods
            //this._cmdSendQueue.QueueConsumingAction = SendAction;
            //this._cmdRecvQueue.QueueConsumingAction = RecvAction;

            this.Connected += new EventHandler(CommandClient_Connected);
            this.Disconnected += new EventHandler(CommandClient_Disconnected);
        }

        void CommandClient_Disconnected(object sender, EventArgs e)
        {
            //this._cmdSendQueue.CompleteAdding();
            //this._cmdRecvQueue.CompleteAdding();
            this.Connected -= new EventHandler(CommandClient_Connected);
            this.Disconnected -= new EventHandler(CommandClient_Disconnected);
            _clientLogger.Debug("oops disconnected");
        }

        void CommandClient_Connected(object sender, EventArgs e)
        {
            // start processing of queues
            //this._cmdSendQueue.Start();
            //this._cmdRecvQueue.Start();
        }
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        #endregion // Public Methods

        #region Private Methods
        private void SendAction(IClrOneCommand cmd)
        {
            // do nothing for now
            _clientLogger.Debug("Sending Command");
        }

        private void RecvAction(IClrOneCommand cmd)
        {
            // do nothing for now.
            _clientLogger.Debug("Received Command");
        }
        #endregion // Private Methods

        #region enums
        #endregion

    }
}
