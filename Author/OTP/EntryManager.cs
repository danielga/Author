using System;
using System.Collections.ObjectModel;
using Author.Utility;
using Xamarin.Forms;
using System.Threading.Tasks;
using Author.Database;
using Author.UI.ViewModels;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Author.OTP
{
    public class EntryManager
    {
        const string EntriesPath = "entries.txt";

        public ObservableCollection<Entry> Entries
        {
            get
            {
                return _mainPageVM.EntriesList;
            }

            private set
            {
                _mainPageVM.EntriesList = value;
            }
        }

        MainPageViewModel _mainPageVM = ViewModelLocator.MainPageVM;

        readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        readonly List<Entry> _visibleEntries = new List<Entry>();
        bool _shouldUpdate = false;

        public EntryManager()
        {
            Task.Run(async () => {
                try
                {
                    ObservableCollection<Entry> entries =
                        await Filesystem.LoadFromPath<ObservableCollection<Entry>>(EntriesPath);
                    if (entries != null)
                        Entries = entries;
                }
                catch(Exception)
                { }

                // TODO: Check if collection changes can happen before this is executed
                Entries.CollectionChanged += OnEntriesChanged;
            });
        }

        void OnEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Run(async () => await Filesystem.SaveToPath(EntriesPath, Entries));
        }

        bool UpdateEntries()
        {
            long timestamp = Time.GetCurrent();
            foreach (Entry entry in _visibleEntries)
                entry.UpdateCode(timestamp);

            return _shouldUpdate;
        }

        internal void EnableUpdate()
        {
            _shouldUpdate = true;
            UpdateEntries();
            Device.StartTimer(_updateInterval, UpdateEntries);
        }

        internal void DisableUpdate()
        {
            _shouldUpdate = false;
        }

        internal void OnEntryAppearing(Entry entry)
        {
            if (entry == null)
                return;

            entry.UpdateCode(Time.GetCurrent());
            _visibleEntries.Add(entry);
        }

        internal void OnEntryDisappearing(Entry entry)
        {
            if (entry == null || (Device.RuntimePlatform == Device.Windows && !_shouldUpdate))
                return;

            _visibleEntries.Remove(entry);
        }
    }
}
