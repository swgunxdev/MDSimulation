/*****************************************************************
 * Copyright Spencer George 2014
 ****************************************************************/

using System.IO;
using Earlz.BareMetal;
using MetaDataModeling.Providers;
using MetaDataModeling.Utils;

namespace MetaDataProtocol
{
    public enum AuthenticationValues
    {
        NOTAUTHENTICATED = 0,
        SUCCESS,
        BADNAMEORPASSWORD,
        OTHER,
    };

    public class MDAuthenticateReply : BaseNetMsg
    {
        #region private variables
        private AuthenticationValues _returnValue = AuthenticationValues.NOTAUTHENTICATED;
        #endregion

        #region Constructor & Dispose
        public MDAuthenticateReply()
        {
            CommandPort = 0;
            FirmwarePort = 0;
            MeterPort = 0;
        }

        public MDAuthenticateReply(byte [] data)
        {
            FromByteArray(data, 0);
        }

        #endregion

        #region Public Properties
        public AuthenticationValues Return 
        {
            get 
            {
                    return _returnValue;
            }

            set
            {
                _returnValue = value;
            }
        }

        public int CommandPort { get; set; }

        public int FirmwarePort { get; set; }

        public int MeterPort { get; set; }

        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte[] b = base.ToByteArray();
            ms.Write(b, 0, b.Length);
            byte[] msgId = BytesProvider<int>.Default.GetBytes(EnumExtensions.ToInt(_returnValue));
            ms.Write(msgId, 0, msgId.Length);
            msgId = BytesProvider<int>.Default.GetBytes(CommandPort);
            ms.Write(msgId, 0, msgId.Length);
            msgId = BytesProvider<int>.Default.GetBytes(FirmwarePort);
            ms.Write(msgId, 0, msgId.Length);
            msgId = BytesProvider<int>.Default.GetBytes(MeterPort);
            ms.Write(msgId, 0, msgId.Length);
            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            int curPos = offset;
            _returnValue = EnumExtensions.ToEnum<int, AuthenticationValues>(ByteArryTypeProvider<int>.Default.Convert(data, curPos));
            curPos += BareMetal.SizeOf<int>();
            CommandPort = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<int>();
            FirmwarePort = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<int>();
            MeterPort = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<int>();
            return curPos;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
    }
}
