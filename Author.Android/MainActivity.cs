using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Author.Android
{
    [Activity(Label = "@string/application_name",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Main",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

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

        private void HandleUri(Intent intent)
        {
            UI.Pages.App app = (UI.Pages.App)Xamarin.Forms.Application.Current;
            if (intent.Scheme == "otpauth")
            {
                app.OnUriRequestReceived(new System.Uri(Intent.DataString));
            }
            else if (intent.Scheme == "file")
            {
                app.OnFileRequestReceived(new System.Uri(Intent.DataString));
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
