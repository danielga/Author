using System.ComponentModel;
using System.Runtime.CompilerServices;
using Author.OTP;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using Author.Utility;
using Xamarin.Forms;
using Author.UI.Pages;

namespace Author.UI.ViewModels
{
    public class EntryPageViewModel : INotifyPropertyChanged
    {
        public EntryPage Page = null;

        OTP.Entry _entry = null;
        public OTP.Entry Entry
        {
            get { return _entry; }

            set
            {
                _entry = value;

                if (value != null)
                {
                    Type = value.Type;
                    Name = value.Name;
                    Length = value.Digits;
                    Period = value.Period;
                    Secret = value.SecretData;
                }
            }
        }

        byte _type = OTP.Type.Time;
        public byte Type
        {
            get { return _type; }

            set
            {
                bool changed = _type != value;

                _type = value;

                if (changed)
                {
                    OnPropertyChanged();
                    Factory.SetupEntryPage(value, this);
                }
            }
        }

        string _name = "";
        public string Name
        {
            get { return _name; }

            set
            {
                bool changed = _name != value;

                _name = value;

                if (changed)
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
                bool changed = _lengthIndex != value;

                _lengthIndex = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        public byte Length
        {
            get { return (byte)(LengthIndex + PasswordLengthDifference); }

            set
            {
                Debug.Assert(value >= 4 && value <= 8, "OTP length is invalid");

                bool changed = Length != value;

                LengthIndex = value - PasswordLengthDifference;

                if (changed)
                    OnPropertyChanged();
            }
        }

        double _period = 30;
        public double Period
        {
            get { return _period; }

            set
            {
                bool changed = _period != value;

                _period = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        string _secret = "";
        public string Secret
        {
            get { return _secret; }

            set
            {
                bool changed = _secret != value;

                _secret = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        string _title = "Add OTP entry";
        public string Title
        {
            get { return _title; }

            set
            {
                bool changed = _title != value;

                _title = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        bool _lengthPickerEnabled = true;
        public bool LengthPickerEnabled
        {
            get { return _lengthPickerEnabled; }

            set
            {
                bool changed = _lengthPickerEnabled != value;

                _lengthPickerEnabled = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        bool _periodSliderEnabled = true;
        public bool PeriodSliderEnabled
        {
            get { return _periodSliderEnabled; }

            set
            {
                bool changed = _periodSliderEnabled != value;

                _periodSliderEnabled = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        public Command DisappearingCommand { get; private set; }
        public Command AcceptCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EntryPageViewModel()
        {
            DisappearingCommand = new Command(new Action(OnDisappearing));
            AcceptCommand = new Command(new Action(OnAcceptTapped));
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

        void OnDisappearing()
        {
            Reset();
        }

        void OnAcceptTapped()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Secret))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Detected invalid properties for the entry")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
                return;
            }

            ObservableCollection<OTP.Entry> entriesList =
                ViewModelLocator.MainPageVM.EntriesList;

            if (Entry != null)
            {
                OTP.Entry entry = Entry;

                entry.Type = Type;
                entry.Name = Name;
                entry.Digits = Length;
                entry.Period = (byte)Period;
                entry.SecretData = Secret;
                entry.UpdateData();
                entry.UpdateCode(Time.GetCurrent(), true);

                Reset();

                Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Saved edited entry")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
            }
            else
            {
                entriesList.Add(new OTP.Entry(new Secret
                {
                    Type = Type,
                    Name = Name,
                    Data = Secret,
                    Digits = Length,
                    Period = (byte)Period
                }));

                Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Added new entry")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
            }

            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PopAsync();
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
