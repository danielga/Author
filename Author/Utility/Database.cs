using System;
using Author.OTP;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Author.Utility
{
    public class Database
    {
        private const string ServiceName = "Author";
        private const string IdentifiersName = ServiceName + ".List";
        private const char IdentifiersSeparator = ';';

        private HashSet<string> _storedIdentifiers = null;

        public async Task Initialize()
        {
            try
            {
                string identifiers = await SecureStorage.GetAsync(IdentifiersName);
                if (identifiers != null)
                {
                    _storedIdentifiers = new HashSet<string>(identifiers.Split(IdentifiersSeparator));
                    return;
                }
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch
            {
                SecureStorage.Remove(IdentifiersName);
            }

            _storedIdentifiers = new HashSet<string>();
        }

        private async Task SaveIdentifiers()
        {
            if (_storedIdentifiers.Count == 0)
            {
                SecureStorage.Remove(IdentifiersName);
                return;
            }

            string ids = string.Join(IdentifiersSeparator.ToString(), _storedIdentifiers);
            await SecureStorage.SetAsync(IdentifiersName, ids);
        }

        public async Task AddEntry(Secret entry)
        {
            string identifier = entry.Identifier.ToString();
            await SecureStorage.SetAsync(ServiceName + "." + identifier, entry.ToString());
            _storedIdentifiers.Add(identifier);
            await SaveIdentifiers();
        }

        public async Task<bool> RemoveEntry(Secret entry)
        {
            string identifier = entry.Identifier.ToString();
            bool success = SecureStorage.Remove(ServiceName + "." + identifier);
            if (success)
            {
                _storedIdentifiers.Remove(identifier);
                await SaveIdentifiers();
            }

            return success;
        }

        public async Task<List<Secret>> GetEntries()
        {
            List<Secret> entries = new List<Secret>();
            foreach (string identifier in _storedIdentifiers)
            {
                string entryString = await SecureStorage.GetAsync(ServiceName + "." + identifier);
                entries.Add(Secret.Parse(entryString));
            }

            return entries;
        }

        public async Task RemoveEntries()
        {
            foreach (string identifier in _storedIdentifiers)
                SecureStorage.Remove(ServiceName + "." + identifier);

            _storedIdentifiers.Clear();
            await SaveIdentifiers();
        }
    }
}
