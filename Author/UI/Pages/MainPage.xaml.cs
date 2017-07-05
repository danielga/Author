using Author.UI.ViewModels;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModelLocator.MainPageVM.Page = this;
        }
    }
}
