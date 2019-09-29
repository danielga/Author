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
        public ObservableCollection<MainPageEntryViewModel> Entries { get; } = new ObservableCollection<MainPageEntryViewModel>();

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
        private readonly HashSet<MainPageEntryViewModel> _visibleEntries = new HashSet<MainPageEntryViewModel>();
        private readonly Database _database = new Database();
        private bool _shouldUpdate = false;

        public EntryManager()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _database.Initialize();
                    foreach (Secret secret in await _database.GetEntries())
                    {
                        Entries.Add(new MainPageEntryViewModel(secret));
                    }

                    Entries.CollectionChanged += OnEntriesChanged;

#if DEBUG
                    // Xamarin.Forms Previewer data
                    if (Entries.Count != 0)
                    {
                        return;
                    }

                    Random random = new Random();
                    for (int i = 0; i < 5; ++i)
                    {
                        MainPageEntryViewModel entry = new MainPageEntryViewModel(new Secret
                        {
                            Name = "Hello world " + i,
                            Digits = (byte) (4 + i),
                            Period = (byte) (5 + i),
                            Data = new string(Enumerable.Repeat(Base32.ValidCharacters, 32).Select(s => s[random.Next(s.Length)])
                                .ToArray())
                        });
                        Entries.Add(entry);
                    }
#endif
                }
                catch
                {
                    // ignored
                }
            });
        }

        private void OnEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (MainPageEntryViewModel entry in e.NewItems)
                        {
                            await _database.AddEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (MainPageEntryViewModel entry in e.OldItems)
                        {
                            await _database.RemoveEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (MainPageEntryViewModel entry in e.OldItems)
                        {
                            await _database.RemoveEntry(entry.Secret);
                        }

                        foreach (MainPageEntryViewModel entry in e.NewItems)
                        {
                            await _database.AddEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Move:
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        await _database.RemoveEntries();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
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
    }
}
