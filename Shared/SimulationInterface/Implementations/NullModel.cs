//
// File Name: NullModel
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SimulationInterface.Implementations
{
    /// <summary>
    /// NullModel
    /// </summary>
    public class NullModel : IDSStorageModel
    {
        #region Private Variables
        #endregion // Private Variables

        #region Public Events
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public NullModel()
        {
        }
        #endregion // Constructor / Dispose

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public ulong ModelId
        {
            get { return 0; }
        }

        public string ModelName
        {
            get { return string.Empty; }
        }

        //public ISendReceiveCmds CommandInterface
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion // Properties

        #region Public Methods
        public int LoadModel(string modelFileName)
        {
            // do nothing
            return 0;
        }

        public int UnloadModel()
        {
            // do nothing
            return 0;
        }
        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion
    }
}
