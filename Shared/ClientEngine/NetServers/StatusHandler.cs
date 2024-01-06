using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;

namespace ClientEngine.NetServers
{
	public class StatusHandler
	{
		private TcpListener _Listener = null;
		private TcpClient _Client = null;
		private int _Port = 0;
		private byte[] _Buffer = new byte[0x2000];
		private bool _Connected = false;
		
		public StatusHandler ()
		{
		}
		
		~StatusHandler()
		{
			StopListening();
		}
		
		public bool Connected
		{
			get { return _Connected; }
		}

		public void StartListening (int statusPort)
		{
			_Port = statusPort;
			_Listener = new TcpListener(new IPEndPoint(IPAddress.Any, _Port));
			_Listener.Start();
			_Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
			_Connected = true;
			
			Console.WriteLine(string.Format("TCP Status handler accepting on port {0}", _Port));
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
			
			Console.WriteLine("TCP Status handler stopped");
		}
		
		private void OnAcceptConnection(IAsyncResult ar)
		{
			TcpListener listener = ar.AsyncState as TcpListener;
			
			if (listener != null)
			{
				try
				{
					_Client = listener.EndAcceptTcpClient(ar);
					_Client.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData),
						_Client.GetStream());
					
					Console.WriteLine("TCP Status accepted connection. listening for more");
					
					_Listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptConnection), _Listener);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}
		
		private void OnReceiveData(IAsyncResult ar)
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
					Console.WriteLine(ex);
				}
			}
		}
		
		private void HandleData(int bytesReceived)
		{
			// send the acknowledge
			byte[] data = Encoding.Unicode.GetBytes(string.Format("ack. received {0} bytes.", bytesReceived));
			_Client.GetStream().Write(data, 0, data.Length);
			
			Console.WriteLine(string.Format("TCP Status handler received {0} bytes.", bytesReceived));
			
			// TODO: when we implement commands, handle them here
		}
	}
}
