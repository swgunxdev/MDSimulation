//
// File Name: Authentication
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimulationInterface.Implementations
{
    public struct AuthenticationRequest
    {
        public string UserName;
        public string Password;
        public byte [] ToByteArray()
        {
            string authString = string.Format("{0};{1}", UserName, Password);
            MemoryStream data = new MemoryStream();
            byte[] authData = Encoding.Unicode.GetBytes(authString);
            data.Write(authData, 0, authData.Length);
            return data.ToArray();
        }

        public static AuthenticationRequest FromByteArray(byte [] data)
        {
            string authString = Encoding.Unicode.GetString(data);
            string [] parts = authString.Split(';');
            AuthenticationRequest request = new AuthenticationRequest();
            request.UserName = parts[0];
            request.Password = parts[1];
            return request;
        }
    }

    public struct AuthenticationReply
    {
        public bool success;
        public int FirmwarePort;
        public int CommandPort;
        public int MeterPort;
        public int StatusPort;

        public byte [] ToByteArray()
        {
            string portString = string.Format("{0};{1};{2};{3};{4}", success.ToString(), FirmwarePort.ToString(),CommandPort.ToString(), MeterPort.ToString(), StatusPort.ToString());
            MemoryStream data = new MemoryStream();
            byte[] portData = Encoding.Unicode.GetBytes(portString);
            data.Write(portData, 0, portData.Length);
            return data.ToArray();
        }

        public static AuthenticationReply FromByteArray(byte[] data)
        {
            string authString = Encoding.Unicode.GetString(data);
            string [] parts = authString.Split(';');
            AuthenticationReply reply = new AuthenticationReply();
            reply.success = Convert.ToBoolean(parts[0]);
            reply.FirmwarePort = Convert.ToInt32(parts[1]);
            reply.CommandPort = Convert.ToInt32(parts[2]);
            reply.MeterPort = Convert.ToInt32(parts[3]);
            reply.StatusPort = Convert.ToInt32(parts[4]);
            return reply;
        }

    }
 
}
