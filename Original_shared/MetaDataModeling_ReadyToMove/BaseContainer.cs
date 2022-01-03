using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using MetaDataModeling;
using System.ComponentModel;
using MetaDataModeling.Providers;
using Earlz.BareMetal;

namespace MetaDataModeling
{
	public class BaseContainer : IdType, IMDSerialize
	{
        protected readonly byte[] EndOfContainer = new byte[1] { 0xFF };
		protected Dictionary<LocationID, BaseContainer> _children;
		protected Dictionary<LocationID, PropertyBase> _properties;
		protected ulong _interfaceId = 0;

		public BaseContainer()
			: base()
		{
			_children = new Dictionary<LocationID, BaseContainer>();
			_properties = new Dictionary<LocationID, PropertyBase>();
		}

		public BaseContainer(ushort id, ushort typeId, string name)
			: base(id, typeId, name)
		{
			_children = new Dictionary<LocationID, BaseContainer>();
			_properties = new Dictionary<LocationID, PropertyBase>();
		}

		public BaseContainer(ushort id, ushort typeId, string name, ulong interfaceId)
			: base(id, typeId, name)
		{
			_children = new Dictionary<LocationID, BaseContainer>();
			_properties = new Dictionary<LocationID, PropertyBase>();
			_interfaceId = interfaceId;
		}

		public Dictionary<LocationID, BaseContainer> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public Dictionary<LocationID, PropertyBase> Properties
		{
			get { return _properties; }
			set { _properties = value; }
		}

		/// <summary>
		/// Gets or sets the interface identifier.   TODO: Expand upon this idea
		/// </summary>
		/// <value>
		/// The interface identifier.
		/// </value>
		public ulong InterfaceId { get; set; }

		/*public PropertyBase this [string propertyName] {
			get {

			}
			set {
			}
		}*/

		public PropertyBase this [LocationID id] {
			get 
			{
				LocationIdIterator iterator = new LocationIdIterator(id);
				PropertyBase property = FindProperty(iterator);
				if (property != null)
				{
					return property;
				}
				throw new KeyNotFoundException(string.Format("The property with Id {0} cannot be found",id.ToIdTypeString()));
			}
			set {
				LocationIdIterator iterator = new LocationIdIterator(id);
				PropertyBase property = FindProperty(iterator);
				if (property != null)
				{
					property.SetValue(value);
				}
			}
		}



		/// <summary>
		/// Adds the property.
		/// </summary>
		/// <returns>
		/// The property.
		/// </returns>
		/// <param name='newProp'>
		/// If set to <c>true</c> new property.
		/// </param>
		public bool AddProperty(PropertyBase newProp)
		{
			if (newProp == null) return false;

			if (_properties.ContainsKey(newProp.locationId)) return false;

			newProp.SetParent(this._locId);
			_properties.Add(newProp.locationId, newProp);
            newProp.PropertyChanged+=new PropertyChangedEventHandler(ChildOrPropertyChanged);
            RaisePropertyChanged(() => Properties);
			return true;
		}

		public bool AddChild(BaseContainer newKid)
		{
			if (newKid == null) return false;

			if (_children.ContainsKey(newKid.locationId)) return false;

			newKid.SetParent(this._locId);
			_children.Add(newKid.locationId, newKid);
            newKid.PropertyChanged += new PropertyChangedEventHandler(ChildOrPropertyChanged);
            RaisePropertyChanged(() => Children);
            return true;
		}

		public void RemoveChild(LocationID id)
		{
			if (_children.ContainsKey(id))
			{
				_children.Remove(id);
                RaisePropertyChanged(() => Children);
            }
		}

		public void RemoveProperty(LocationID id)
		{
			if (_properties.ContainsKey(id))
			{
				_properties.Remove(id);
                RaisePropertyChanged(() => Properties);
            }
		}

		public PropertyBase Property(LocationID id)
		{
			LocationIdIterator iterator = new LocationIdIterator(id);
			return FindProperty(iterator);
		}

		public BaseContainer Child(LocationID id)
		{
			LocationIdIterator iterator = new LocationIdIterator(id);
			return FindChild(iterator);
		}

		public bool SetProperty(LocationID id, object value)
		{
			LocationIdIterator iterator = new LocationIdIterator(id);
			PropertyBase property = FindProperty(iterator);
			if (property != null)
			{
				return property.SetValue(value);
			}
			return false;
		}

		protected PropertyBase FindProperty(LocationIdIterator iterator)
		{
			if (this.locationId == iterator.CurrentLocation)
			{
				iterator.MoveNext(); // move to the property location or the prop location
				if (iterator.IsDone())
				{
					if (_properties.ContainsKey(iterator.CurrentLocation))
					{
						return _properties[iterator.CurrentLocation];
					}
				}
				else
				{
					if (_children.ContainsKey(iterator.CurrentLocation))
					{
						return _children[iterator.CurrentLocation].FindProperty(iterator);
					}
				}
			}
			return null;
		}

		protected BaseContainer FindChild(LocationIdIterator iterator)
		{
			if (this.locationId == iterator.CurrentLocation)
			{
				iterator.MoveNext(); // move to the property location or the prop location
				if (iterator.IsDone())
				{
					if (_children.ContainsKey(iterator.CurrentLocation))
					{
						return _children[iterator.CurrentLocation];
					}
				}
				else
				{
					if (_children.ContainsKey(iterator.CurrentLocation))
					{
						return _children[iterator.CurrentLocation].FindChild(iterator);
					}
				}
			}
			return null;
		}

