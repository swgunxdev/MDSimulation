using slf4net;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public class UDPAsyncClient
    {
		#region Private Variables
		UdpClient _client = null;
		protected ILogger _logger = null;
		protected int _portNumber = -1;
		protected IPEndPoint _clientEndPoint = null;
		protected string _name = "UDPAsyncClient";
        bool _stopped = false;
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
		protected UDPAsyncClient()
		{
			_logger = LoggerFactory.GetLogger("debuglogger");
		}

		public UDPAsyncClient(int transmitPort)
			: this()
		{
            _portNumber = transmitPort;
            //_clientEndPoint = new IPEndPoint(IPAddress.Broadcast, transmitPort);
            _client = new System.Net.Sockets.UdpClient();
            _client.EnableBroadcast = true;
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
        ~UDPAsyncClient()
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
				if (DataOperation != null) DataOperation = null;
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
		public virtual bool Start()
		{
			_logger.Debug("Starting up {0} server", Name);
			return true;
		}


		public void ReceiveCallback(IAsyncResult ar)
		{
			IPEndPoint client = new IPEndPoint(IPAddress.Broadcast,_portNumber);
			try {

				Byte[] receiveBytes = _client.EndReceive(ar,ref client);
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
			catch (ObjectDisposedException ex)
			{
                if (_stopped == false)
                {
                    _logger.Error(ex.ToString());
                }
				//We expect this after the server has been stopped.
				return;
			}
            catch(SocketException sockEx)
            {
                _logger.Error(sockEx.ToString());
            }
		}

        public void Write(byte [] msg)
        {
            //foreach (IPAddress addr in IPAddressHelper.GetIP4Address())
            //{
            //    string[] addrSplit = addr.ToString().Split(new char[] { '.' });
            //    IPEndPoint sendEP = new IPEndPoint(IPAddress.Parse(string.Format("{0}.255.255.255", addrSplit[0])), _portNumber);
            //    //IPEndPoint sendEP = new IPEndPoint(sendEP, _portNumber);

            //    Write(sendEP, msg);
            //}
            IPEndPoint sendEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _portNumber);
            Write(sendEP, msg);
        }

		public void OnSend(IAsyncResult ar)
		{
			try
			{
				_client.EndSend(ar);
				_client.BeginReceive(new AsyncCallback(ReceiveCallback), _client);
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
		public virtual bool Stop()
		{
			_logger.Debug("Stopping {0} server", Name);
			this._client.Close();
			_logger.Debug("Stopped {0} server", Name);
			return true;
		}

		public virtual int ReturnOperatingPort()
		{
            if (this._client != null && this._client.Client.IsBound)
			{
                IPEndPoint ipEnd = this._client.Client.LocalEndPoint as IPEndPoint;
				if (ipEnd != null)
				{
					return ipEnd.Port;
				}

			} return 0;
		}
		#endregion // Public Methods

		#region Private Methods
        private void Write(IPEndPoint iPEndPoint, byte[] msg)
        {
            try
            {
                _client.BeginSend(msg, msg.Length, iPEndPoint, OnSend, _client);
            }
            catch (SocketException sockEx)
            {
                _logger.Debug("{0}", sockEx.ToString());
            }
            _logger.Debug("Sent {0} bytes to {1}:{2}", msg.Length, iPEndPoint.Address, iPEndPoint.Port);
        }

        #endregion // Private Methods

		#region enums
		#endregion
    }
}
