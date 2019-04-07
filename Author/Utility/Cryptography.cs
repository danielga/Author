using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Author.Utility
{
    public static class Cryptography
    {
        public class Account
        {
            public string Username { get; set; }
            public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

            public Account(string username)
            {
                Username = username;
            }

            public Account(string username, Dictionary<string, string> properties) : this(username)
            {
                Properties = properties;
            }
        }

        private const string LibraryName = "Author";

        public class AccountStore
        {
            public Task SaveAsync(Account account, string serviceName)
            {
                return SecureStorage.SetAsync(LibraryName + "." + serviceName + "." + account.Username, account.ToString());
            }

            public Task<bool> DeleteAsync(Account account, string serviceName)
            {
                return new Task<bool>(() => SecureStorage.Remove(LibraryName + "." + serviceName + "." + account.Username));
            }

            public Task<List<Account>> FindAccountsForServiceAsync(string serviceName)
            {
                return null;
            }
        }

        public static AccountStore CreateAccountStore(string password)
        {
            return new AccountStore();
        }

        public static AccountStore CreateAccountStore()
        {
            return new AccountStore();
        }
    }
}
