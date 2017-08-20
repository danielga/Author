namespace Author.Utility
{
    public static class Cryptography
    {
        const string DefaultPassword = "xJ1X0kbGNYHzS6MJ6uAGf4O8z8Ix2XhNHIGhxHxMHCt1pjj3EJG4dRDvlFbB";

        public static Xamarin.Auth.AccountStore CreateAccountStore(string password)
        {
            return Xamarin.Auth.AccountStore.Create(Android.App.Application.Context, password);
        }

        public static Xamarin.Auth.AccountStore CreateAccountStore()
        {
            return CreateAccountStore(DefaultPassword);
        }
    }
}
