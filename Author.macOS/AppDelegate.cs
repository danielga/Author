using AppKit;
using Foundation;

namespace Author.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate
    {
        private readonly NSWindow window;

        public override NSWindow MainWindow
        {
            get { return window; }
        }

        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            window = new NSWindow(rect, style, NSBackingStore.Buffered, false)
            {
                Title = "Author",
                TitleVisibility = NSWindowTitleVisibility.Hidden
            };
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Xamarin.Forms.Forms.Init();
            LoadApplication(new UI.Pages.App());
            base.DidFinishLaunching(notification);
        }
    }
}
