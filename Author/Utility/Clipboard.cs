using System.Threading.Tasks;

namespace Author.Utility
{
    public static class Clipboard
    {
        public static bool HasText => Xamarin.Essentials.Clipboard.HasText;

        public static Task<string> GetTextAsync() => Xamarin.Essentials.Clipboard.GetTextAsync();

        public static Task SetTextAsync(string text) => Xamarin.Essentials.Clipboard.SetTextAsync(text);
    }
}
