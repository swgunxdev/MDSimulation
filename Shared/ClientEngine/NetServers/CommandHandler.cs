using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Slf;

namespace ClientEngine.NetServers
{

    public abstract class TCPProtocolHandler
    {
        protected TcpListener _Listener = null;
        protected TcpClient _Client = null;
        protected int _Port = 0;
        protected byte[] _Buffer = new byte[0x2000];
        protected bool _Connected = false;
        protected ILogger _logger = null;

        public TCPProtocolHandler()
        {
            _logger = LoggerService.GetLogger();
        }

        ~TCPProtocolHandler()
        {
            StopListening();
        }

        public bool Connected
        {
            get { return _Connected; }
        }

        public int Port { get { return _Port; } }

        public void StartListening(int port)
        {
            _Port = port;
            _Listener = new TcpListener(new IPEndPoint(IPAddress.Any, _Port));
            _Listener.Start();
            _Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
            _Connected = true;

            _logger.Debug(string.Format("TCP Command handler accepting on port {0}", _Port));
        }

        public void StopListening()
        {
            if (_Listener != null)
            {
                _Listener.Stop();
                _Listener = null;
            }

            if (_Client != null)
            {
                _Client.Close();
                _Client = null;
            }

            _Connected = false;

            _logger.Debug("TCP Command handler stopped listening");
        }

        protected virtual void OnAcceptConnection(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;

            if (listener != null)
            {
                try
                {
                    _Client = listener.EndAcceptTcpClient(ar);
                    _Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData),
                        _Client.GetStream());

                    _logger.Debug("TCP Command handler accepted connection. listening for more");

                    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                }
            }
        }

        protected virtual void OnReceiveData(IAsyncResult ar)
        {
            NetworkStream stream = ar.AsyncState as NetworkStream;

            if (stream != null)
            {
                try
                {
//                  int bytesReceived = stream.EndRead(ar);
                    stream.EndRead(ar);

                    // do something with the data
                    //HandleData(bytesReceived);

                    // start listening again
                    _Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData), _Client.GetStream());
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                }
            }
        }
    }

    public class CommandHandler : TCPProtocolHandler
    {
        //private TcpListener _Listener = null;
        //private TcpClient _Client = null;
        //private int _Port = 0;
        //private byte[] _Buffer = new byte[0x2000];
        //private bool _Connected = false;
        private Dictionary<Command, Action<CommandArgs>> _commands = null;

        public CommandHandler()
        {
            InitCommandDictionary();
        }

        ~CommandHandler()
        {
            StopListening();
        }

        //public bool Connected
        //{
        //    get { return _Connected; }
        //}

        //public void StartListening(int port)
        //{
        //    _Port = port;
        //    _Listener = new TcpListener(new IPEndPoint(IPAddress.Any, _Port));
        //    _Listener.Start();
        //    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
        //    _Connected = true;

        //    _logger.Debug(string.Format("TCP Command handler accepting on port {0}", _Port));
        //}

        //public void StopListening()
        //{
        //    if (_Listener != null)
        //    {
        //        _Listener.Stop();
        //        _Listener = null;
        //    }

        //    if (_Client != null)
        //    {
        //        _Client.Close();
        //        _Client = null;
        //    }

        //    _Connected = false;

        //    _logger.Debug("TCP Command handler stopped listening");
        //}

        protected override void OnAcceptConnection(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;

            if (listener != null)
            {
                try
                {
                    _Client = listener.EndAcceptTcpClient(ar);
                    _Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData),
                        _Client.GetStream());

                    _logger.Debug("TCP Command handler accepted connection. listening for more");

                    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                }
            }
        }

        protected override void OnReceiveData(IAsyncResult ar)
        {
            NetworkStream stream = ar.AsyncState as NetworkStream;

            if (stream != null)
            {
                try
                {
                    int bytesReceived = stream.EndRead(ar);

                    // do something with the data
                    HandleData(bytesReceived);

                    // start listening again
                    _Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData), _Client.GetStream());
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                }
            }
        }

        private void HandleData(int bytesReceived)
        {
            // send the acknowledge
            byte[] data = Encoding.Unicode.GetBytes(string.Format("ack. received {0} bytes.", bytesReceived));
            _Client.GetStream().Write(data, 0, data.Length);

            _logger.Debug(string.Format("TCP Command handler received {0} bytes.", bytesReceived));

            // TODO: when we implement commands, handle them here
            // verify header

            DispatchCommand();
        }

        private void DispatchCommand()
        {
            // What type of command is this?
            // Pull out payload if any pass to command handlers
            // Get Command out
            try
            {
                CommandArgs args = new CommandArgs();
                Command cmd = Command.Noop;
                _commands[cmd].Invoke(args);
            }
            catch (KeyNotFoundException keyExp)
            {
                // Command not found skipping
                _logger.Debug(keyExp.Message);
            }
        }

        private void InitCommandDictionary()
        {
            _commands = new Dictionary<Command, Action<CommandArgs>>() {
                {Command.Noop, NoopCmd},
            };
        }

        private void NoopCmd(CommandArgs args)
        {
            ; // no operations to perform
        }

        private void HandleFirmwareUpdateCmd(CommandArgs args)
        {
            ; // tear apart parameters and create response.
        }

        private void HandleSendConfigurationFileCmd(CommandArgs args)
        {
            ; // tear apart parameters and create response.
        }
    }
}