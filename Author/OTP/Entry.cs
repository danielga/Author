using Author.UI.Messages;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Author.OTP
{
    [Serializable]
    public class Entry : INotifyPropertyChanged, ISerializable
    {
        IBaseGenerator _generator = null;
        bool _dirtySecret = false;
        long _nextUpdate = long.MinValue;

        string _code = null;
        public string Code
        {
            get { return _code; }

            set
            {
                bool changed = _code != value;

                _code = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        double _progress = 0.0;
        public double Progress
        {
            get { return _progress; }

            set
            {
                bool changed = _progress != value;

                _progress = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        double _animatedProgress = 0.0;
        public double AnimatedProgress
        {
            get { return _animatedProgress; }

            set
            {
                bool changed = _animatedProgress != value;

                _animatedProgress = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        byte _type = 0;
        public byte Type
        {
            get { return _type; }

            internal set
            {
                bool changed = _type != value;

                _type = value;

                if (changed)
                {
                    _dirtySecret = true;
                    OnPropertyChanged();
                }
            }
        }

        string _name = null;
        public string Name
        {
            get { return _name; }

            internal set
            {
                bool changed = _name != value;

                _name = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        byte _digits = 0;
        public byte Digits
        {
            get { return _digits; }

            internal set
            {
                bool changed = _digits != value;

                _digits = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        byte _period = 0;
        public byte Period
        {
            get { return _period; }

            internal set
            {
                bool changed = _period != value;

                _period = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        string _secretData = null;
        public string SecretData
        {
            get { return _secretData; }

            internal set
            {
                bool changed = _secretData != value;

                _secretData = value;

                if (changed)
                {
                    _dirtySecret = true;
                    OnPropertyChanged();
                }
            }
        }

        public Command EditCommand { get; private set; }
        public Command DeleteCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Entry(Secret secret)
        {
            EditCommand = new Command(() =>
                MessagingCenter.Send(new RequestEditEntry { Entry = this }, "RequestEditEntry"));
            DeleteCommand = new Command(() =>
                MessagingCenter.Send(new DeleteEntry { Entry = this }, "DeleteEntry"));

            Type = secret.Type;
            Name = secret.Name;
            Digits = secret.Digits;
            Period = secret.Period;
            SecretData = secret.Data;

            _generator = Factory.CreateGenerator(Type, SecretData);
        }

        public Entry(SerializationInfo info, StreamingContext context)
        {
            EditCommand = new Command(() =>
                MessagingCenter.Send(new RequestEditEntry { Entry = this }, "RequestEditEntry"));
            DeleteCommand = new Command(() =>
                MessagingCenter.Send(new DeleteEntry { Entry = this }, "DeleteEntry"));

            Type = (byte)info.GetValue("Type", typeof(byte));
            Name = (string)info.GetValue("Name", typeof(string));
            Digits = (byte)info.GetValue("Digits", typeof(byte));
            Period = (byte)info.GetValue("Period", typeof(byte));
            SecretData = (string)info.GetValue("SecretData", typeof(string));

            _generator = Factory.CreateGenerator(Type, SecretData);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", _type, typeof(byte));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Digits", _digits, typeof(byte));
            info.AddValue("Period", _period, typeof(byte));
            info.AddValue("SecretData", _secretData, typeof(string));
        }

        public void UpdateCode(long timestamp, bool force = false)
        {
            int progress = (int)(timestamp % _period);

            // We want the progress bar to change immediately
            if (force)
                Progress = progress / (double)_period;

            AnimatedProgress = (progress + 1) / (double)_period;

            if (!force && timestamp < _nextUpdate)
                return;

            Code = _generator.GetCode(timestamp, _digits, _period);
            _nextUpdate = timestamp + _period - timestamp % _period;
        }

        public void UpdateData()
        {
            if (_dirtySecret)
            {
                _dirtySecret = false;
                _generator = Factory.CreateGenerator(Type, SecretData);
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
