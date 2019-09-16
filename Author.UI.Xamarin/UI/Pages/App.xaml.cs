﻿using Author.UI.ViewModels;
using System;
using System.IO;
using Author.OTP;
using Xamarin.Essentials;
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

        public void OnUriRequestReceived(Uri uri)
        {
            NavigationPage navPage = MainPage as NavigationPage;
            MainPageViewModel viewModel = navPage?.CurrentPage.BindingContext as MainPageViewModel;
            if (viewModel == null)
            {
                return;
            }

            try
            {
                Secret secret = Secret.Parse(uri.AbsoluteUri);
                viewModel.SetAddEntryPageAsMainPage(new Entry(secret));
            }
            catch (Exception)
            { }
        }

        public void OnFileRequestReceived(Uri path)
        {
            NavigationPage navPage = MainPage as NavigationPage;
            MainPageViewModel viewModel = navPage?.CurrentPage.BindingContext as MainPageViewModel;
            if (viewModel == null)
            {
                return;
            }

            try
            {
                using (StreamReader reader = File.OpenText(path.AbsolutePath))
                {
                    viewModel.ImportStreamAsync(reader);
                }
            }
            catch (Exception)
            { }
        }
    }
}
