using Author.OTP;

namespace Author.Utility
{
    public class Database
    {
        private const string ServiceName = "Author";
        private const string IdentifiersName = ServiceName + ".List";
        private const char IdentifiersSeparator = ';';

        private readonly HashSet<string> _storedIdentifiers = new();

        public async Task Initialize()
        {
            try
            {
                var identifiers = await SecureStorage.GetAsync(IdentifiersName);
                if (identifiers != null)
                    _storedIdentifiers.UnionWith(identifiers.Split(IdentifiersSeparator));
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch
            {
                SecureStorage.Remove(IdentifiersName);
            }
        }

        private async Task SaveIdentifiers()
        {
            if (_storedIdentifiers.Count == 0)
            {
                SecureStorage.Remove(IdentifiersName);
                return;
            }

            var ids = string.Join(IdentifiersSeparator.ToString(), _storedIdentifiers);
            await SecureStorage.SetAsync(IdentifiersName, ids);
        }

        public async Task AddEntry(Secret entry)
        {
            var identifier = entry.Identifier.ToString();
            await SecureStorage.SetAsync(ServiceName + "." + identifier, entry.ToString());
            _storedIdentifiers.Add(identifier);
            await SaveIdentifiers();
        }

        public async Task<bool> RemoveEntry(Secret entry)
        {
            var identifier = entry.Identifier.ToString();
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
            var entries = new List<Secret>();

            foreach (var identifier in _storedIdentifiers)
            {
                var entryString = await SecureStorage.GetAsync(ServiceName + "." + identifier);
                entries.Add(Secret.Parse(entryString));
            }

            return entries;
        }

        public async Task RemoveEntries()
        {
            foreach (var identifier in _storedIdentifiers)
                SecureStorage.Remove(ServiceName + "." + identifier);

            _storedIdentifiers.Clear();

            await SaveIdentifiers();
        }
    }
}
