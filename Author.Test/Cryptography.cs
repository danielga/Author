namespace Author.Utility
{
    public static class Cryptography
    {
        public static Xamarin.Auth.AccountStore CreateAccountStore(string password)
        {
            return CreateAccountStore();
        }

        public static Xamarin.Auth.AccountStore CreateAccountStore()
        {
            return Xamarin.Auth.AccountStore.Create();
        }
    }
}
