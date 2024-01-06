//
// File Name: TCPAsyncServer
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using slf4net;
 
namespace Networking
{
    public delegate byte [] DataOperation(SrvClient client);

    public delegate string StringOpeation(string data);

    /// <summary>
    /// An Asynchronous TCP Server that makes use of system managed threads
    /// and callbacks to stop the server ever locking up.
    /// </summary>
    public class TCPAsyncServer : IDisposable
    {
        private TcpListener tcpListener = null;
        private List<SrvClient> clients = null;
        protected ILogger _logger = null;
        protected int _portNumber = -1;
        protected int DEFAULT_BACKLOG = 4;
        protected IPAddress _localAddress = null;

        public TCPAsyncServer(IPAddress localaddr)
            : this()
        {
            _localAddress = localaddr;
        }

        /// <summary>
        /// Constructor for a new server using an IPAddress and Port
        /// </summary>
        /// <param name="localaddr">The Local IP Address for the server.</param>
        /// <param name="port">The port for the server.</param>
        public TCPAsyncServer(IPAddress localaddr, int port)
            : this(localaddr)
        {
            tcpListener = new TcpListener(localaddr, port);
        }
 
        /// <summary>
        /// Constructor for a new server using an end point
        /// </summary>
        /// <param name="localEP">The local end point for the server.</param>
        public TCPAsyncServer(IPEndPoint localEP)
            : this()
        {
            tcpListener = new TcpListener(localEP);
        }
 
        /// <summary>
        /// Private constructor for the common constructor operations.
        /// </summary>
        protected TCPAsyncServer()
        {
            this.Encoding = Encoding.Default;
            this.clients = new List<SrvClient>();
            _logger = LoggerFactory.GetLogger("debuglogger");
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
        ~TCPAsyncServer()
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
                if (PerformDataOperation != null) PerformDataOperation = null;
                if (PerformStringOperation != null) PerformStringOperation = null;
                Stop();
            }
            // free native resources if there are any.
        }


        #endregion IDisposable Members

 
        /// <summary>
        /// The encoding to use when sending / receiving strings.
        /// </summary>
        public Encoding Encoding { get; set; }

        public DataOperation PerformDataOperation { get; set; }

        public StringOpeation PerformStringOperation { get; set; }

        public virtual int ReturnOperatingPort()
        {
            if (this.tcpListener != null && this.tcpListener.Server.IsBound)
            {
                IPEndPoint ipEnd = this.tcpListener.LocalEndpoint as IPEndPoint;
                if (ipEnd != null)
                {
                    return ipEnd.Port;
                }
                
            } return 0;
        }

        public string Name { get; set; }

        /// <summary>
        /// An enumerable collection of all the currently connected tcp clients
        /// </summary>
        public IEnumerable<TcpClient> TcpClients
        {
            get
            {
                foreach (SrvClient client in this.clients)
                {
                    yield return client.TcpClient;
                }
            }
        }
 
        /// <summary>
        /// Starts the TCP Server listening for new clients.
        /// </summary>
        public bool Start()
        {
            _logger.Debug("Starting up {0} server", Name);
            this.tcpListener.Start(DEFAULT_BACKLOG);

            this.tcpListener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            _logger.Debug("Started {0} server on port {1}", Name, ReturnOperatingPort());
            return true;
        }
 
        /// <summary>
        /// Stops the TCP Server listening for new clients and disconnects
        /// any currently connected clients.
        /// </summary>
        public bool Stop()
        {
            _logger.Debug("Stopping {0} server on port {1}", Name, ReturnOperatingPort());
            this.tcpListener.Stop();
            lock (this.clients)
            {
                foreach (SrvClient client in this.clients)
                {
                    client.TcpClient.Client.Disconnect(false);
                }
                this.clients.Clear();
            }
            _logger.Debug("Stopped {0} server", Name);
            return true;
        }
 
        /// <summary>
        /// Writes a string to a given TCP TCPAsyncClient
        /// </summary>
        /// <param name="tcpClient">The client to write to</param>
        /// <param name="data">The string to send.</param>
        public void Write(TcpClient tcpClient, string data)
        {
            byte[] bytes = this.Encoding.GetBytes(data);
            Write(tcpClient, bytes);
        }
 
        /// <summary>
        /// Writes a string to all clients connected.
        /// </summary>
        /// <param name="data">The string to send.</param>
        public virtual void Write(string data)
        {
            foreach (SrvClient client in this.clients)
            {
                Write(client.TcpClient, data);
            }
        }
 
        /// <summary>
        /// Writes a byte array to all clients connected.
        /// </summary>
        /// <param name="bytes">The bytes to send.</param>
        public virtual void Write(byte[] bytes)
        {
            foreach (SrvClient client in this.clients)
            {
                Write(client.TcpClient, bytes);
            }
        }
 
