using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Author.Android
{
    [Activity(Label = "@string/application_name",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "otpauth",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        Icon = "@drawable/icon")]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetTheme(Resource.Style.Theme_Main);
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Acr.UserDialogs.UserDialogs.Init(this);
            Xamarin.Essentials.Platform.Init(this, bundle);

            LoadApplication(new UI.Pages.App());

            if (Intent != null)
            {
                HandleUri(Intent);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            HandleUri(intent);
        }

        private void HandleUri(Intent intent)
        {
            if (intent.Data?.Scheme == "otpauth")
            {
                UI.Pages.App app = Xamarin.Forms.Application.Current as UI.Pages.App;
                app.HandleUriScheme(new System.Uri(Intent.Data.ToString()));
            }
        }
    }
}
