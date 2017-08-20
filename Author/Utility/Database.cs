using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace Author.Utility
{
    public static class Database
    {
        const string ServiceName = "Author";

        static AccountStore _accountStore = Cryptography.CreateAccountStore();

        public static async Task AddEntry(OTP.Entry entry)
        {
            string identifier = entry.Identifier.ToString();
            await _accountStore.SaveAsync(new Account(identifier, new Dictionary<string, string>
            {
                { "Identifier", identifier },
                { "Type", entry.Type.ToString() },
                { "Name", entry.Name },
                { "Digits", entry.Digits.ToString() },
                { "Period", entry.Period.ToString() },
                { "Data", entry.SecretData }
            }), ServiceName);
        }

        public static async Task RemoveEntry(OTP.Entry entry)
        {
            await _accountStore.DeleteAsync(new Account(entry.Identifier.ToString()), ServiceName);
        }

        public static async Task<List<OTP.Entry>> GetEntries()
        {
            List<Account> accounts = await _accountStore.FindAccountsForServiceAsync(ServiceName);
            if (accounts == null)
                return null;

            List<OTP.Entry> entries = new List<OTP.Entry>();
            foreach (Account account in accounts)
                entries.Add(new OTP.Entry(new OTP.Secret
                {
                    Identifier = Guid.Parse(account.Properties["Identifier"]),
                    Type = byte.Parse(account.Properties["Type"]),
                    Name = account.Properties["Name"],
                    Digits = byte.Parse(account.Properties["Digits"]),
                    Period = byte.Parse(account.Properties["Period"]),
                    Data = account.Properties["Data"]
                }));

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
