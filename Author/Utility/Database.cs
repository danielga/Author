using Author.OTP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Author.Utility.Cryptography;

namespace Author.Utility
{
    public static class Database
    {
        private const string ServiceName = "Author";

        private static AccountStore _accountStore = CreateAccountStore();

        public static async Task AddEntry(Secret entry)
        {
            string identifier = entry.Identifier.ToString();
            await _accountStore.SaveAsync(new Account(identifier, new Dictionary<string, string>
            {
                { "Identifier", identifier },
                { "Type", entry.Type.Name },
                { "Name", entry.Name },
                { "Digits", entry.Digits.ToString() },
                { "Period", entry.Period.ToString() },
                { "Data", entry.Data }
            }), ServiceName);
        }

        public static async Task RemoveEntry(Secret entry)
        {
            await _accountStore.DeleteAsync(new Account(entry.Identifier.ToString()), ServiceName);
        }

        public static async Task<List<Secret>> GetEntries()
        {
            List<Account> accounts = await _accountStore.FindAccountsForServiceAsync(ServiceName);
            if (accounts == null)
                return null;

            List<Secret> entries = new List<Secret>();
            foreach (Account account in accounts)
                entries.Add(new Secret
                {
                    Identifier = Guid.Parse(account.Properties["Identifier"]),
                    Type = OTP.Type.Parse(account.Properties["Type"]),
                    Name = account.Properties["Name"],
                    Digits = byte.Parse(account.Properties["Digits"]),
                    Period = byte.Parse(account.Properties["Period"]),
                    Data = account.Properties["Data"]
                });

            return entries;
        }

        public static async Task RemoveEntries()
        {
            IEnumerable<Account> accounts = await _accountStore.FindAccountsForServiceAsync(ServiceName);
            if (accounts == null)
                return;

            foreach (Account account in accounts)
                await _accountStore.DeleteAsync(account, ServiceName);
        }
    }
}
