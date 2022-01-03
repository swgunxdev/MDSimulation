/*****************************************************************
 * Copyright Spencer George 2014
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaDataModeling;
using System.IO;

namespace MetaDataProtocol
{
    class MDSetPropertyMsg : BaseNetMsg, IToFromByteArray
    {
        #region private variables

        #endregion

        #region Constructor & Dispose
        public MDSetPropertyMsg()
            : base(NetMessages.SetPropertyMsg)
        {
        }

        public MDSetPropertyMsg(LocationID locId, object value)
            : this()
        {

        }

        public MDSetPropertyMsg( byte [] data)
            : this()
        {
            FromByteArray(data, 0);
        }
        #endregion

        #region Public Properties
        public LocationID LocationId { get; set; }

        public IdType Value { get; set; }
        #endregion

        #region Public Methods
        public override byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            byte [] baseBytes = base.ToByteArray();
            ms.Write(baseBytes, 0, baseBytes.Length);
            byte[] locationBytes = LocationId.ToByteArray();
            ms.Write(locationBytes, 0, locationBytes.Length);
            byte[] valueBytes = MetaDataPayload.ToByteArray(Value);
            ms.Write(valueBytes, 0, valueBytes.Length);
            return ms.ToArray();
        }

        public override int FromByteArray(byte[] data, int offset)
        {
            int curPos = base.FromByteArray(data, offset);
            LocationId = new LocationID();
            curPos = LocationId.FromByteArray(data, curPos);
            Tuple<IdType, int> mdPayload = MetaDataPayload.FromByteArray(data, curPos);
            Value = mdPayload.Item1;
            curPos = mdPayload.Item2;
            return curPos;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Enumertations

        #endregion
    }
    class MDSetPropertyReply : BaseNetMsg, IToFromByteArray
    {
        #region private variables

        #endregion

        #region Constructor & Dispose
        public MDSetPropertyReply()
            : base(NetMessages.SetProperyReply)
        {
        }

        public MDSetPropertyReply(byte [] data)
        {
            FromByteArray(data, 0);
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
