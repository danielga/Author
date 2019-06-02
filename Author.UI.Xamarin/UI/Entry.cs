using Author.OTP;
using Author.UI.Messages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Author.UI
{
    public class Entry : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Command EditCommand { get; private set; }
        public Command DeleteCommand { get; private set; }

        public Entry(Secret secret)
        {
            Secret = secret;

            EditCommand = new Command(() =>
                MessagingCenter.Send(new RequestEditEntry { Entry = this }, "RequestEditEntry"));
            DeleteCommand = new Command(() =>
                MessagingCenter.Send(new DeleteEntry { Entry = this }, "DeleteEntry"));
        }

        public Entry() : this(new Secret()) { }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateCode(long timestamp, bool force = false)
        {
            if (Secret == null)
            {
                return;
            }

            byte period = Secret.Period;
            int progress = (int)(timestamp % period);

            // We want the progress bar to change immediately
            if (force)
            {
                Progress = progress / (double)period;
            }

            AnimatedProgress = (progress + 1) / (double)period;

            Secret.UpdateCode(timestamp, force);
        }
    }
}
