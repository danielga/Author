using Author.OTP;
using Author.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Author.UI
{
    public class EntryManager
    {
        private readonly ObservableCollection<MainPageEntryViewModel> _entries = new ObservableCollection<MainPageEntryViewModel>();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
        private readonly HashSet<MainPageEntryViewModel> _visibleEntries = new HashSet<MainPageEntryViewModel>();
        private readonly Database _database = new Database();
        private bool _shouldUpdate = false;

        public ReadOnlyObservableCollection<MainPageEntryViewModel> Entries { get; }

        public EntryManager()
        {
            Entries = new ReadOnlyObservableCollection<MainPageEntryViewModel>(_entries);

            Task.Run(async () =>
            {
                try
                {
                    await _database.Initialize();

                    List<MainPageEntryViewModel> entries = new List<MainPageEntryViewModel>();
                    foreach (Secret secret in await _database.GetEntries())
                    {
                        entries.Add(new MainPageEntryViewModel(secret));
                    }

#if DEBUG
                    // Xamarin.Forms Previewer data
                    if (entries.Count == 0)
                    {
                        Random random = new Random();
                        for (int i = 0; i < 5; ++i)
                        {
                            MainPageEntryViewModel entry = new MainPageEntryViewModel(new Secret
                            {
                                Name = "Hello world " + i,
                                Digits = (byte)(4 + i),
                                Period = (byte)(5 + i),
                                Data = new string(Enumerable.Repeat(Base32.ValidCharacters, 32).Select(s => s[random.Next(s.Length)])
                                    .ToArray())
                            });
                            entries.Add(entry);
                        }
                    }
#endif

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (MainPageEntryViewModel entry in entries)
                        {
                            _entries.Add(entry);
                        }
                    });
                }
                catch
                {
                    // ignored
                }
            });
        }

        private bool UpdateEntries()
        {
            long timestamp = Time.GetCurrent();
            foreach (MainPageEntryViewModel entry in _visibleEntries)
            {
                entry.UpdateCode(timestamp);
            }

            return _shouldUpdate;
        }

        public void EnableUpdate()
        {
            if (_shouldUpdate)
            {
                return;
            }

            _shouldUpdate = true;
            UpdateEntries();
            Device.StartTimer(_updateInterval, UpdateEntries);
        }

        public void DisableUpdate()
        {
            _shouldUpdate = false;
        }

        public void OnEntryAppearing(MainPageEntryViewModel entry)
        {
            if (entry == null)
            {
                return;
            }

            entry.UpdateCode(Time.GetCurrent(), true);
            _visibleEntries.Add(entry);
        }

        public void OnEntryDisappearing(MainPageEntryViewModel entry)
        {
            if (entry == null || (Device.RuntimePlatform == Device.UWP && !_shouldUpdate))
            {
                return;
            }

            _visibleEntries.Remove(entry);
        }

        public async void Add(MainPageEntryViewModel entry)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                _entries.Add(entry);
            });

            await _database.AddEntry(entry.Secret);
        }

        public async void Remove(MainPageEntryViewModel entry)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                _entries.Remove(entry);
            });

            await _database.RemoveEntry(entry.Secret);
        }

        public async void Reset()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                _entries.Clear();
            });

            await _database.RemoveEntries();
        }
    }
}
