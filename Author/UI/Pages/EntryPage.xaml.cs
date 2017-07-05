using Acr.UserDialogs;
using Author.OTP;
using Author.UI.ViewModels;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class EntryPage : ContentPage
    {
        public EntryPage()
        {
            InitializeComponent();

            ViewModelLocator.EntryPageVM.Page = this;
        }
    }
}