		public override void SetParent(LocationID parent)
		{
			this._locId.SetParent(parent);
			_properties = ReparentDictionary<PropertyBase>(_properties, this._locId);
			_children = ReparentDictionary<BaseContainer>(_children, this._locId);
		}

		private Dictionary<LocationID, T> ReparentDictionary<T>(Dictionary<LocationID, T> dictionary, LocationID newParent)
			where T : IdType
		{
			Dictionary<LocationID, T> reParentedProperties = new Dictionary<LocationID, T>();
			foreach (KeyValuePair<LocationID, T> kvp in dictionary)
			{
				kvp.Value.SetParent(this._locId);
				reParentedProperties.Add(kvp.Value.locationId, kvp.Value);
			}
			dictionary.Clear();
			return reParentedProperties;
		}

		public override ushort MetaDataType { get { return (ushort)MetadataEnum.BaseContainer; ; } }

		public override ushort IntegralType { get { return (ushort)ClrIntegralType.None; } }

		public IEnumerable<PropertyBase> WalkPropertyTree ()
		{
			var propertyPairs = from propertyPair in _properties
				orderby propertyPair.Key ascending
					select propertyPair.Value;

			foreach (PropertyBase property in propertyPairs) {
				yield return property;
			}

			IEnumerable<BaseContainer> child = from pair in _children
				orderby pair.Key ascending
					select pair.Value;

			foreach (BaseContainer c in child) 
			{
				foreach(PropertyBase p in c.WalkPropertyTree())
				{
					yield return p;
				}
			}
		}

		public IEnumerable<IdType> WalkTree ()
		{
			yield return this;

			var propertyPairs = from propertyPair in _properties
				orderby propertyPair.Key ascending
					select propertyPair.Value;

			foreach (PropertyBase property in propertyPairs) {
				yield return property;
			}

			IEnumerable<BaseContainer> child = from pair in _children
				orderby pair.Key ascending
					select pair.Value;

			foreach (BaseContainer c in child) 
			{
				yield return c;

				foreach(PropertyBase p in c.WalkPropertyTree())
				{
					yield return p;
				}
			}
		}

		public override byte[] ToByteArray()
		{
			MemoryStream ms = new MemoryStream();

			byte[] baseData = base.ToByteArray();
			ms.Write(baseData,0,baseData.Length);
			byte [] Iid = BytesProvider<ulong>.Default.GetBytes(InterfaceId);
			ms.Write(Iid,0,Iid.Length);

			// iterate over children objects and write them to the
			// stream
			var children = from pair in _children
			orderby pair.Key ascending
			select pair;
			foreach (KeyValuePair<LocationID,BaseContainer> child in children)
			{
				byte[] data = MetaDataPayload.ToByteArray(child.Value);

				ms.Write(data, 0, data.Length);
			}
			// iterate over properties of this object and write them
			// to the stream
			var properties = from pair in _properties
						orderby pair.Key ascending
						select pair;
			foreach (KeyValuePair<LocationID, PropertyBase> prop in properties)
			{
				byte[] data = MetaDataPayload.ToByteArray(prop.Value);
				ms.Write(data, 0, data.Length);
			}
			
			// write an end of container marker so we know when we are done
			ms.Write(EndOfContainer, 0, EndOfContainer.Length);

			return ms.ToArray();
		}

		public override int FromByteArray(byte[] data, int offset)
		{
			int curPos = base.FromByteArray(data, offset);

			// deserialize the interface id of the contianer
            InterfaceId = ByteArryTypeProvider<ulong>.Default.Convert(data, curPos);
            curPos += BareMetal.SizeOf<ulong>();

			// deserailize the children and properties
			while (curPos < data.Length && !EndOfContainerCheck(data, curPos))
			{
				Tuple<IdType, int> mdObjOffset = MetaDataPayload.FromByteArray(data, curPos);
				curPos = mdObjOffset.Item2;

				BaseContainer container = mdObjOffset.Item1 as BaseContainer;
				PropertyBase property = mdObjOffset.Item1 as PropertyBase;
				if (container != null)
				{
					AddChild(container);
				}
				else if (property != null)
				{
					AddProperty(property);
				}
				else
				{
					if(mdObjOffset.Item1 != null)
					{
						IdType obj = mdObjOffset.Item1;
						throw new InvalidDataException(string.Format("Unknown metadata object id: {0} type: {1} name: {2}", obj.Id, obj.TypeId, obj.Name));
					}
				}
			}
            // add one to get past the end of container marker
			return curPos + 1;
		}

		private bool EndOfContainerCheck(byte[] data, int offset)
		{
			if (data.Length >= offset)
			{
				if (data[offset] == EndOfContainer[0])
				{
					return true;
				}
			}
			return false;
		}

        private void ChildOrPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyBase property = sender as PropertyBase;
            if (property != null)
            {
                OnPropertyChanged(property.locationId, "Properties");
            }
            else
            {
                BaseContainer container = sender as BaseContainer;
                if (container != null)
                    OnPropertyChanged(container.locationId, "Children");
                else
                {
                    Debug.WriteLine(sender.GetType().ToString());
                    OnPropertyChanged(e.PropertyName);
                }
            }
        }
	}
}