        /// <summary>
        /// Writes a byte array to a given TCP TCPAsyncClient
        /// </summary>
        /// <param name="tcpClient">The client to write to</param>
        /// <param name="bytes">The bytes to send</param>
        public void Write(TcpClient tcpClient, byte[] bytes)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            StateAsyncInfo asyncInfo = new StateAsyncInfo(networkStream);
            networkStream.BeginWrite(bytes, 0, bytes.Length, WriteCallback, asyncInfo);
        }
 
        /// <summary>
        /// Callback for the write operation.
        /// </summary>
        /// <param name="result">The async result object</param>
        protected virtual void WriteCallback(IAsyncResult result)
        {
            StateAsyncInfo asyncInfo = result.AsyncState as StateAsyncInfo;
            NetworkStream networkStream = asyncInfo.NetStream;
            networkStream.EndWrite(result);
        }
 
        /// <summary>
        /// Callback for the accept tcp client operation.
        /// </summary>
        /// <param name="result">The async result object</param>
        protected virtual void AcceptTcpClientCallback(IAsyncResult result)
        {
            try
            {
                // get new client socket to listen to send and receive data
                TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);

                SrvClient client = new SrvClient(tcpClient.ReceiveBufferSize, tcpClient);
                _logger.Debug("Accepted new client from {0} on port {1}", client.ConnectionAddress, client.ConnectionPort);

                lock (this.clients)
                {
                    this.clients.Add(client);
                }
                NetworkStream networkStream = client.NetworkStream;
                networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
                // go back to listening for connections
                tcpListener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }
            catch (ObjectDisposedException)
            {
                //We expect this after the server has been stopped.
                return;
            }
        }
 
        /// <summary>
        /// Callback for the read operation.
        /// </summary>
        /// <param name="result">The async result object</param>
        protected virtual void ReadCallback(IAsyncResult result)
        {
            SrvClient client = result.AsyncState as SrvClient;
            if (client == null) return;
            NetworkStream networkStream = client.NetworkStream;
            int read = 0;
            
            try 
            { 
                read = networkStream.EndRead(result);
            }
            catch
            {
                read = 0;
            }
            
            if (read == 0)
            {
                DisconnectClient(client);
                return;
            }
            if (this.Encoding == Encoding.Default)
            {
                _logger.Debug("Client read {0} bytes", read);
                client.AddToCompletedData(read);
                // is more data available?
                if (networkStream.DataAvailable)
                {
                    networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
                    return;
                }
                if (PerformDataOperation != null)
                {
                    _logger.Debug("Client at {0} sent data to be processed", client.ConnectionAddress);
                    byte[] reply = PerformDataOperation(client);
                    if (reply == null)
                    {
                        DisconnectClient(client);
                        return;
                    }
                    else
                    {
                        Write(client.TcpClient, reply);
                    }
                }
            }
            else
            {
                string data = this.Encoding.GetString(client.Buffer, 0, read);
                if (PerformStringOperation != null)
                {
                    //if (PerformStringOperation(data) < 0)
                    //{
                    //    DisconnectClient(client);
                    //    return;
                    //}
                }
            }
            networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
        }

        protected virtual void DisconnectClient(SrvClient client)
        {
            lock (this.clients)
            {
                this.clients.Remove(client);
            }
			_logger.Debug("Disconnected from client {0}",client.ConnectionAddress);
		}
    }
 
    /// <summary>
    /// Internal class to join the TCP client and buffer together 
    /// for easy management in the server
    /// </summary>
    public class SrvClient
    {
        byte[] _completed = null;
        byte[] _readBuffer = null;

        public SrvClient()
            : this(8192)
        {
        }

        public SrvClient(int buffersize)
        {
            _readBuffer = new byte [buffersize];
        }

/// <summary>
        /// Constructor for a new TCPAsyncClient
        /// </summary>
        /// <param name="tcpClient">The TCP client</param>
        /// <param name="buffer">The byte array buffer</param>
        public SrvClient( int bufferSize, TcpClient tcpClient)
            : this(bufferSize)
        {
            if (tcpClient == null) throw new ArgumentNullException("tcpClient");
            this.TcpClient = tcpClient;
        }

        public Guid WriteGuid { get; set; }
        public Byte[] Buffer
        {
            get
            {
                return _readBuffer;
            }
        }
        public byte[] CompletedData
        {
            get
            {
                return _completed;
            }
        }

        public string ConnectionAddress { get { return TcpClient.Client.LocalEndPoint.ToString(); } }

        public int ConnectionPort
        {
            get
            {
                IPEndPoint ipEndPoint = TcpClient.Client.LocalEndPoint as IPEndPoint;
                if (ipEndPoint == null) throw new InvalidCastException("Attempting to cast EndPoint as IPEndPoint that failed");

                return ipEndPoint.Port;
            }
        }
        /// <summary>
        /// Gets the TCP TCPAsyncClient
        /// </summary>
        public TcpClient TcpClient { get; private set; }
 


        /// <summary>
        /// Gets the network stream
        /// </summary>
        public NetworkStream NetworkStream { get { return TcpClient.GetStream(); } }
            
        public void AddToCompletedData(int bytesToCopy)
        {
            if (_completed == null)
            {
                _completed = new byte[bytesToCopy];
                Array.Copy(_readBuffer, _completed, bytesToCopy);
            }
            else
            {
                byte[] newcompleted = new byte[bytesToCopy + _completed.Length];
                Array.Copy(_completed, newcompleted, _completed.Length);
                Array.Copy(_readBuffer, 0, newcompleted, _completed.Length, bytesToCopy);
                _completed = newcompleted;
            }
        }

    }
}

