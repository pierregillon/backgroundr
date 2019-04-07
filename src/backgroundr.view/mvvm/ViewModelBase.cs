using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace backgroundr.view.mvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual T GetNotifiableProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (_properties.ContainsKey(propertyName)) {
                return (T) _properties[propertyName];
            }
            return default(T);
        }
        protected virtual void SetNotifiableProperty<T>(object value, [CallerMemberName] string propertyName = null)
        {
            if (_properties.ContainsKey(propertyName) == false) {
                _properties.Add(propertyName, value);
            }
            else {
                _properties[propertyName] = value;
            }
            OnPropertyChanged(propertyName);
        }
    }
}