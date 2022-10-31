using CommunityToolkit.Maui.Core;
using MauiToast = CommunityToolkit.Maui.Alerts.Toast;
using MauiToastDuration = CommunityToolkit.Maui.Core.ToastDuration;

namespace Author;

public enum ToastDuration
{
    Short = 0,
    Long = 1
}

public class Toast : IDisposable
{
    private readonly string Message;
    private ToastDuration Duration = ToastDuration.Short;
    private double FontSize = AlertDefaults.FontSize;
    private readonly CancellationTokenSource ToastCancellationTokenSource = new();

    public static Toast Create(string message)
    {
        return new(message);
    }

    private Toast(string message)
    {
        Message = message;
    }

    public Toast SetDuration(ToastDuration duration)
    {
        Duration = duration;
        return this;
    }

    public Toast SetFontSize(double fontSize)
    {
        FontSize = fontSize;
        return this;
    }

    public void Show()
    {
        var duration = Duration == ToastDuration.Short ? MauiToastDuration.Short : MauiToastDuration.Long;
        var toast = MauiToast.Make(Message, duration, FontSize);
        toast.Show(ToastCancellationTokenSource.Token);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ToastCancellationTokenSource.Cancel();
            ToastCancellationTokenSource.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
