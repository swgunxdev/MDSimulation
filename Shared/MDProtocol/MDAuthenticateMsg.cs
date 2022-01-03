using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using MDModeling.Providers;

namespace MDProtocol
{
    public class MDAuthenticateMsg : BaseNetMsg
    {
        #region private variables
        string _userName = string.Empty;
        string _password = string.Empty;
        #endregion

        #region Constructor & Dispose
        public MDAuthenticateMsg()
            : base(NetMessages.AuthConnectMsg)
        {
        }

        public MDAuthenticateMsg(string userName, string password)
            : this()
        {
            _userName = userName;
            _password = password;
        }

        public MDAuthenticateMsg(byte [] data)
            : this()
        {
            FromByteArray(data, 0);
        }

        #endregion

        #region Public Properties
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte [] baseBytes = base.ToByteArray();
            ms.Write(baseBytes, 0, baseBytes.Length);
            byte[] nameData = Encoding.UTF8.GetBytes(_userName);
            byte[] value = BytesProvider<int>.Default.GetBytes(nameData.Length);
            ms.Write(value, 0, value.Length);
            ms.Write(nameData, 0, nameData.Length);

            byte[] passwdData = Encoding.UTF8.GetBytes(_password);
            byte[] pwdLen = BytesProvider<int>.Default.GetBytes(passwdData.Length);
            ms.Write(pwdLen, 0, pwdLen.Length);
            ms.Write(passwdData, 0, passwdData.Length);
            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            int curPos = 0;
            curPos = base.FromByteArray(data, offset);
            // deserialize the length of the name string
            int nameLength = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += Marshaller.SizeOf<int>();

            // deserialize the name
            _userName = ByteArryTypeProvider<string>.Default.Convert(data, curPos);
            //_name = _name.Substring(0, nameLength);
            curPos += nameLength;

            int passwordLength = ByteArryTypeProvider<int>.Default.Convert(data, curPos);
            curPos += Marshaller.SizeOf<int>();

            // deserialize the name
            _password = ByteArryTypeProvider<string>.Default.Convert(data, curPos);
            //_name = _name.Substring(0, nameLength);
            curPos += passwordLength;
            return curPos;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
   }
}
