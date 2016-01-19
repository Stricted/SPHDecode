using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPHDecode.Implementations
{
    public class SuperViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(property, value))
                return;

            property = value;
            onPropertyChanged(propertyName);
        }

        protected void onPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (object.Equals(PropertyChanged, null))
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

