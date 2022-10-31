using Author.Messages;
using Author.OTP;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Author.ViewModels;

public class MainPageEntryViewModel : INotifyPropertyChanged
{
    private Secret _secret;
    public Secret Secret
    {
        get => _secret;

        private set
        {
            if (value != _secret)
            {
                _secret = value;
                OnPropertyChanged();
            }
        }
    }

    private double _progress = 0.0;
    public double Progress
    {
        get => _progress;

        set
        {
            if (value != _progress)
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
    }

    private double _animatedProgress = 0.0;
    public double AnimatedProgress
    {
        get => _animatedProgress;

        set
        {
            if (value != _animatedProgress)
            {
                _animatedProgress = value;
                OnPropertyChanged();
            }
        }
    }

    private int _animatedProgressAnimationTime = 0;
    public int AnimatedProgressAnimationTime
    {
        get => _animatedProgressAnimationTime;

        set
        {
            if (value != _animatedProgressAnimationTime)
            {
                _animatedProgressAnimationTime = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Command EditCommand { get; }
    public Command DeleteCommand { get; }

    public MainPageEntryViewModel(Secret secret)
    {
        _secret = secret;

        EditCommand = new Command(() =>
            MessagingCenter.Send(new RequestEditEntry(this), "RequestEditEntry"));
        DeleteCommand = new Command(() =>
            MessagingCenter.Send(new DeleteEntry(this), "DeleteEntry"));
    }

    public MainPageEntryViewModel() : this(new Secret()) { }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void UpdateCode(long timestamp, bool force = false)
    {
        if (Secret == null || !Secret.UpdateCode(timestamp, force))
            return;

        byte period = Secret.Period;
        int progress = (int)(timestamp % period);

        AnimatedProgressAnimationTime = 0;
        AnimatedProgress = progress / (double)period;

        AnimatedProgressAnimationTime = (period - progress) * 1000;
        AnimatedProgress = 1;
    }
}
