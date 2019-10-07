using Author.OTP;
using Author.UI.Messages;
using Author.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class EntryPageViewModel : INotifyPropertyChanged
    {
        private bool _addingEntry = false;

        private MainPageEntryViewModel _entry = null;
        public MainPageEntryViewModel Entry
        {
            get => _entry;

            private set
            {
                if (value != _entry)
                {
                    if (_entry != null)
                    {
                        _entry.Secret.PropertyChanged -= OnTypeChanged;
                    }

                    if (value != null)
                    {
                        value.Secret.PropertyChanged += OnTypeChanged;
                    }

                    _entry = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private string _title = "Add OTP entry";
        public string Title
        {
            get => _title;

            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _lengthPickerEnabled = true;
        public bool LengthPickerEnabled
        {
            get => _lengthPickerEnabled;

            set
            {
                if (value != _lengthPickerEnabled)
                {
                    _lengthPickerEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _periodSliderEnabled = true;
        public bool PeriodSliderEnabled
        {
            get => _periodSliderEnabled;

            set
            {
                if (value != _periodSliderEnabled)
                {
                    _periodSliderEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command DisappearingCommand { get; private set; }
        public Command AcceptCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EntryPageViewModel()
        {
            DisappearingCommand = new Command(OnDisappearing);
            AcceptCommand = new Command(OnAcceptTapped);
        }

        private void OnDisappearing()
        {
            _entry = null;
        }

        private void OnAcceptTapped()
        {
            if (string.IsNullOrEmpty(Entry.Secret.Name) ||
                string.IsNullOrEmpty(Entry.Secret.Data))
            {
                try
                {
                    Notification.Create("Detected invalid properties for the entry")
                        .SetDuration(TimeSpan.FromSeconds(3))
                        .SetPosition(Notification.Position.Bottom)
                        .Show();
                }
                catch
                { }

                return;
            }

            if (_addingEntry)
            {
                MessagingCenter.Send(new AddEntry { Entry = Entry }, "AddEntry");

                try
                {
                    Notification.Create("Added new entry")
                        .SetDuration(TimeSpan.FromSeconds(3))
                        .SetPosition(Notification.Position.Bottom)
                        .Show();
                }
                catch
                { }
            }
            else
            {
                MainPageEntryViewModel entry = Entry;
                entry.UpdateCode(Time.GetCurrent(), true);

                MessagingCenter.Send(new EditEntry { Entry = entry }, "EditEntry");

                try
                {
                    Notification.Create("Saved edited entry")
                        .SetDuration(TimeSpan.FromSeconds(3))
                        .SetPosition(Notification.Position.Bottom)
                        .Show();
                }
                catch
                { }
            }
        }

        public void AddEntry(MainPageEntryViewModel entry = null)
        {
            Title = "Add OTP entry";
            Entry = entry ?? new MainPageEntryViewModel();
            _addingEntry = true;
        }

        public void EditEntry(MainPageEntryViewModel entry)
        {
            Title = "Edit OTP entry";
            Entry = entry;
            _addingEntry = false;
        }

        private void OnTypeChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Type")
            {
                Factory.SetupEntryPage(((Secret)sender).Type, this);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
