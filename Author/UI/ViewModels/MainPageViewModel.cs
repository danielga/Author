using Author.OTP;
using Author.UI.Pages;
using Author.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        readonly EntryManager _entryManager = new EntryManager();

        readonly EntryPage _entryPage = new EntryPage();
        readonly SettingsPage _settingsPage = new SettingsPage();
        readonly AboutPage _aboutPage = new AboutPage();

        public MainPage Page = null;

        ObservableCollection<OTP.Entry> _entriesList = new ObservableCollection<OTP.Entry>();
        public ObservableCollection<OTP.Entry> EntriesList
        {
            get
            {
                return _entriesList;
            }

            set
            {
                bool changed = _entriesList != value;

                _entriesList = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        public object SelectedItem
        {
            get
            {
                return null;
            }

            set
            {
                OnPropertyChanged();
            }
        }

        public Command AppearingCommand { get; private set; }
        public Command DisappearingCommand { get; private set; }
        public Command AddCommand { get; private set; }
        public Command SettingsCommand { get; private set; }
        public Command AboutCommand { get; private set; }
        public Command ItemAppearingCommand { get; private set; }
        public Command ItemDisappearingCommand { get; private set; }
        public Command ItemEditCommand { get; private set; }
        public Command ItemDeleteCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            AppearingCommand = new Command(new Action(OnAppearing));
            DisappearingCommand = new Command(new Action(OnDisappearing));
            AddCommand = new Command(new Action(OnAddTapped));
            SettingsCommand = new Command(new Action(OnSettingsTapped));
            AboutCommand = new Command(new Action(OnAboutTapped));
            ItemAppearingCommand = new Command(new Action<object>(OnItemAppearing));
            ItemDisappearingCommand = new Command(new Action<object>(OnItemDisappearing));
            ItemEditCommand = new Command(new Action<object>(OnItemEdit));
            ItemDeleteCommand = new Command(new Action<object>(OnItemDelete));

            // Xamarin.Forms Previewer data
            const string Chars = Base32.ValidCharacters;

            Random random = new Random();
            long timestamp = Time.GetCurrent();
            for (int i = 0; i < 5; ++i)
            {
                OTP.Entry entry = new OTP.Entry(new Secret
                {
                    Type = OTP.Type.Time,
                    Name = "Hello world " + i,
                    Digits = (byte)(4 + i),
                    Period = (byte)(30 + (i - 2) * 5),
                    Data = new string(Enumerable.Repeat(Chars, 32).Select(s => s[random.Next(s.Length)]).ToArray())
                });
                _entriesList.Add(entry);
                entry.UpdateCode(timestamp);
            }
        }

        void OnAppearing()
        {
            _entryManager.EnableUpdate();
        }

        void OnDisappearing()
        {
            _entryManager.DisableUpdate();
        }

        void SetPage(Page page)
        {
            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PushAsync(page);
        }

        void OnAddTapped()
        {
            ViewModelLocator.EntryPageVM.Title = "Add OTP entry";

            SetPage(_entryPage);
        }

        void OnSettingsTapped()
        {
            _settingsPage.BindingContext = ViewModelLocator.SettingsPageVM;
            SetPage(_settingsPage);
        }

        void OnAboutTapped()
        {
            SetPage(_aboutPage);
        }

        // This event is not triggered when we go back to this page
        void OnItemAppearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            _entryManager.OnEntryAppearing((OTP.Entry)e.Item);
        }

        // This event is triggered when we navigate to another page
        void OnItemDisappearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            _entryManager.OnEntryDisappearing((OTP.Entry)e.Item);
        }

        void OnItemEdit(object context)
        {
            OTP.Entry entry = (OTP.Entry)context;

            ViewModelLocator.EntryPageVM.Title = "Edit OTP entry";
            ViewModelLocator.EntryPageVM.Entry = entry;

            SetPage(_entryPage);
        }

        void OnItemDelete(object context)
        {
            OTP.Entry entry = (OTP.Entry)context;
            ObservableCollection<OTP.Entry> entriesList =
                ViewModelLocator.MainPageVM.EntriesList;

            entriesList.Remove(entry);

            Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Deleted entry")
                .SetDuration(TimeSpan.FromSeconds(3))
                .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
