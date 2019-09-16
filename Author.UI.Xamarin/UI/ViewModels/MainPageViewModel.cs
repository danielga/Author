﻿using Author.OTP;
using Author.UI.Messages;
using Author.UI.Pages;
using Author.Utility;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private static readonly EntryPage _entryPage = new EntryPage();
        private static readonly EntryPageViewModel _entryPageVM = null;
        private static readonly SettingsPage _settingsPage = new SettingsPage();
        private static readonly AboutPage _aboutPage = new AboutPage();

        public MainPage Page = null;

        public EntryManager EntriesManager { get; } = new EntryManager();

        public object SelectedItem
        {
            get => null;

            set => OnPropertyChanged();
        }

        public Command AppearingCommand { get; private set; }
        public Command DisappearingCommand { get; private set; }
        public Command AddCommand { get; private set; }
        public Command ImportCommand { get; private set; }
        public Command ExportCommand { get; private set; }
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
            ItemEditCommand = new Command(OnItemEdit);
            ItemDeleteCommand = new Command(OnItemDelete);
            ItemTappedCommand = new Command(OnItemTapped);

#if DEBUG
            // Xamarin.Forms Previewer data
            if (EntriesManager.Entries.Count != 0)
            {
                return;
            }

            const string Chars = Base32.ValidCharacters;

            Random random = new Random();
            long timestamp = Time.GetCurrent();
            for (int i = 0; i < 5; ++i)
            {
                Entry entry = new Entry(new Secret
                {
                    Name = "Hello world " + i,
                    Digits = (byte)(4 + i),
                    Period = (byte)(5 + i),
                    Data = new string(Enumerable.Repeat(Chars, 32).Select(s => s[random.Next(s.Length)]).ToArray())
                });
                EntriesManager.Entries.Add(entry);
                Database.AddEntry(entry.Secret).Wait();
                entry.UpdateCode(timestamp);
            }
#endif
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

        public void SetAddEntryPageAsMainPage(Entry entry = null)
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
                        ImportStreamAsync(reader);
                    }
                }
            }
            catch (Exception)
            { }
        }

        public async void ImportStreamAsync(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                try
                {
                    Secret secret = Secret.Parse(await reader.ReadLineAsync());
                    EntriesManager.Entries.Add(new Entry(secret));
                    await Database.AddEntry(secret);
                }
                catch (Exception)
                { }
            }
        }

        private async void OnExportTapped()
        {
            try
            {
                string path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "export.otp");

                using (StreamWriter fileWriter = File.CreateText(path))
                {
                    foreach (Entry entry in EntriesManager.Entries)
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
            catch (Exception)
            { }
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
            EntriesManager.OnEntryAppearing((Entry)e.Item);
        }

        // This event is triggered when we navigate to another page
        private void OnItemDisappearing(object ev)
        {
            ItemVisibilityEventArgs e = (ItemVisibilityEventArgs)ev;
            EntriesManager.OnEntryDisappearing((Entry)e.Item);
        }

        private void OnItemEdit(object context)
        {
            _entryPageVM.EditEntry((Entry)context);
            SetPage(_entryPage);
        }

        private void OnItemDelete(object context)
        {
            EntriesManager.Entries.Remove((Entry)context);

            try
            {
                Notification.Create("Deleted entry")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Notification.Position.Bottom)
                    .Show();
            }
            catch (NotImplementedException)
            { }
        }

        private async void OnItemTapped(object context)
        {
            ItemTappedEventArgs args = (ItemTappedEventArgs)context;
            Entry entry = (Entry)args.Item;
            try
            {
                await Clipboard.SetTextAsync(entry.Secret.Code);
                Notification.Create("Copied OTP")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Notification.Position.Bottom)
                    .Show();
            }
            catch (NotImplementedException)
            { }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
