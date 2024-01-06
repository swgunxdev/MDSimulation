using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MetadataUnitTesting
{
    public class EventChangeCounter
    {
        int _counter = 0;

		public object Sender { get; set; }

		public PropertyChangedEventArgs EventArgs { get; set; }

        public int Counter { get { return _counter; } }

        public void EventChangedHandler(object sender, PropertyChangedEventArgs e)
        {
			Sender = sender;
			EventArgs = e;
            _counter += 1;
        }
    }
}
