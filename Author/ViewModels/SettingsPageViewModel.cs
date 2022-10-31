namespace Author.ViewModels;

public class SettingsPageViewModel
{
    public Command AcceptCommand { get; private set; }

    public SettingsPageViewModel()
    {
        AcceptCommand = new Command(OnAcceptTapped);
    }

    void OnAcceptTapped()
    {
        // Save settings

        try
        {
            Toast.Create("Saved settings")
                .SetDuration(ToastDuration.Long)
                .Show();
        }
        catch
        { }
    }
}
