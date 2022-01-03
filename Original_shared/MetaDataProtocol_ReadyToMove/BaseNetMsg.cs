using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaDataModeling;
using Earlz.BareMetal;
using MetaDataModeling.Providers;
using System.IO;
using MetaDataModeling.Utils;
using TimpInterfaces.Implementations;

namespace MetaDataProtocol
{
    public enum NetMessages
    {
        NoMessage,
        DiscoveryMsg,
        DiscoveryReply,
        AuthConnectMsg,
        AuthConnectReply,
        SetPropertyMsg,
        SetProperyReply,
        GetPropertyMsg,
        GetPropertyReply,
        SendModelFileMsg,
        SendModelFileReply,
        StartModelSimulationMsg,
        StartModelSimulationReply,
        StopmodelSimulationMsg,
    }

    public class BaseNetMsg : IToFromByteArray
    {
        #region private variables
        protected NetMessages _cmdId;
        #endregion

        #region Constructor & Dispose
        public BaseNetMsg()
        {
            _cmdId = NetMessages.NoMessage;
        }

        public BaseNetMsg(NetMessages msgType)
            :this()
        {
            _cmdId = msgType;
        }


        #endregion

        #region Public Properties
        public NetMessages CmdId { get { return _cmdId; } }
        #endregion

        #region Public Methods
        public virtual byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte[] msgId = BytesProvider<int>.Default.GetBytes(EnumExtensions.ToInt(_cmdId));
            ms.Write(msgId, 0, msgId.Length);
            return ms.ToArray();
        }

        public virtual int FromByteArray(byte[] data, int offset)
        {
            int curPos = offset;
            _cmdId = EnumExtensions.ToEnum<int, NetMessages>(ByteArryTypeProvider<int>.Default.Convert(data, curPos));
            curPos += BareMetal.SizeOf<int>();
            return curPos;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion



    }
}
