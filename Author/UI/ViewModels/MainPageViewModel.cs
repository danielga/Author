using Author.OTP;
using Author.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Author.UI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Entry> _entriesList = new ObservableCollection<Entry>();
        public ObservableCollection<Entry> EntriesList
        {
            get
            {
                return _entriesList;
            }

            internal set
            {
                _entriesList = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            // Xamarin.Forms Previewer data
            const string Chars = Base32.ValidCharacters;

            Random random = new Random();
            long timestamp = Time.GetCurrent();
            for (int i = 0; i < 5; ++i)
            {
                Entry entry = new Entry(new Secret
                {
                    Type = "time",
                    Name = "Hello world " + i,
                    Digits = (byte)(4 + i),
                    Period = (byte)(30 + (i - 2) * 5),
                    Data = new string(Enumerable.Repeat(Chars, 32).Select(s => s[random.Next(s.Length)]).ToArray())
                });
                _entriesList.Add(entry);
                entry.Update(timestamp);
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
