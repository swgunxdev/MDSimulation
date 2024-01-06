/*****************************************************************
 * Copyright Spencer George 2014
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDModeling;

namespace MDProtocol
{
    class MDSendFile : BaseNetMsg, IToFromByteArray
    {
        #region private variables
        private string _fileName;
        #endregion

        #region Constructor & Dispose
        public MDSendFile()
            : base()
        {
        }

        public MDSendFile(string fileName)
            : this()
        {
            _fileName = fileName;
        }

        #endregion

        #region Public Properties
        public string FilePathName { get; set; }
        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            return base.ToByteArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            return base.FromByteArray(data, offset);
        }
        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
    }

    class MDSendFileReply : BaseNetMsg, IToFromByteArray
    {
        #region private variables
        private bool _successfullyInstalled = false;
        #endregion

        #region Constructor & Dispose
        public MDSendFileReply()
            : base()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            return base.ToByteArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            return base.FromByteArray(data, offset);
        }


        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
    }

}
