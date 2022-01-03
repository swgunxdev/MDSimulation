using System;
using System.Net;
using System.Net.Sockets;
using Networking;
using Networking.Interfaces;
using TimpInterfaces;
using MetaDataModeling;
using MetaDataProtocol;

namespace ClientEngine.NetServers
{
	public class AuthenticationServer : TCPAsyncServer
	{
		private IAuthenticatorSimple _authenticator = null;
 
		/// <summary>
		/// Constructor for a new server on any IP address and the OS picks the port number.
		/// </summary>
		/// <param name="auth">The authenticator to use.</param>
		public AuthenticationServer(IAuthenticatorSimple auth)
			: base(new IPEndPoint(IPAddress.Any,0))
		{
			_authenticator = auth;
			PerformDataOperation = HandleAuthentication;
			Name = "Authentication Server";
		}

        public GetOperatingPortDelegate CommandPort { get; set; }

        public GetOperatingPortDelegate FirmwarePort { get; set; }

        public GetOperatingPortDelegate MeterPort { get; set; }

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
					DisconnectClient(client);
					return;
				}
				else
				{
					// Tell the client the info
					Write(client.TcpClient, reply);
					// then disconnect.
					DisconnectClient(client);
					return;
				}
			}
			networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
		}

		private byte [] HandleAuthentication (SrvClient client)
		{
			uint index = 0;
			_logger.Info ("Received client for authentication");
            Packet authPacket = new Packet(client.Buffer);
            if (authPacket.IsValid && authPacket.Payload.Header.PayloadType == (ushort)NetMessages.AuthConnectMsg)
            {
                MDAuthenticateMsg authMsg = new MDAuthenticateMsg(authPacket.Payload.PayloadData);

                MDAuthenticateReply reply = new MDAuthenticateReply();

                //// do we have an authenticator? 
                if (_authenticator != null)
                {
                    // yes -- decode and use authenticator 
                    // did the creds authenticate
                    if (_authenticator.Authenticate(authMsg.UserName, authMsg.Password) == false)
                    {   // no drop the connection
                        _logger.Info("Client {0} was denied", client.ConnectionAddress);
                        reply.Return = AuthenticationValues.BADNAMEORPASSWORD;
                        Packet replyPacket = new Packet(1, new Payload((ushort)NetMessages.AuthConnectReply, 1, reply.ToByteArray()));
                        return replyPacket.ToByteArray();
                    }
                }
                // Authentication Success

                // start up other servers
                _logger.Info ("Client {0} was authenticated", client.ConnectionAddress);


                reply.Return = AuthenticationValues.SUCCESS;
                if (FirmwarePort != null)
                {
                    reply.FirmwarePort = FirmwarePort();
                }
                if (CommandPort != null)
                {
                    reply.CommandPort = CommandPort();
                }
                if (MeterPort != null)
                {
                    reply.MeterPort = MeterPort();
                }
                Packet responsePacket = new Packet(1, new Payload((ushort)NetMessages.AuthConnectReply, 1, reply.ToByteArray()));
                return responsePacket.ToByteArray();
            }
            return new byte[0];
		}
	}
}
