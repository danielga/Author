using System;
using AppKit;
using Foundation;
using Xamarin.Forms;

namespace Author.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate
    {
        private bool FinishedInitializing;
        private Uri StartupUri;

        private readonly NSWindow window;
        public override NSWindow MainWindow
        {
            get { return window; }
        }

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
            UI.Pages.App app = Application.Current as UI.Pages.App;
            app.HandleUriScheme(uri);
        }

        [Export("handleGetURLEvent:withReplyEvent:")]
        private void HandleGetURLEvent(NSAppleEventDescriptor descriptor,
            NSAppleEventDescriptor replyEvent)
        {
            if (descriptor.EventClass != AEEventClass.Internet ||
                descriptor.EventID != AEEventID.GetUrl)
            {
                return;
            }

            for (int i = 1; i <= descriptor.NumberOfItems; i++)
            {
                var innerDesc = descriptor.DescriptorAtIndex(i);
                if (!string.IsNullOrEmpty(innerDesc.StringValue))
                {
                    Uri uri = new Uri(innerDesc.StringValue);
                    if (FinishedInitializing)
                    {
                        HandleUri(uri);
                    }
                    else
                    {
                        StartupUri = uri;
                    }
                }
            }
        }
    }
}
