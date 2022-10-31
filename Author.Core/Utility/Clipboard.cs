using MauiClipboard = Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard;

namespace Author.Utility
{
    public static class Clipboard
    {
        public static bool HasText => MauiClipboard.HasText;

        public static async Task<string?> GetTextAsync()
        {
            string? text = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                text = await MauiClipboard.GetTextAsync();
            });
            return text;
        }

        public static async Task SetTextAsync(string? text)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await MauiClipboard.SetTextAsync(text);
            });
        }
    }
}
