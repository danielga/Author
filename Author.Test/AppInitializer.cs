using Xamarin.UITest;

namespace Author.Test
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.Debug()
                    .ApkFile("Author.Android/bin/Debug/xyz.metaman.Author.apk")
                    .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}
