using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
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
            base.OnLaunched(e);

            CreateMainPage(e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            CreateMainPage(e);

            if (e.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = (ProtocolActivatedEventArgs)e;
                if (eventArgs.Uri.Scheme == "otpauth")
                {
                    UI.Pages.App app = (UI.Pages.App)Xamarin.Forms.Application.Current;
                    app.OnUriRequestReceived(eventArgs.Uri);
                }
            }
        }

        protected override void OnFileActivated(FileActivatedEventArgs e)
        {
            base.OnFileActivated(e);

            CreateMainPage(e);

            UI.Pages.App app = (UI.Pages.App)Xamarin.Forms.Application.Current;
            foreach (IStorageItem file in e.Files)
            {
                app.OnFileRequestReceived(new Uri(file.Path));
            }
        }

        // Currently closes the new window that is opened and changes state on the old window to prevent a Xamarin UWP crash.
        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs e)
        {
            base.OnShareTargetActivated(e);

            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                CreateMainPage(e);
            }

            var newWindow = Window.Current;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                ShareOperation shareOp = e.ShareOperation;
                shareOp.ReportStarted();

                UI.Pages.App app = (UI.Pages.App)Xamarin.Forms.Application.Current;
                DataPackageView sharedData = shareOp.Data;
                bool handled = false;
                foreach (string format in sharedData.AvailableFormats)
                {
                    if (format == "Text" || format == "RTF" || format == "URI")
                    {
                        handled = true;
                        shareOp.ReportDataRetrieved();
                    }
                    else if (format == "StorageItems" || format == "Shell IDList Array")
                    {
                        IReadOnlyList<IStorageItem> items = await sharedData.GetStorageItemsAsync();
                        foreach (IStorageItem item in items)
                        {
                            app.OnFileRequestReceived(new Uri(item.Path));
                        }

                        handled = true;
                        shareOp.ReportDataRetrieved();
                    }
                }

                if (handled)
                {
                    shareOp.ReportCompleted();
                }
                else
                {
                    shareOp.ReportError("Trying to share an unknown object");
                }

                if (e.PreviousExecutionState == ApplicationExecutionState.Running)
                {
                    await newWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        newWindow.Close();
                    });
                }
            });
        }

        protected override void OnFileOpenPickerActivated(FileOpenPickerActivatedEventArgs e)
        {
            base.OnFileOpenPickerActivated(e);

            CreateMainPage(e);


        }

        protected override void OnFileSavePickerActivated(FileSavePickerActivatedEventArgs e)
        {
            base.OnFileSavePickerActivated(e);

            CreateMainPage(e);


        }
    }
}
