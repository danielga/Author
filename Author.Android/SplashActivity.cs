using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;

namespace Author.Android
{
    [Activity(Label = "@string/application_name",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        NoHistory = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "otpauth",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        Icon = "@drawable/icon")]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent intent = new Intent(this, typeof(MainActivity));

            if (Intent?.Data != null)
            {
                intent.SetData(Intent.Data);
            }

            StartActivity(intent);
            Finish();
        }
    }
}