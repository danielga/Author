using Author.OTP;
using Author.UI.ViewModels;
using Author.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class EntryPage : ContentPage
    {
        EntryPageViewModel _entryPageVM = ViewModelLocator.EntryPageVM;

        public EntryPage()
        {
            InitializeComponent();

            _entryPageVM.PropertyChanged += OnVMPropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _entryPageVM.Reset();
        }

        void OnAcceptTapped(object sender, EventArgs args)
        {
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

        public void LockLengthPicker(byte value = 6)
        {
            LengthPicker.IsEnabled = false;
            _entryPageVM.Length = value;
        }

        public void UnlockLengthPicker()
        {
            LengthPicker.IsEnabled = true;
            _entryPageVM.Length = 6;
        }

        public void LockPeriodSlider(byte value = 30)
        {
            PeriodSlider.IsEnabled = false;
            _entryPageVM.Period = value;
        }

        public void UnlockPeriodSlider()
        {
            PeriodSlider.IsEnabled = true;
            _entryPageVM.Period = 30;
        }

        void OnVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
                Factory.SetupEntryPage(_entryPageVM.Type, this);
        }
    }
}
