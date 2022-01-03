using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using ClientEngine.NetClients;

namespace ClientEngine.NetServers
{
	public class ConnectedClient
	{
		private enum EConnectionState
		{
			NotLoggedIn,
			LoggedIn,
		}
		
		private TcpClient _Listener = null;
		private byte[] _Buffer = new byte[0x2000];
		private EConnectionState _ConnectionState = EConnectionState.NotLoggedIn;
		
		public delegate void UsernamePasswordDelegate(string username, string password, out bool isValid);
		public event UsernamePasswordDelegate GetUsernamePassword;
		
		public delegate void TcpConnectionPortsDelegate(EndPoint addr, out int commandPort, out int meterPort, out int statusPort, out int firmwarePort);
		public event TcpConnectionPortsDelegate TcpConnectionPorts;
		
		private CommandHandler _CommandHandler = null;
		private StatusHandler _StatusHandler = null;
		private FirmwareClient _FirmwareHandler = null;
		private MeterHandler _MeterHandler = null;
		
		public ConnectedClient(TcpClient listener)
		{
			_Listener = listener;
			
			// start listening for data
			_Listener.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData),
				_Listener.GetStream());
		}

		public void Stop ()
		{
			if (_CommandHandler != null)
			{
				_CommandHandler.StopListening();
				_CommandHandler = null;
			}
			
			if (_FirmwareHandler != null)
			{
				_FirmwareHandler.StopListening();
				_FirmwareHandler = null;
			}
			
			if (_Listener != null)
			{
				_Listener.Close();
				_Listener = null;
			}
			
			if (_MeterHandler != null)
			{
				_MeterHandler.StopListening();
				_MeterHandler = null;
			}
			
			if (_StatusHandler != null)
			{
				_StatusHandler.StopListening();
				_StatusHandler = null;
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
					_Listener.GetStream().BeginRead(_Buffer, 0, _Buffer.Length, new AsyncCallback(OnReceiveData), _Listener.GetStream());
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}
		
		private void HandleData(int bytesReceived)
		{
			// copy the data into another buffer
			byte[] data = new byte[0x2000];
			_Buffer.CopyTo(data, 0);
			
			// translate the buffer
			string sData = UnicodeEncoding.Unicode.GetString(data, 0, bytesReceived);
			
			ProcessData(sData);
		}
		
		private void ProcessData(string sData)
		{
			// depending on the state, we need to deal differently
			switch(_ConnectionState)
			{
			case EConnectionState.NotLoggedIn:
				HandleLogin(sData);
				break;
			case EConnectionState.LoggedIn:
				HandleCommand(sData);
				break;
			}
		}
		
		private void HandleLogin(string data)
		{
			// the first of the string must be "login"
			int index = data.ToLower().IndexOf("login");
			
			if (index == 0)
			{
				if (IsUsernamePasswordValid(data))
				{
					// send the login accepted message
					byte[] bData = Encoding.Unicode.GetBytes("login accepted");
					_Listener.GetStream().Write(bData, 0, bData.Length);
					
					bData = GetPortsForConnection();
					_Listener.GetStream().Write(bData, 0, bData.Length);
					
					// move the state
					_ConnectionState = EConnectionState.LoggedIn;
					
					Console.WriteLine("TCP login accepted");
				}
				else
				{
					// send the login rejected message
					byte[] bData = Encoding.Unicode.GetBytes("login rejected");
					_Listener.GetStream().Write(bData, 0, bData.Length);
					
					Console.WriteLine("TCP Login rejected");
				}
			}
			else
			{
				// send the invalid command message
				byte[] bData = Encoding.Unicode.GetBytes("Invalid command. Not logged in.");
				_Listener.GetStream().Write(bData, 0, bData.Length);
				
				Console.WriteLine("TCP Invalid command durning login");
			}
		}
		
		private bool IsUsernamePasswordValid(string data)
		{
			bool isValid = false;
			string username = string.Empty, password = string.Empty;
			
			int userIndex = data.IndexOf("username:") + 9;
			int userSpaceIndex = data.IndexOf(" ", userIndex + 1);
			int passIndex = data.IndexOf("password:") + 9;
			int passSpaceIndex = data.IndexOf(" ", passIndex + 1);
			
			username = data.Substring(userIndex, userSpaceIndex - userIndex);
			if (passSpaceIndex == -1)
			{
				password = data.Substring(passIndex);
			}
			else
			{
				password = data.Substring(passIndex, passSpaceIndex - passIndex);
			}
			
			// check the list of usernames
			if (GetUsernamePassword != null)
			{
				username = username.Trim();
				password = password.Trim();
				GetUsernamePassword(username, password, out isValid);
			}
			
			return isValid;
		}
		
		private void HandleCommand(string data)
		{
			// any other command besides logging in/out should be on a different port
			// send back an error
			
			if (data.ToLower().Contains("disconnect"))
			{
				Stop();
			}
			else
			{
				byte[] bData = Encoding.Unicode.GetBytes("Non log in/out commands not supported on this port");
				_Listener.GetStream().Write(bData, 0, bData.Length);
				
				Console.WriteLine("TCP Got command in wrong place");
			}
		}
		
		private byte[] GetPortsForConnection()
		{
			byte[] bData = null;
			int commandPort = 0;
			int meterPort = 0;
			int statusPort = 0;
			int firmwarePort = 0;
			
			if (TcpConnectionPorts != null)
			{
				TcpConnectionPorts(_Listener.Client.RemoteEndPoint, out commandPort, out firmwarePort, out meterPort, out statusPort);
			}
			
			string str = string.Format("commandPort={0}; firmwarePort={1}; meterPort={2}; statusPort={3};",
				commandPort, firmwarePort, meterPort, statusPort);
			
			SetupPortListeners(commandPort, firmwarePort, meterPort, statusPort);
			
			bData = Encoding.Unicode.GetBytes(str);
			
			return bData;
		}

		private void SetupPortListeners (int commandPort, int firmwarePort, int meterPort, int statusPort)
		{
			_CommandHandler = new CommandHandler();
			_FirmwareHandler = new FirmwareClient();
			_MeterHandler = new MeterHandler();
			_StatusHandler = new StatusHandler();
			
			_CommandHandler.StartListening(commandPort);
			_FirmwareHandler.StartListening(firmwarePort);
			_MeterHandler.StartListening(meterPort);
			_StatusHandler.StartListening(statusPort);
			
			Console.WriteLine("TCP Additional ports set up and listening");
		}
	}
}
