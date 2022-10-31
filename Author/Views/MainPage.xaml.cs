using Author.ViewModels;

namespace Author.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        MainPageViewModel mainPageVM = (MainPageViewModel)BindingContext;
        mainPageVM.Page = this;
    }
}
