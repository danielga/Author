using Author.OTP;
using Author.ViewModels;

namespace Author.Views;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
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
        if ((MainPage as NavigationPage)?.CurrentPage.BindingContext is not MainPageViewModel viewModel)
            return;

        try
        {
            var secret = Secret.Parse(uri.AbsoluteUri);
            viewModel.SetAddEntryPageAsMainPage(new MainPageEntryViewModel(secret));
        }
        catch
        {
            // ignored
        }
    }

    public async void OnFileRequestReceived(Uri path)
    {
        if ((MainPage as NavigationPage)?.CurrentPage.BindingContext is not MainPageViewModel viewModel)
            return;

        try
        {
            using StreamReader reader = File.OpenText(path.AbsolutePath);
            await viewModel.ImportStreamAsync(reader);
        }
        catch
        {
            // ignored
        }
    }
}
