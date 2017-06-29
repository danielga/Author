using Android.App;
using Android.OS;

namespace Author.Droid
{
    [Activity(Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
        MainLauncher = true, //Set it as boot activity
        NoHistory = true)] //Doesn't place it in back stack
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }
}
