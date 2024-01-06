using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MDModeling
{

    public class MetadataChangedEventArgs : PropertyChangedEventArgs
    {
        public MetadataChangedEventArgs(LocationID locId, string name):
            base(name)
        {
            Location = locId;
        }
        public LocationID Location { get; set; }
    }

    /// <summary>
    /// This class is a base class for implementing INotifyPropertyChanged.
    /// With this class you can use it on a model as well as a view model.
    /// </summary>
    public abstract class ModelNotifyChanged : INotifyPropertyChanged
    {
        #region Private Variables

        #endregion Private Variables

        #region Public Events

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Constructor / Dispose

        /// <summary>
        /// The default constructor
        /// </summary>
        public ModelNotifyChanged()
        {
        }

        #endregion Constructor / Dispose

        #region Properties

        #endregion Properties

        #region Public Methods

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion Debugging Aides

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged(LocationID locId, string propertyName)
        {
            PropertyChangedEventArgs eventArgs = null;
            if (locId != null)
            {
                eventArgs = new MetadataChangedEventArgs(locId, propertyName);
            }
            else
            {
                eventArgs = new PropertyChangedEventArgs(propertyName);
            }

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }


        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            LocationID locId = null;
            OnPropertyChanged(locId, propertyName);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            this.OnPropertyChanged(propertyName);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        #endregion INotifyPropertyChanged Members

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

        #region enums

        #endregion enums
    }
}