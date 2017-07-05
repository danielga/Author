namespace Author.UI.ViewModels
{
    public static class ViewModelLocator
    {
        public static EntryPageViewModel EntryPageVM { get; private set; }
        public static MainPageViewModel MainPageVM { get; private set; }
        public static SettingsPageViewModel SettingsPageVM { get; private set; }

        static ViewModelLocator()
        {
            EntryPageVM = new EntryPageViewModel();
            MainPageVM = new MainPageViewModel();
            SettingsPageVM = new SettingsPageViewModel();
        }
    }
}
