using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Author.OTP
{
    public class Entry : INotifyPropertyChanged
    {
        IBaseGenerator _generator = null;
        bool _dirtySecret = false;
        long _nextUpdate = long.MinValue;

        public Guid Identifier { get; }

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

        Type _type = Type.Hash;
        public Type Type
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

        string _secret = null;
        public string Secret
        {
            get { return _secret; }

            internal set
            {
                bool changed = _secret != value;

                _secret = value;

                if (changed)
                {
                    _dirtySecret = true;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Entry(Secret secret)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(secret.Name), "Invalid name (null or only contains whitespace)");
            Debug.Assert(secret.Digits >= 4 && secret.Digits <= 8, "Invalid OTP length (must be between 4 and 8)");
            Debug.Assert(secret.Period >= 5 && secret.Period <= 60, "Invalid OTP period (must be between 5 and 60)");
            Debug.Assert(!string.IsNullOrWhiteSpace(secret.Data), "Invalid secret (null or only contains whitespace)");

            Identifier = secret.Identifier.Equals(Guid.Empty) ? Guid.NewGuid() : secret.Identifier;
            Type = secret.Type;
            Name = secret.Name;
            Digits = secret.Digits;
            Period = secret.Period;
            Secret = secret.Data;

            _generator = Factory.CreateGenerator(Type, Secret);
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
                _generator = Factory.CreateGenerator(Type, Secret);
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Secret GetSecret()
        {
            return new Secret
            {
                Identifier = Identifier,
                Type = Type,
                Name = Name,
                Digits = Digits,
                Period = Period,
                Data = Secret
            };
        }
    }
}
