//
// File Name: TCPAsyncClient
// ----------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using System.Text;

using System;
using slf4net;
using System.IO;
 

namespace Networking
{
       /// <summary>
    /// Represents an asynchronous TCP client.
    /// </summary>
    public class TCPAsyncClient : IDisposable
    {
        /// <summary>
        /// The default length for the read buffer.
        /// </summary>
        protected const int DefaultClientReadBufferLength = 4096;
 
        /// <summary>
        /// The tcp client used for the outgoing connection.
        /// </summary>
        protected readonly TcpClient client;
 
        /// <summary>
        /// The port to connect to on the remote server.
        /// </summary>
        protected readonly int port;
 
        /// <summary>
        /// A reset event for use if a DNS lookup is required.
        /// </summary>
        private readonly ManualResetEvent dnsGetHostAddressesResetEvent = null;
 
        /// <summary>
        /// The length of the read buffer.
        /// </summary>
        private readonly int clientReadBufferLength;
 
        /// <summary>
        /// The addresses to try connection to.
        /// </summary>
        protected IPAddress[] addresses;
 
        protected ILogger _clientLogger = null;

        /// <summary>
        /// How many times to retry connection.
        /// </summary>
        private int retries;
         
        public int Retries
        {
            get { return this.retries; }
            set { this.retries = value; }
        }

        /// <summary>
        /// Occurs when the client connects to the server.
        /// </summary>
        public event EventHandler Connected;
 
        /// <summary>
        /// Occurs when the client disconnects from the server.
        /// </summary>
        public event EventHandler Disconnected;
 
        /// <summary>
        /// Occurs when data is read by the client.
        /// </summary>
        public event EventHandler<DataReadEventArgs> DataRead;
 
        /// <summary>
        /// Occurs when data is written by the client.
        /// </summary>
        public event EventHandler<DataWrittenEventArgs> DataWritten;
 
        /// <summary>
        /// Occurs when an exception is thrown during connection.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ClientConnectException;
 
        /// <summary>
        /// Occurs when an exception is thrown while reading data.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ClientReadException;
 
        /// <summary>
        /// Occurs when an exception is thrown while writing data.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ClientWriteException;
 
        /// <summary>
        /// Occurs when an exception is thrown while performing the DNS lookup.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> DnsGetHostAddressesException;
 
        /// <summary>
        /// Constructor for a new client object based on a host name or server address string and a port.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address of the server as a string.</param>
        /// <param name="port">The port on the server to connect to.</param>
        /// <param name="clientReadBufferLength">The clients read buffer length.</param>
        public TCPAsyncClient(string hostNameOrAddress, int port, int clientReadBufferLength = DefaultClientReadBufferLength)
            : this(port, clientReadBufferLength)
        {
            this.dnsGetHostAddressesResetEvent = new ManualResetEvent(false);
            Dns.BeginGetHostAddresses(hostNameOrAddress, this.DnsGetHostAddressesCallback, null);
        }
 
        /// <summary>
        /// Constructor for a new client object based on a number of IP Addresses and a port.
        /// </summary>
        /// <param name="addresses">The IP Addresses to try connecting to.</param>
        /// <param name="port">The port on the server to connect to.</param>
        /// <param name="clientReadBufferLength">The clients read buffer length.</param>
        public TCPAsyncClient(IPAddress[] addresses, int port, int clientReadBufferLength = DefaultClientReadBufferLength)
            : this(port, clientReadBufferLength)
        {
            this.addresses = addresses;
        }
 
        /// <summary>
        /// Constructor for a new client object based on a single IP Address and a port.
        /// </summary>
        /// <param name="address">The IP Address to try connecting to.</param>
        /// <param name="port">The port on the server to connect to.</param>
        /// <param name="clientReadBufferLength">The clients read buffer length.</param>
        public TCPAsyncClient(IPAddress address, int port, int clientReadBufferLength = DefaultClientReadBufferLength)
            : this(new[] {address}, port, clientReadBufferLength)
        {
        }
 
