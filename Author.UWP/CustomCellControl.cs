using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace Author.UWP
{
    // This is required because the UWP platform on Windows seems to have a bug on ListView
    // which causes deletion, addition and deletion of the previous addition to call
    // context actions with the wrong binding context (or just wrong view cell)
    // Bug 42999 - MenuItems in ContextActions are sometimes bound to the wrong bindingContext in UWP
    // https://bugzilla.xamarin.com/show_bug.cgi?id=42999
    // Bug 57982 - ListView context actions called with the wrong binding context
    // https://bugzilla.xamarin.com/show_bug.cgi?id=57982
    public class CustomCellControl : CellControl
    {
        public CustomCellControl()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (CellContent == null)
                return;

            var flyout = FlyoutBase.GetAttachedFlyout(CellContent) as MenuFlyout;
            if (flyout?.Items == null)
                return;

            foreach (var flyoutItem in flyout.Items)
            {
                var menuItem = (MenuItem)flyoutItem.DataContext;
                menuItem.BindingContext = args.NewValue;
            }
        }
    }
}
