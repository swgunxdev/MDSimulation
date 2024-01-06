/*****************************************************************
 * Copyright Spencer George 2014
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Networking
{
    public class StateAsyncInfo
    {
        byte[] _completed = null;
        byte[] _readBuffer = null;

        public Guid WriteGuid { get; set; }
        public Byte[] ReadBuffer
        {
            get
            {
                return _readBuffer;
            }
        }
        public byte[] CompletedData
        {
            get
            {
                return _completed;
            }
        }
        public NetworkStream NetStream { get; set; }

        public StateAsyncInfo(int buffersize)
        {
            _readBuffer = new byte [buffersize];
        }

        public StateAsyncInfo(NetworkStream stream)
            : this(8192)
        {
            NetStream = stream;
        }

        public StateAsyncInfo(Guid guid, NetworkStream stream)
            : this(stream)
        {
            WriteGuid = guid;
         }
        #region private variables

        #endregion

        #region Constructor & Dispose
        public StateAsyncInfo()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods
        public void AddToCompletedData(int bytesToCopy)
        {
            if (_completed == null)
            {
                _completed = new byte[bytesToCopy];
                Array.Copy(_readBuffer, _completed, bytesToCopy);
            }
            else
            {
                byte[] newcompleted = new byte[bytesToCopy + _completed.Length];
                Array.Copy(_completed, newcompleted, _completed.Length);
                Array.Copy(_readBuffer, 0, newcompleted, _completed.Length, bytesToCopy);
                _completed = newcompleted;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
    }
}
