using Author.OTP;
using Author.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class MainPage : ContentPage
    {
        readonly EntryManager _entryManager = new EntryManager();

        readonly EntryPage _entryPage = new EntryPage();
        readonly SettingsPage _settingsPage = new SettingsPage();
        readonly AboutPage _aboutPage = new AboutPage();

        public MainPage()
        {
            InitializeComponent();

            _entryManager.EnableUpdate();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _entryManager.EnableUpdate();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _entryManager.DisableUpdate();
        }

        void SetPage(Page page)
        {
            NavigationPage parent = (NavigationPage)Parent;
            parent.PushAsync(page);
        }

        void OnAddTapped(object sender, EventArgs e)
        {
            SetPage(_entryPage);
        }

        void OnSettingsTapped(object sender, EventArgs e)
        {
            SetPage(_settingsPage);
        }

        void OnAboutTapped(object sender, EventArgs e)
        {
            SetPage(_aboutPage);
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        // This event is not triggered when we go back to this page
        void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            _entryManager.OnEntryAppearing((OTP.Entry)e.Item);
        }

        // This event is triggered when we navigate to another page
        void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            _entryManager.OnEntryDisappearing((OTP.Entry)e.Item);
        }

        void OnItemEdit(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            OTP.Entry entry = (OTP.Entry)menuItem.BindingContext;

            ViewModelLocator.EntryPageVM.Entry = entry;

            SetPage(_entryPage);
        }

        void OnItemDelete(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            OTP.Entry entry = (OTP.Entry)menuItem.BindingContext;
            ObservableCollection<OTP.Entry> entriesList =
                ViewModelLocator.MainPageVM.EntriesList;
            entriesList.Remove(entry);
        }
    }
}
