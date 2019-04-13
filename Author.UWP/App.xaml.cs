using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Author.UWP
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        private void CreateMainPage(IActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), (e as LaunchActivatedEventArgs)?.Arguments);
            }

            Window.Current.Activate();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            CreateMainPage(e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            CreateMainPage(e);

            if (e.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = e as ProtocolActivatedEventArgs;
                if (eventArgs.Uri.Scheme == "otpauth")
                {
                    UI.Pages.App app = Xamarin.Forms.Application.Current as UI.Pages.App;
                    app.HandleUriScheme(eventArgs.Uri);
                }
            }
        }
    }
}
