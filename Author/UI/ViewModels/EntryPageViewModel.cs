using System.ComponentModel;
using System.Runtime.CompilerServices;
using Author.OTP;
using System.Diagnostics;

namespace Author.UI.ViewModels
{
    public class EntryPageViewModel : INotifyPropertyChanged
    {
        Entry _entry = null;
        public Entry Entry
        {
            get { return _entry; }

            set
            {
                _entry = value;

                if (value != null)
                {
                    Type = OTP.Type.FromName[value.Type];
                    Name = value.Name;
                    Length = value.Digits;
                    Period = value.Period;
                    Secret = value.SecretData;
                }
            }
        }

        int _type = OTP.Type.Time;
        public int Type
        {
            get { return _type; }

            set
            {
                _type = value;
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

        public byte Length
        {
            get { return (byte)(LengthIndex + PasswordLengthDifference); }

            set
            {
                Debug.Assert(value >= 4 && value <= 8, "OTP length is invalid");
                LengthIndex = value - PasswordLengthDifference;
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

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            Entry = null;
            Type = OTP.Type.Time;
            Name = "";
            Secret = "";
            Length = 6;
            Period = 30;
        }
    }
}
