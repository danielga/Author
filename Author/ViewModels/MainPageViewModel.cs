using Author.Messages;
using Author.OTP;
using Author.Views;

namespace Author.ViewModels;

public class MainPageViewModel
{
    private static readonly EntryPage _entryPage = new();
    private static readonly EntryPageViewModel _entryPageVM;
    private static readonly SettingsPage _settingsPage = new();
    private static readonly AboutPage _aboutPage = new();

    public MainPage? Page = null;

    public EntryManager EntriesManager { get; } = new();

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

        AppearingCommand = new(OnAppearing);
        DisappearingCommand = new(OnDisappearing);
        AddCommand = new(OnAddTapped);
        ImportCommand = new(OnImportTapped);
        ExportCommand = new(OnExportTapped);
        SettingsCommand = new(OnSettingsTapped);
        AboutCommand = new(OnAboutTapped);
        ItemAppearingCommand = new(OnItemAppearing);
        ItemDisappearingCommand = new(OnItemDisappearing);
        ItemTappedCommand = new(OnItemTapped);
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
        var shell = Application.Current?.MainPage as AppShell;
        shell?.Navigation.PopAsync();
    }

    private void SetPage(Page page)
    {
        var shell = Application.Current?.MainPage as AppShell;
        shell?.Navigation.PushAsync(page);
    }

    public void SetAddEntryPageAsMainPage(MainPageEntryViewModel entry)
    {
        _entryPageVM.AddEntry(entry);
        SetPage(_entryPage);
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
            var fileData = await FilePicker.PickAsync();
            if (fileData == null)
                return;

            using var stream = await fileData.OpenReadAsync();
            using var reader = new StreamReader(stream);
            await ImportStreamAsync(reader);
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
            var path = Path.Combine(FileSystem.CacheDirectory, "export.otp");

            using (var fileWriter = File.CreateText(path))
            {
                foreach (MainPageEntryViewModel entry in EntriesManager.Entries)
                    await fileWriter.WriteLineAsync(entry.Secret.ToString());
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Author OTP export",
                File = new(path)
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
            Toast.Create("Deleted entry")
                .SetDuration(ToastDuration.Long)
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
            await Utility.Clipboard.SetTextAsync(entry.Secret.Code);
            Toast.Create("Copied OTP")
                .SetDuration(ToastDuration.Long)
                .Show();
        }
        catch
        { }
    }
}