        /// <summary>
        /// Private constructor for a new client object.
        /// </summary>
        /// <param name="port">The port on the server to connect to.</param>
        /// <param name="clientReadBufferLength">The clients read buffer length.</param>
        private TCPAsyncClient(int port, int clientReadBufferLength)
        {
            this.client = new TcpClient();
            this.port = port;
            this.clientReadBufferLength = clientReadBufferLength;
            _clientLogger = LoggerFactory.GetLogger("debuglogger");
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
        ~TCPAsyncClient()
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
                this.client.Client.Disconnect(false);
            }
            // free native resources if there are any.
        }
        #endregion IDisposable Members

        /// <summary>
        /// Starts an asynchronous connection to the remote server.
        /// </summary>
        public void Connect()
        {
            if (this.dnsGetHostAddressesResetEvent != null)
                this.dnsGetHostAddressesResetEvent.WaitOne();
            this.retries = 0;
            this.client.BeginConnect(this.addresses[0], this.port, this.ClientConnectCallback, null);
            _clientLogger.Debug("Begin connect on address {0} port {1}", this.addresses[0], this.port);
        }

        public void Disconnect()
        {
            if (this.client.Connected)
            {
                this.client.Close();
                if (this.Disconnected != null)
                    this.Disconnected(this, new EventArgs());
            }
        }
 
        /// <summary>
        /// Writes a string to the server using a given encoding.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>A Guid that can be used to match the data written to the confirmation event.</returns>
        public Guid Write(string value, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(value);
            return this.Write(buffer);
        }
 
        /// <summary>
        /// Writes a byte array to the server.
        /// </summary>
        /// <param name="buffer">The byte array to write.</param>
        /// <returns>A Guid that can be used to match the data written to the confirmation event.</returns>
        public Guid Write(byte[] buffer)
        {
            Guid guid = Guid.NewGuid();
            _clientLogger.Debug("Writing to socket with client {0} size: {1}", this.client.Client.LocalEndPoint.ToString(), buffer.Length);
            NetworkStream networkStream = this.client.GetStream();
            networkStream.BeginWrite(buffer, 0, buffer.Length, this.ClientWriteCallback, new StateAsyncInfo(guid, networkStream));
            return guid;
        }
 
        /// <summary>
        /// Callback from the asynchronous DNS lookup.
        /// </summary>
        /// <param name="asyncResult">The result of the async operation.</param>
        private void DnsGetHostAddressesCallback(IAsyncResult asyncResult)
        {
            try
            {
                this.addresses = Dns.EndGetHostAddresses(asyncResult);
                this.dnsGetHostAddressesResetEvent.Set();
            }
            catch (Exception ex)
            {
                if (this.DnsGetHostAddressesException != null)
                    this.DnsGetHostAddressesException(this, new ExceptionEventArgs(ex));
            }
        }
 
