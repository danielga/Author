using Author.OTP;
using Author.UI.Messages;
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
        static readonly EntryManager _entryManager = new EntryManager();

        static readonly EntryPage _entryPage = new EntryPage();
        static readonly EntryPageViewModel _entryPageVM = null;
        static readonly SettingsPage _settingsPage = new SettingsPage();
        static readonly AboutPage _aboutPage = new AboutPage();

        public MainPage Page = null;

        public ObservableCollection<OTP.Entry> EntriesList => _entryManager.Entries;

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

        static MainPageViewModel()
        {
            _entryPageVM = (EntryPageViewModel)_entryPage.BindingContext;
        }

        public MainPageViewModel()
        {
            MessagingCenter.Subscribe<AddEntry>(this, "AddEntry", (msg) =>
            {
                EntriesList.Add(msg.Entry);
                GoToPreviousPage();
            });

            MessagingCenter.Subscribe<DeleteEntry>(this, "DeleteEntry", (msg) => OnItemDelete(msg.Entry));

            MessagingCenter.Subscribe<EditEntry>(this, "EditEntry", (msg) =>
            {
                GoToPreviousPage();

                if (Device.RuntimePlatform == Device.UWP)
                {
                    // Possible race condition on Xamarin.Forms?
                    // Bug 58028 - ListView cell replacement when unfocused results in a blank space
                    // https://bugzilla.xamarin.com/show_bug.cgi?id=58028
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        int index = EntriesList.IndexOf(msg.Entry);
                        EntriesList[index] = msg.Entry;
                    });
                }
                else
                {
                    int index = EntriesList.IndexOf(msg.Entry);
                    EntriesList[index] = msg.Entry;
                }
            });

            MessagingCenter.Subscribe<RequestEditEntry>(this, "RequestEditEntry", (msg) => OnItemEdit(msg.Entry));

            AppearingCommand = new Command(OnAppearing);
            DisappearingCommand = new Command(OnDisappearing);
            AddCommand = new Command(OnAddTapped);
            SettingsCommand = new Command(OnSettingsTapped);
            AboutCommand = new Command(OnAboutTapped);
            ItemAppearingCommand = new Command(OnItemAppearing);
            ItemDisappearingCommand = new Command(OnItemDisappearing);
            ItemEditCommand = new Command(OnItemEdit);
            ItemDeleteCommand = new Command(OnItemDelete);

            // Xamarin.Forms Previewer data
            if (EntriesList.Count != 0)
                return;

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
                    Period = (byte)(5 + i),
                    Data = new string(Enumerable.Repeat(Chars, 32).Select(s => s[random.Next(s.Length)]).ToArray())
                });
                EntriesList.Add(entry);
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

        void GoToPreviousPage()
        {
            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PopAsync();
        }

        void SetPage(Page page)
        {
            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PushAsync(page);
        }

        void OnAddTapped()
        {
            SetPage(_entryPage);

            _entryPageVM.Entry = null;
        }

        void OnSettingsTapped()
        {
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

            SetPage(_entryPage);

            _entryPageVM.Entry = entry;
        }

        void OnItemDelete(object context)
        {
            OTP.Entry entry = (OTP.Entry)context;

            EntriesList.Remove(entry);

            Acr.UserDialogs.UserDialogs.Instance.Toast(
                new Acr.UserDialogs.ToastConfig("Deleted entry")
                .SetDuration(TimeSpan.FromSeconds(3))
                .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));

            UWPFix();
        }

        // This is required because the UWP platform on Windows seems to have a bug on ListView
        // which causes deletion, addition and deletion of the previous addition to call
        // context actions with the wrong binding context (or just wrong view cell)
        // Bug 57982 - ListView context actions called with the wrong binding context
        // https://bugzilla.xamarin.com/show_bug.cgi?id=57982
        void UWPFix()
        {
            if (Device.RuntimePlatform != Device.UWP)
                return;

            MessagingCenter.Unsubscribe<AddEntry>(this, "AddEntry");
            MessagingCenter.Unsubscribe<DeleteEntry>(this, "DeleteEntry");
            MessagingCenter.Unsubscribe<EditEntry>(this, "EditEntry");
            MessagingCenter.Unsubscribe<RequestEditEntry>(this, "RequestEditEntry");

            Page.Content = null;
            Page = new MainPage();
            Application.Current.MainPage = new NavigationPage(Page);
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
