using Author.OTP;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Author.Utility
{
    public static class Database
    {
        private const string ServiceName = "Author";
        private const string IdentifiersName = ServiceName + ".List";
        private const string IdentifiersSeparator = ";";

        private static readonly HashSet<string> StoredIdentifiers = LoadIdentifiers();

        private static HashSet<string> LoadIdentifiers()
        {
            Task<string> task = SecureStorage.GetAsync(IdentifiersName);
            task.Wait();
            string identifiers = task.Result;
            if (identifiers == null)
                return new HashSet<string>();

            return new HashSet<string>(identifiers.Split(IdentifiersSeparator));
        }

        private static async Task SaveIdentifiers(HashSet<string> identifiers)
        {
            if (identifiers.Count == 0)
            {
                SecureStorage.Remove(IdentifiersName);
                return;
            }

            string ids = string.Join(IdentifiersSeparator, identifiers);
            await SecureStorage.SetAsync(IdentifiersName, ids);
        }

        public static async Task AddEntry(Secret entry)
        {
            string identifier = entry.Identifier.ToString();
            await SecureStorage.SetAsync(ServiceName + "." + identifier, entry.ToString());
            StoredIdentifiers.Add(identifier);
            await SaveIdentifiers(StoredIdentifiers);
        }

        public static async Task<bool> RemoveEntry(Secret entry)
        {
            string identifier = entry.Identifier.ToString();
            bool success = SecureStorage.Remove(ServiceName + "." + identifier);
            if (success)
            {
                StoredIdentifiers.Remove(identifier);
                await SaveIdentifiers(StoredIdentifiers);
            }

            return success;
        }

        public static async Task<List<Secret>> GetEntries()
        {
            List<Secret> entries = new List<Secret>();
            foreach (string identifier in StoredIdentifiers)
            {
                string entryString = await SecureStorage.GetAsync(ServiceName + "." + identifier);
                entries.Add(Secret.Parse(entryString));
            }

            return entries;
        }

        public static async Task RemoveEntries()
        {
            foreach (string identifier in StoredIdentifiers)
                SecureStorage.Remove(ServiceName + "." + identifier);

            StoredIdentifiers.Clear();
            await SaveIdentifiers(StoredIdentifiers);
        }
    }
}
