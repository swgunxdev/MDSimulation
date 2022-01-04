using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace ClientEngine.NetServers
{
	public class MeterHandler
	{
		private struct UdpState
		{
			public IPEndPoint e;
			public UdpClient u;
		}

		private UdpClient _Client = null;
		private int _Port = 0;
		private bool _Connected = false;
		private bool _Done;
		private Thread _ListenThread = null;
		private Random _Random = new Random();
		private EndPoint _Remote;
	
		public MeterHandler()
		{
			_Done = true;
		}
		
		~MeterHandler()
		{
			StopListening();
		}
		
		public bool Connected
		{
			get { return _Connected; }
		}


		public void StartListening(int meterPort)
		{
			_Port = meterPort;
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, _Port);
			_Client = new UdpClient(ep);
			UdpState us = new UdpState();
			us.u = _Client;
			us.e = ep;
			_Client.BeginReceive(new AsyncCallback(OnReceiveData), us);
			_Connected = true;
			
			Console.WriteLine(string.Format("UDP Meter handler accepting on port {0}", _Port));
		}
		
		public void StopListening()
		{
			if (_Client != null)
			{
				_Client.Close();
				_Client = null;
			}
			
			_Connected = false;
			_Done = true;
		}
		
		private void OnReceiveData(IAsyncResult ar)
		{
			UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
			IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;
			
			// do something with the data
			try
			{
				byte[] data = u.EndReceive(ar, ref e);
				HandleData(data, e);
				
				// begin listening again
				Console.WriteLine("UDP Meter continuing to receive data");
				_Client.BeginReceive(new AsyncCallback(OnReceiveData), (UdpState)ar.AsyncState);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
		
		private void HandleData(byte[] data, EndPoint remote)
		{
			// send the acknowledge

			if (data.Length > 0)
			{
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				s.EnableBroadcast = true;
				byte[] ackData = Encoding.Unicode.GetBytes(string.Format("ack. received {0} bytes.", data.Length));
				
				IPAddress ip = GetMatchingIpAddress(remote);
				
				s.Bind(new IPEndPoint(ip, _Port));
				s.SendTo(ackData, remote);
				
				s.Close();
				
				Console.WriteLine(string.Format("UDP Meter handler received {0} bytes.", data.Length));
			}
						
			// TODO: Implement the command sending here
			
			string str = Encoding.Unicode.GetString(data);
			if (str.ToLower().Contains("start"))
			{
				_Remote = remote;
				StartSendingMeterData();
			}
			else if (str.ToLower().Contains("stop"))
			{
				_Done = true;
				Console.WriteLine("UDP Meter handler set Done to true");
			}
		}

		void StartSendingMeterData ()
		{
			if (_Done)
			{
				_Done = false;
				
				try
				{
					if (_ListenThread == null || !_ListenThread.IsAlive)
					{
						ThreadStart ts = new ThreadStart(ListenThread);
						_ListenThread = new Thread(ts);
						_ListenThread.Name = "UdpListenThread";
						_ListenThread.Priority = ThreadPriority.Normal;
						_ListenThread.Start();
						Console.WriteLine("UDP Started listening thread");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}
		
		private void ListenThread()
		{
			while (!_Done)
			{
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				s.EnableBroadcast = true;
				IPAddress ip = GetMatchingIpAddress(_Remote);
				s.Bind(new IPEndPoint(ip, _Port));
				
				byte[] ackData = Encoding.Unicode.GetBytes(string.Format("{0}", _Random.Next() % 100));
				s.SendTo(ackData, _Remote);
				s.Close();
				
				Thread.Sleep(5);
			}
		}
		
		private IPAddress GetMatchingIpAddress(EndPoint remote)
		{
			IPAddress retval = IPAddress.None;
			List<IPAddress> addresses = new List<IPAddress>();
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

			foreach (NetworkInterface adapter in adapters)
			{
				IPInterfaceProperties properties = adapter.GetIPProperties();

				for (int i = 0; i < properties.UnicastAddresses.Count; i++)
				{
					IPAddress add = properties.UnicastAddresses[i].Address;

					if (add.AddressFamily == AddressFamily.InterNetwork)
					{
						if (!add.ToString().Contains("127"))
						{
							addresses.Add(add);
						}
					}
				}
			}
			
			for (int i = 0; i < addresses.Count; i++)
			{
				string localIp = addresses[i].ToString();
				string remoteIp = remote.ToString();
				
				// make sure first two numbers are the same
				int localIndex = localIp.IndexOf('.');
				localIndex = localIp.IndexOf('.', localIndex + 1);
				localIp = localIp.Substring(0, localIndex);
				
				int remoteIndex = remoteIp.IndexOf('.');
				remoteIndex = remoteIp.IndexOf('.', remoteIndex + 1);
				remoteIp = remoteIp.Substring(0, remoteIndex);
				
				if (localIp == remoteIp)
				{
					retval = addresses[i];
					break;
				}
			}
			
			return retval;
		}		
	}
}
