 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MetaDataProtocol
{
    public class  MDDiscoveryPing : BaseNetMsg
    {
        #region private variables
        
        #endregion

        #region Constructor & Dispose
        public  MDDiscoveryPing()
            : base(NetMessages.DiscoveryMsg)
        {
        }

        public MDDiscoveryPing(byte [] data)
            : this()
        {
            this.FromByteArray(data, 0);
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
