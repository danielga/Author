using Author.OTP;
using Author.UI.Messages;
using Author.Utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Author.UI
{
    public class MainPageEntryViewModel : INotifyPropertyChanged
    {
        private Secret _secret = null;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Command EditCommand { get; }
        public Command DeleteCommand { get; }

        public MainPageEntryViewModel(Secret secret)
        {
            Secret = secret;

            EditCommand = new Command(() =>
                MessagingCenter.Send(new RequestEditEntry { Entry = this }, "RequestEditEntry"));
            DeleteCommand = new Command(() =>
                MessagingCenter.Send(new DeleteEntry { Entry = this }, "DeleteEntry"));
        }

        public MainPageEntryViewModel() : this(new Secret()) { }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateCode(long timestamp, bool force = false)
        {
            bool? shouldUpdate = Secret?.UpdateCode(timestamp, force);
            if (shouldUpdate.HasValue && shouldUpdate.Value)
            {
                byte period = Secret.Period;
                int progress = (int)(timestamp % period);

                AnimatedProgressAnimationTime = 0;
                AnimatedProgress = progress / (double)period;

                AnimatedProgressAnimationTime = (period - progress) * 1000;
                AnimatedProgress = 1;
            }
        }
    }
}
