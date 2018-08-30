using Xamarin.Auth;

namespace Author.Utility
{
    public static class Cryptography
    {
        public static AccountStore CreateAccountStore(string password)
        {
            return CreateAccountStore();
        }

        public static AccountStore CreateAccountStore()
        {
            return AccountStore.Create();
        }
    }
}
