using System;

namespace MetadataUnitTesting.TestHelpers
{
	public class EventCounter    {
        private int _counter;

        public int Value
        {
            get { return _counter; }
            private set { _counter = value; }
        }

        public void Increment(object o, EventArgs e)
        {
            Value++;
        }
    } 
}

