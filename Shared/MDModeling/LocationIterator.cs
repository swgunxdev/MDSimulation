using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDModeling
{
    public class LocationIdIterator : IEnumerator<IdNode>
    {
        private int _currentIndex = -1;
        private LocationID _locationId = null;

        public LocationIdIterator(LocationID locId)
        {
            _locationId = new LocationID(locId);
            MoveNext();
        }

        public IdNode Current
        {
            get { return _locationId[_currentIndex]; }
        }

        public LocationID CurrentLocation
        {
            get
            {
                List<IdNode> nodes = new List<IdNode>();
                if (_currentIndex >= 0)
                {
                    for (int index = 0; index <= _currentIndex; index++)
                    {
                        nodes.Add(new IdNode(_locationId[index]));
                    }                        
                }
                return new LocationID(nodes.ToArray());
            }
        }

        public LocationID Original
        {
            get { return _locationId; }
        }
        public void Dispose()
        {
            ;
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return _locationId[_currentIndex];
            }
        }

        public bool MoveNext()
        {
            if (_currentIndex + 1 < _locationId.Count)
            {
                _currentIndex += 1;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public bool IsDone()
        {
            return (_currentIndex + 1 >= _locationId.Count);
        }
    }
}
