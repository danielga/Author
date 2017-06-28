using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Author.OTP
{
    [Serializable]
    public class Entry : INotifyPropertyChanged, ISerializable
    {
        readonly IBaseGenerator _generator;

        long _nextUpdate = long.MinValue;
        public long NextUpdate
        {
            get { return _nextUpdate; }

            set
            {
                _nextUpdate = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _generator.Name; }

            set
            {
                _generator.Name = value;
                OnPropertyChanged();
            }
        }

        string _code;
        public string Code
        {
            get { return _code; }

            internal set
            {
                _code = value;
                OnPropertyChanged();
            }
        }

        double _progress;
        public double Progress
        {
            get { return _progress; }

            private set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Entry(Secret secret)
        {
            _generator = Factory.CreateGenerator(secret);
        }

        public Entry(SerializationInfo info, StreamingContext context)
        {
            _generator = Factory.CreateGenerator(new Secret {
                Type = (string)info.GetValue("Type", typeof(string)),
                Name = (string)info.GetValue("Name", typeof(string)),
                Digits = (byte)info.GetValue("Digits", typeof(byte)),
                Period = (byte)info.GetValue("Period", typeof(byte)),
                Data = (string)info.GetValue("SecretData", typeof(string))
            });
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", _generator.Type, typeof(string));
            info.AddValue("Name", _generator.Name, typeof(string));
            info.AddValue("Digits", _generator.Digits, typeof(byte));
            info.AddValue("Period", _generator.Period, typeof(byte));
            info.AddValue("SecretData", _generator.SecretData, typeof(string));
        }

        public void Update(long timestamp)
        {
            int progress = (int)(timestamp % _generator.Period);
            Progress = progress / (double)_generator.Period;

            if (timestamp < NextUpdate)
                return;

            Code = _generator.GetCode(timestamp);
            NextUpdate = timestamp + _generator.Period - timestamp % _generator.Period;
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
