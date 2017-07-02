﻿using Author.OTP;
using Author.UI.ViewModels;
using Author.Utility;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class EntryPage : ContentPage
    {
        EntryPageViewModel _entryPageVM = ViewModelLocator.EntryPageVM;

        public EntryPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _entryPageVM.Reset();
        }

        void OnAcceptTapped(object sender, EventArgs args)
        {
            if (string.IsNullOrWhiteSpace(_entryPageVM.Name) ||
                string.IsNullOrWhiteSpace(_entryPageVM.Secret))
                return;
            ObservableCollection<OTP.Entry> entriesList =
                ViewModelLocator.MainPageVM.EntriesList;

            if (_entryPageVM.Entry != null)
            {
                OTP.Entry entry = _entryPageVM.Entry;

                entry.Type = _entryPageVM.Type;
                entry.Name = _entryPageVM.Name;
                entry.Digits = _entryPageVM.Length;
                entry.Period = (byte)_entryPageVM.Period;
                entry.SecretData = _entryPageVM.Secret;
                entry.UpdateData();
                entry.UpdateCode(Time.GetCurrent(), true);

                _entryPageVM.Reset();
            }
            else
                entriesList.Add(new OTP.Entry(new Secret
                {
                    Type = _entryPageVM.Type,
                    Name = _entryPageVM.Name,
                    Data = _entryPageVM.Secret,
                    Digits = _entryPageVM.Length,
                    Period = (byte)_entryPageVM.Period
                }));

            NavigationPage parent = (NavigationPage)Parent;
            parent.PopAsync();
        }
    }
}
