using Author.UI.ViewModels;
using System;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnResume()
        {

        }

        protected override void OnSleep()
        {

        }

        public void HandleUriScheme(Uri uri)
        {
            NavigationPage navPage = MainPage as NavigationPage;
            MainPageViewModel viewModel = navPage?.CurrentPage.BindingContext as MainPageViewModel;
            viewModel?.SetAddEntryPageAsMainPage();
        }
    }
}
