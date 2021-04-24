using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Content;
using Android.Net;
using System.IO;

namespace Author.Android
{
    [Activity(Label = "@string/application_name",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        NoHistory = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        Label = "@string/application_name",
        Icon = "@drawable/icon",
        DataScheme = "otpauth")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        Label = "@string/application_name",
        Icon = "@drawable/icon",
        DataSchemes = new[] { "file", "content" },
        DataMimeType = "*/*",
        DataPathPattern = ".*\\.otp")]
    [IntentFilter(new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        Label = "@string/application_name",
        Icon = "@drawable/icon",
        DataSchemes = new[] { "file", "content" },
        DataMimeType = "*/*",
        DataPathPattern = ".*\\.otp")]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent intent = new Intent(this, typeof(MainActivity));

            if (Intent?.Data != null)
            {
                try
                {
                    string path = Path.Combine(CacheDir.AbsolutePath, "import.otp");
                    Uri data = Intent.Data;
                    if (data.Scheme == "content")
                    {
                        using (Stream stream = ContentResolver.OpenInputStream(data))
                        using (Stream writer = new FileStream(path, FileMode.Create))
                            stream.CopyTo(writer);

                        data = Uri.Parse("file://" + path);
                    }
                    else if(data.Scheme == "file")
                    {
                        File.Copy(data.Path, path, true);
                        data = Uri.Parse("file://" + path);
                    }

                    intent.SetData(data);
                }
                catch (System.Exception)
                { }
            }

            StartActivity(intent);
            Finish();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
