namespace Author.UI.ViewModels
{
    public static class ViewModelLocator
    {
        static EntryPageViewModel _entryPageVM;
        public static EntryPageViewModel EntryPageVM => _entryPageVM;

        static MainPageViewModel _mainPageVM;
        public static MainPageViewModel MainPageVM => _mainPageVM;

        static ViewModelLocator()
        {
            _entryPageVM = new EntryPageViewModel();
            _mainPageVM = new MainPageViewModel();
        }
    }
}
