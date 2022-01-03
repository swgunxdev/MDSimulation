//
// File Name: UDPAsyncServer
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using slf4net;
using System.Net;
using System.Net.Sockets;
using SimulationInterface;

namespace Networking
{
	public delegate byte [] UDPDataOperation(byte [] data);

    public delegate byte[] UDPDataOperationFrom(byte[] data, IPEndPoint from);

    /// <summary>
	/// UDPAsyncServer
	/// </summary>
	public class UDPAsyncServer
	{
		#region Private Variables
		System.Net.Sockets.UdpClient _listener = null;
		//private List<SrvClient> clients = null;
		protected ILogger _logger = null;
		protected int _portNumber = -1;
		protected int DEFAULT_BACKLOG = 4;
		protected IPEndPoint _srvrEndPoint = null;
		protected string _name = "UDPAsyncServer";
		#endregion // Private Variables

		#region Public Events
		#endregion // Public Events

		#region Constructor / Dispose
		/// <summary>
		/// The default constructor
		/// </summary>
		/// <summary>
		/// Private constructor for the common constructor operations.
		/// </summary>
		protected UDPAsyncServer()
		{ 
			_logger = LoggerFactory.GetLogger("debugger");
		}

		public UDPAsyncServer(int listeningPort)
			: this()
		{
			_srvrEndPoint = new IPEndPoint(IPAddress.Any, listeningPort);
			_listener = new System.Net.Sockets.UdpClient(_srvrEndPoint);
		}

        #region IDisposable Members

		/// <summary>
		/// Dispose() calls Dispose(true)
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't
		// own unmanaged resources itself, but leave the other methods
		// exactly as they are.
		~UDPAsyncServer()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
				//if (PerformDataOperation != null) PerformDataOperation = null;
				//if (PerformStringOperation != null) PerformStringOperation = null;
				Stop();
			}
			// free native resources if there are any.
		}


		#endregion IDisposable Members

		#endregion // Constructor / Dispose

		#region Properties
        public UDPDataOperationFrom DataOperation { get; set; }
		public string Name { 
			get { return _name; } 
			protected set{ _name = value; }
		}
		#endregion // Properties

		#region Public Methods
		/// <summary>
		/// Starts the TCP Server listening for new clients.
		/// </summary>
		public bool Start()
		{
			_logger.Debug("Starting up {0} server", Name);

			_listener.BeginReceive(new AsyncCallback(ReceiveCallback), null);
			return true;
		}


		public void ReceiveCallback(IAsyncResult ar)
		{
			IPEndPoint client = new IPEndPoint(IPAddress.Any,0);
			try {

				Byte[] receiveBytes = _listener.EndReceive(ar,ref client);
				_logger.Debug("Recieved {0} bytes from {1}:{2}",receiveBytes.Length, client.Address, client.Port);

				byte [] reply = null;
				if (DataOperation != null)
				{
					reply = DataOperation(receiveBytes, client);
				}
				if (reply != null)
				{
					Write(client, reply);
				} 
			}
			catch (ObjectDisposedException)
			{
				//We expect this after the server has been stopped.
				return;
			}
		}

		private void Write(IPEndPoint iPEndPoint, byte[] reply)
		{
			_listener.BeginSend(reply, reply.Length, iPEndPoint, OnSend, null);
			_logger.Debug("Sent {0} bytes to {1}:{2}",reply.Length,iPEndPoint.Address,iPEndPoint.Port);
		}


		public void OnSend(IAsyncResult ar)
		{
			try
			{
				_listener.EndSend(ar);
				_listener.BeginReceive(new AsyncCallback(ReceiveCallback), null);
				_logger.Debug("Finished sending");
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
			}
		}

		/// <summary>
		/// Stops the TCP Server listening for new clients and disconnects
		/// any currently connected clients.
		/// </summary>
		public bool Stop()
		{
			_logger.Debug("Stopping {0} server", Name);
			this._listener.Close();
			//lock (this.clients)
			//{
			//    foreach (SrvClient client in this.clients)
			//    {
			//        client.TcpClient.Client.Disconnect(false);
			//    }
			//    this.clients.Clear();
			//}
			_logger.Debug("Stopped {0} server", Name);
			return true;
		}

		public virtual int ReturnOperatingPort()
		{
			if (this._listener != null && this._listener.Client.IsBound)
			{
				IPEndPoint ipEnd = this._listener.Client.LocalEndPoint as IPEndPoint;
				if (ipEnd != null)
				{
					return ipEnd.Port;
				}

			} return 0;
		}

        public IPAddress ReturnOperatingIP()
        {
            return IPAddressHelper.GetIP4Address().First();
        }
		#endregion // Public Methods

		#region Private Methods
		#endregion // Private Methods

		#region enums
		#endregion
	}
}
