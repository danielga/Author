using CommunityToolkit.Maui.Core;
using MauiSnackbar = CommunityToolkit.Maui.Alerts.Snackbar;

namespace Author;

public static class SnackbarDuration
{
    public static readonly TimeSpan Short = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan Long = TimeSpan.FromSeconds(3.5);
}

public class Snackbar : IDisposable
{
    private readonly string Message;
    private Action? Action = null;
    private string ActionButtonText = AlertDefaults.ActionButtonText;
    private TimeSpan? Duration = null;
    private readonly SnackbarOptions Options = new();
    private readonly CancellationTokenSource SnackbarCancellationTokenSource = new();

    public static Snackbar Create(string message)
    {
        return new(message);
    }

    private Snackbar(string message)
    {
        Message = message;
    }

    public Snackbar SetAction(Action action)
    {
        Action = action;
        return this;
    }

    public Snackbar SetActionButtonText(string text)
    {
        ActionButtonText = text;
        return this;
    }

    public Snackbar SetDuration(TimeSpan duration)
    {
        Duration = duration;
        return this;
    }

    public Snackbar SetDuration(int ms)
    {
        Duration = TimeSpan.FromMilliseconds(ms);
        return this;
    }

    public Snackbar SetBackgroundColor(Color color)
    {
        Options.BackgroundColor = color;
        return this;
    }

    public Snackbar SetMessageTextColor(Color color)
    {
        Options.TextColor = color;
        return this;
    }

    public void Show()
    {
        var snackbar = MauiSnackbar.Make(Message, Action, ActionButtonText, Duration, Options);
        snackbar.Show(SnackbarCancellationTokenSource.Token);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            SnackbarCancellationTokenSource.Cancel();
            SnackbarCancellationTokenSource.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
