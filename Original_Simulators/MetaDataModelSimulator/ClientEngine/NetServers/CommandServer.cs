//
// File Name: CommandServer
// ----------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Sockets;
using Networking;

namespace ClientEngine.NetServers
{
	/// <summary>
	/// CommandServer
	/// </summary>
	public class CommandServer : TCPAsyncServer
	{
		#region Private Variables
		#endregion // Private Variables

		#region Public Events
		#endregion // Public Events

		#region Constructor / Dispose
		/// <summary>
		/// The default constructor
		/// </summary>
		public CommandServer()
			: base(new IPEndPoint(IPAddress.Any,0))
		{
			PerformDataOperation = ProcessCommand;
			Name = "Command Server";
		}
		#endregion // Constructor / Dispose

		#region Properties
		#endregion // Properties

		#region Public Methods
		#endregion // Public Methods

		#region Private Methods
		/// <summary>
		/// Callback for the read operation.
		/// </summary>
		/// <param name="result">The async result object</param>
		protected override void ReadCallback(IAsyncResult result)
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
			if (PerformDataOperation != null)
			{
				byte[] reply = PerformDataOperation(client);
				if (reply == null)
				{
					_logger.Error("Command Processing no return message yet.");
					return;
				}
				else
				{
					// Tell the client the info
					Write(client.TcpClient, reply);
				}
			}
			networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
		}

		private byte [] ProcessCommand(SrvClient client)
		{
			byte[] reply = null;
			return reply;
		}
		#endregion // Private Methods

		#region enums
		#endregion

	}
}
