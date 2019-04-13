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

        public ObservableCollection<Entry> EntriesList => _entryManager.Entries;

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
        public Command ItemTappedCommand { get; private set; }

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

                int index = EntriesList.IndexOf(msg.Entry);
                EntriesList[index] = msg.Entry;
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
            ItemTappedCommand = new Command(OnItemTapped);

#if DEBUG
            // Xamarin.Forms Previewer data
            if (EntriesList.Count != 0)
                return;

            const string Chars = Base32.ValidCharacters;

            Random random = new Random();
            long timestamp = Time.GetCurrent();
            for (int i = 0; i < 5; ++i)
            {
                Entry entry = new Entry(new Secret
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
#endif
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
            _entryManager.OnEntryAppearing((Entry)e.Item);
        }

        // This event is triggered when we navigate to another page
        void OnItemDisappearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            _entryManager.OnEntryDisappearing((Entry)e.Item);
        }

        void OnItemEdit(object context)
        {
            Entry entry = (Entry)context;

            SetPage(_entryPage);

            _entryPageVM.Entry = entry;
        }

        void OnItemDelete(object context)
        {
            Entry entry = (Entry)context;

            EntriesList.Remove(entry);

            Acr.UserDialogs.UserDialogs.Instance.Toast(
                new Acr.UserDialogs.ToastConfig("Deleted entry")
                .SetDuration(TimeSpan.FromSeconds(3))
                .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
        }

        async void OnItemTapped(object context)
        {
            ItemTappedEventArgs args = (ItemTappedEventArgs)context;
            Entry entry = (Entry)args.Item;
            try
            {
                await Xamarin.Essentials.Clipboard.SetTextAsync(entry.Code);
                Acr.UserDialogs.UserDialogs.Instance.Toast(
                    new Acr.UserDialogs.ToastConfig("Copied OTP")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
            }
            catch (NotImplementedException)
            {}

        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
