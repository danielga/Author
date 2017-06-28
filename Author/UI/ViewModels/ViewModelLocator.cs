namespace Author.UI.ViewModels
{
    public static class ViewModelLocator
    {
        static AddEntryPageViewModel _addEntryPageVM;
        public static AddEntryPageViewModel AddEntryPageVM => _addEntryPageVM;

        static MainPageViewModel _mainPageVM;
        public static MainPageViewModel MainPageVM => _mainPageVM;

        static ViewModelLocator()
        {
            _addEntryPageVM = new AddEntryPageViewModel();
            _mainPageVM = new MainPageViewModel();
        }
    }
}
