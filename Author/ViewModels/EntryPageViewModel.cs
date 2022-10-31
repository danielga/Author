using Author.Messages;
using Author.OTP;
using Author.Utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Author.ViewModels;

public class EntryPageViewModel : INotifyPropertyChanged
{
    private bool _addingEntry = false;

    private MainPageEntryViewModel? _entry;
    public MainPageEntryViewModel? Entry
    {
        get => _entry;

        private set
        {
            if (value != _entry)
            {
                if (_entry != null)
                    _entry.Secret.PropertyChanged -= OnTypeChanged;

                if (value != null)
                    value.Secret.PropertyChanged += OnTypeChanged;

                _entry = value;
                OnPropertyChanged();
            }
        }
    }

    private string _title = "Add OTP entry";
    public string Title
    {
        get => _title;

        set
        {
            if (value != _title)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _lengthPickerEnabled = true;
    public bool LengthPickerEnabled
    {
        get => _lengthPickerEnabled;

        set
        {
            if (value != _lengthPickerEnabled)
            {
                _lengthPickerEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _periodSliderEnabled = true;
    public bool PeriodSliderEnabled
    {
        get => _periodSliderEnabled;

        set
        {
            if (value != _periodSliderEnabled)
            {
                _periodSliderEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public Command DisappearingCommand { get; private set; }
    public Command AcceptCommand { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public EntryPageViewModel()
    {
        DisappearingCommand = new Command(OnDisappearing);
        AcceptCommand = new Command(OnAcceptTapped);
    }

    private void OnDisappearing()
    {
        Entry = null;
    }

    private void OnAcceptTapped()
    {
        if (Entry == null)
            return;

        if (string.IsNullOrEmpty(Entry.Secret.Name) ||
            string.IsNullOrEmpty(Entry.Secret.Data))
        {
            try
            {
                Toast.Create("Detected invalid properties for the entry")
                    .SetDuration(ToastDuration.Long)
                    .Show();
            }
            catch
            { }

            return;
        }

        if (_addingEntry)
        {
            MessagingCenter.Send(new AddEntry(Entry), "AddEntry");

            try
            {
                Toast.Create("Added new entry")
                    .SetDuration(ToastDuration.Long)
                    .Show();
            }
            catch
            { }
        }
        else
        {
            MainPageEntryViewModel entry = Entry;
            entry.UpdateCode(Time.GetCurrent(), true);

            MessagingCenter.Send(new EditEntry(entry), "EditEntry");

            try
            {
                Toast.Create("Saved edited entry")
                    .SetDuration(ToastDuration.Long)
                    .Show();
            }
            catch
            { }
        }
    }

    public void AddEntry(MainPageEntryViewModel? entry = null)
    {
        Title = "Add OTP entry";
        Entry = entry ?? new MainPageEntryViewModel();
        _addingEntry = true;
    }

    public void EditEntry(MainPageEntryViewModel entry)
    {
        Title = "Edit OTP entry";
        Entry = entry;
        _addingEntry = false;
    }

    private void OnTypeChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != "Type" || sender is not Secret secret)
            return;

        Factory.SetupEntryPage(secret.Type, this);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
