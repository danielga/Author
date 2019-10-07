using Author.OTP;
using Author.UI.Messages;
using Author.UI.Pages;
using Author.Utility;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class MainPageViewModel
    {
        private static readonly EntryPage _entryPage = new EntryPage();
        private static readonly EntryPageViewModel _entryPageVM;
        private static readonly SettingsPage _settingsPage = new SettingsPage();
        private static readonly AboutPage _aboutPage = new AboutPage();

        public MainPage Page = null;

        public EntryManager EntriesManager { get; } = new EntryManager();

        public Command AppearingCommand { get; }
        public Command DisappearingCommand { get; }
        public Command AddCommand { get; }
        public Command ImportCommand { get; }
        public Command ExportCommand { get; }
        public Command SettingsCommand { get; }
        public Command AboutCommand { get; }
        public Command ItemAppearingCommand { get; }
        public Command ItemDisappearingCommand { get; }
        public Command ItemTappedCommand { get; }

        static MainPageViewModel()
        {
            _entryPageVM = (EntryPageViewModel)_entryPage.BindingContext;
        }

        public MainPageViewModel()
        {
            MessagingCenter.Subscribe<AddEntry>(this, "AddEntry", msg =>
            {
                EntriesManager.Entries.Add(msg.Entry);
                GoToPreviousPage();
            });

            MessagingCenter.Subscribe<DeleteEntry>(this, "DeleteEntry", msg => OnItemDelete(msg.Entry));

            MessagingCenter.Subscribe<EditEntry>(this, "EditEntry", msg => GoToPreviousPage());

            MessagingCenter.Subscribe<RequestEditEntry>(this, "RequestEditEntry", msg => OnItemEdit(msg.Entry));

            AppearingCommand = new Command(OnAppearing);
            DisappearingCommand = new Command(OnDisappearing);
            AddCommand = new Command(OnAddTapped);
            ImportCommand = new Command(OnImportTapped);
            ExportCommand = new Command(OnExportTapped);
            SettingsCommand = new Command(OnSettingsTapped);
            AboutCommand = new Command(OnAboutTapped);
            ItemAppearingCommand = new Command(OnItemAppearing);
            ItemDisappearingCommand = new Command(OnItemDisappearing);
            ItemTappedCommand = new Command(OnItemTapped);
        }

        private void OnAppearing()
        {
            EntriesManager.EnableUpdate();
        }

        private void OnDisappearing()
        {
            EntriesManager.DisableUpdate();
        }

        private void GoToPreviousPage()
        {
            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PopAsync();
        }

        private void SetPage(Page page)
        {
            NavigationPage navPage = (NavigationPage)Page?.Parent;
            navPage?.PushAsync(page);
        }

        public void SetAddEntryPageAsMainPage(MainPageEntryViewModel entry = null)
        {
            _entryPageVM.AddEntry(entry);
            if (Device.RuntimePlatform == Device.macOS ||
                Device.RuntimePlatform == Device.UWP)
            {
                SetPage(_entryPage);
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(_entryPage);
            }
        }

        private void OnAddTapped()
        {
            _entryPageVM.AddEntry();
            SetPage(_entryPage);
        }

        private async void OnImportTapped()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                {
                    return;
                }

                using (Stream stream = fileData.GetStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        await ImportStreamAsync(reader);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public async Task ImportStreamAsync(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                try
                {
                    Secret secret = Secret.Parse(await reader.ReadLineAsync());
                    EntriesManager.Entries.Add(new MainPageEntryViewModel(secret));
                }
                catch
                {
                    // ignored
                }
            }
        }

        private async void OnExportTapped()
        {
            try
            {
                string path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "export.otp");

                using (StreamWriter fileWriter = File.CreateText(path))
                {
                    foreach (MainPageEntryViewModel entry in EntriesManager.Entries)
                    {
                        await fileWriter.WriteLineAsync(entry.Secret.ToString());
                    }
                }

                await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareFileRequest
                {
                    Title = "Author OTP export",
                    File = new Xamarin.Essentials.ShareFile(path)
                });
            }
            catch
            {
                // ignored
            }
        }

        private void OnSettingsTapped()
        {
            SetPage(_settingsPage);
        }

        private void OnAboutTapped()
        {
            SetPage(_aboutPage);
        }

        // This event is not triggered when we go back to this page
        private void OnItemAppearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            EntriesManager.OnEntryAppearing((MainPageEntryViewModel)e.Item);
        }

        // This event is triggered when we navigate to another page
        private void OnItemDisappearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            EntriesManager.OnEntryDisappearing((MainPageEntryViewModel)e.Item);
        }

        private void OnItemEdit(object context)
        {
            _entryPageVM.EditEntry((MainPageEntryViewModel)context);
            SetPage(_entryPage);
        }

        private void OnItemDelete(object context)
        {
            EntriesManager.Entries.Remove((MainPageEntryViewModel)context);

            try
            {
                Notification.Create("Deleted entry")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Notification.Position.Bottom)
                    .Show();
            }
            catch
            { }
        }

        private async void OnItemTapped(object context)
        {
            ItemTappedEventArgs args = (ItemTappedEventArgs)context;
            MainPageEntryViewModel entry = (MainPageEntryViewModel)args.Item;
            try
            {
                await Clipboard.SetTextAsync(entry.Secret.Code);
                Notification.Create("Copied OTP")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Notification.Position.Bottom)
                    .Show();
            }
            catch
            { }
        }
    }
}
