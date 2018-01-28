using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;

namespace Author.Droid
{
    [Activity(Label = "Author",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar; 

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            UserDialogs.Init(this);

            LoadApplication(new UI.Pages.App());
        }
    }
}