        /// <summary>
        /// Callback from the asynchronous Connect method.
        /// </summary>
        /// <param name="asyncResult">The result of the async operation.</param>
        private void ClientConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                this.client.EndConnect(asyncResult);
                _clientLogger.Debug("Connected to {0}", this.client.Client.RemoteEndPoint.ToString());
                if (this.Connected != null)
                    this.Connected(this, new EventArgs());
                _clientLogger.Debug("After calling Connected to {0}", this.client.Client.RemoteEndPoint.ToString());
            }
            catch (SocketException sockEx)
            {
                if (sockEx.ErrorCode == 10056)
                {
                    this.client.Client.Disconnect(true);
                }
                else
                {
                    _clientLogger.Error("Failed to get connection: {0}", sockEx.Message);
                }
            }
            catch (Exception ex)
            {
                _clientLogger.Debug(ex.Message);
                retries++;
                if (retries < 3)
                {
                    try
                    {
                        this.client.BeginConnect(this.addresses, this.port, this.ClientConnectCallback, null);

                    }
                    catch (Exception innnerex)
                    {
                        _clientLogger.Error(innnerex.ToString());
                    }
                }
                else
                {
                    if (this.ClientConnectException != null)
                        this.ClientConnectException(this, new ExceptionEventArgs(ex));
                }
                return;
            }
 
            try
            {
                NetworkStream networkStream = this.client.GetStream();
                StateAsyncInfo asyncInfo = new StateAsyncInfo(networkStream);
                networkStream.BeginRead(asyncInfo.ReadBuffer, 0, asyncInfo.ReadBuffer.Length, this.ClientReadCallback, asyncInfo);
            }
            catch (Exception ex)
            {
                if (this.ClientReadException != null)
                    this.ClientReadException(this, new ExceptionEventArgs(ex));
            }
        }
 
        /// <summary>
        /// Callback from the asynchronous Read method.
        /// </summary>
        /// <param name="asyncResult">The result of the async operation.</param>
        private void ClientReadCallback(IAsyncResult asyncResult)
        {
            try
            {
                StateAsyncInfo asyncInfo = asyncResult.AsyncState as StateAsyncInfo;
                
                NetworkStream networkStream = asyncInfo.NetStream; 
                int read = networkStream.EndRead(asyncResult);
 
                if (read == 0)
                {
                    this.Disconnect();
                }

                asyncInfo.AddToCompletedData(read);
                if (networkStream.DataAvailable)
                {
                    _clientLogger.Debug("Read {0} bytes going back for more", read);
                    networkStream.BeginRead(asyncInfo.ReadBuffer, 0, asyncInfo.ReadBuffer.Length, this.ClientReadCallback, asyncInfo);
                    return;
                }
                if (this.DataRead != null)
                    this.DataRead(this, new DataReadEventArgs(asyncInfo.CompletedData));
            }
            catch (Exception ex)
            {
                if (this.ClientReadException != null)
                    this.ClientReadException(this, new ExceptionEventArgs(ex));
            }
        }
 
        /// <summary>
        /// Callback from the asynchronous write callback.
        /// </summary>
        /// <param name="asyncResult">The result of the async operation.</param>
        private void ClientWriteCallback(IAsyncResult asyncResult)
        {
            try
            {
                StateAsyncInfo asyncInfo = asyncResult.AsyncState as StateAsyncInfo;
                NetworkStream networkStream = asyncInfo.NetStream;
                networkStream.EndWrite(asyncResult);
                //Guid guid = (Guid)asyncResult.AsyncState;
                if (this.DataWritten != null)
                    this.DataWritten(this, new DataWrittenEventArgs(asyncInfo.WriteGuid));
            }
            catch (Exception ex)
            {
                if (this.ClientWriteException != null)
                    this.ClientWriteException(this, new ExceptionEventArgs(ex));
            }
        }

        public string ConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.client.Connected)
            {
                sb.AppendFormat("Client connected to port {0}:");
                foreach (IPAddress addr in this.addresses)
                {
                    sb.AppendFormat("on address: {0}", addr.ToString());
                }
            }
            return sb.ToString();
        }
    }
 
    /// <summary>
    /// Provides data for an exception occurring event.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for a new Exception Event Args object.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        public ExceptionEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
 
        public Exception Exception { get; private set; }
    }
 
    /// <summary>
    /// Provides data for a data read event.
    /// </summary>
    public class DataReadEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for a new Data Read Event Args object.
        /// </summary>
        /// <param name="data">The data that was read from the remote host.</param>
        public DataReadEventArgs(byte[] data)
        {
            this.Data = data;
        }
 
        /// <summary>
        /// Gets the data that has been read.
        /// </summary>
        public byte[] Data { get; private set; }
    }
 
    /// <summary>
    /// Provides data for a data write event.
    /// </summary>
    public class DataWrittenEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for a Data Written Event Args object.
        /// </summary>
        /// <param name="guid">The guid of the data written.</param>
        public DataWrittenEventArgs(Guid guid)
        {
            this.Guid = guid;
        }
 
        /// <summary>
        /// Gets the Guid used to match the data written to the confirmation event.
        /// </summary>
        public Guid Guid { get; private set; }

    }
}
