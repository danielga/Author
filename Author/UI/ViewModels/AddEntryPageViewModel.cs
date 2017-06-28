using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Author.UI.ViewModels
{
    public class AddEntryPageViewModel : INotifyPropertyChanged
    {
        int _typeIndex = 1;
        public int TypeIndex
        {
            get { return _typeIndex; }

            set
            {
                _typeIndex = value;
                OnPropertyChanged();
            }
        }

        string _name = "";
        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        string _secret = "";
        public string Secret
        {
            get { return _secret; }

            set
            {
                _secret = value;
                OnPropertyChanged();
            }
        }

        public const int PasswordLengthDifference = 4;
        int _lengthIndex = 6 - PasswordLengthDifference;
        public int LengthIndex
        {
            get { return _lengthIndex; }

            set
            {
                _lengthIndex = value;
                OnPropertyChanged();
            }
        }

        double _period = 30;
        public double Period
        {
            get { return _period; }

            set
            {
                _period = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            TypeIndex = 1;
            Name = "";
            Secret = "";
            LengthIndex = 6 - PasswordLengthDifference;
            Period = 30;
        }
    }
}
