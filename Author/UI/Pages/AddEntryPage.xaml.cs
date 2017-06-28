using Author.OTP;
using Author.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class AddEntryPage : ContentPage
    {
        AddEntryPageViewModel _addEntryPageVM = ViewModelLocator.AddEntryPageVM;

        string[] _typeMap =
        {
            "hash",
            "time",
            "steam",
            "blizzard",
            "authy"
        };

        public AddEntryPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _addEntryPageVM.Reset();
        }

        void OnAcceptTapped(object sender, EventArgs args)
        {
            ObservableCollection<OTP.Entry> entriesList =
                ViewModelLocator.MainPageVM.EntriesList;

            entriesList.Add(new OTP.Entry(new Secret {
                Type = _typeMap[_addEntryPageVM.TypeIndex],
                Name = _addEntryPageVM.Name,
                Data = _addEntryPageVM.Secret,
                Digits = (byte)(_addEntryPageVM.LengthIndex +
                    AddEntryPageViewModel.PasswordLengthDifference),
                Period = (byte)_addEntryPageVM.Period
            }));

            NavigationPage parent = (NavigationPage)Parent;
            parent.PopAsync();
        }
    }
}
