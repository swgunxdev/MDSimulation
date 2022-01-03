using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling.Utils
{
    public class InvalidValueException : InvalidOperationException
    {
        public InvalidValueException(string msg)
            : base(msg)
        {
        }
    }
}
