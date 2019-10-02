using System;
using AppKit;
using Foundation;

namespace Author.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate
    {
        private bool FinishedInitializing;
        private Uri StartupUri;

        private readonly NSWindow window;
        public override NSWindow MainWindow => window;

        public AppDelegate()
        {
            NSAppleEventManager.SharedAppleEventManager.SetEventHandler(this,
                new ObjCRuntime.Selector("handleGetURLEvent:withReplyEvent:"),
                AEEventClass.Internet, AEEventID.GetUrl);

            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable |
                NSWindowStyle.Titled;
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
            Acr.UserDialogs.UserDialogs.Instance = new CustomUserDialogs(window);
            LoadApplication(new UI.Pages.App());
            base.DidFinishLaunching(notification);

            FinishedInitializing = true;
            if (StartupUri != null)
            {
                HandleUri(StartupUri);
                StartupUri = null;
            }
        }

        private void HandleUri(Uri uri)
        {
            UI.Pages.App app = (UI.Pages.App)Xamarin.Forms.Application.Current;
            app.OnUriRequestReceived(uri);
        }

        public override void OpenUrls(NSApplication application, NSUrl[] urls)
        {
            // TODO: handle all URIs in case not finished initializing
            for (int i = 0; i < urls.Length; ++i)
            {
                NSUrl url = urls[i];
                if (url.Scheme == "otpauth")
                {
                    Uri uri = new Uri(url.AbsoluteString);
                    if (FinishedInitializing)
                    {
                        HandleUri(uri);
                    }
                    else
                    {
                        StartupUri = uri;
                    }
                }
                else if (url.Scheme == "file")
                {
                    // TODO: handle file opening
                }
            }
        }
    }
}
