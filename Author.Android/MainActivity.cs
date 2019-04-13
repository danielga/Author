using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Author.Android
{
    [Activity(Label = "Author",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Acr.UserDialogs.UserDialogs.Init(this);
            Xamarin.Essentials.Platform.Init(this, bundle);

            LoadApplication(new UI.Pages.App());
        }
    }
}
