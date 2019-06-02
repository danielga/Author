﻿using Author.OTP;
using Author.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Author.UI
{
    public class EntryManager
    {
        public ObservableCollection<Entry> Entries { get; } = new ObservableCollection<Entry>();

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
        private readonly HashSet<Entry> _visibleEntries = new HashSet<Entry>();
        private bool _shouldUpdate = false;

        public EntryManager()
        {
            Task.Run(async () =>
            {
                List<Entry> entries = null;

                try
                {
                    entries = new List<Entry>();
                    foreach (Secret secret in await Database.GetEntries())
                    {
                        entries.Add(new Entry(secret));
                    }
                }
                catch (Exception)
                { }

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (entries != null && entries.Count != 0)
                    {
                        Entries.Clear();
                        foreach (Entry entry in entries)
                        {
                            Entries.Add(entry);
                        }
                    }

                    // TODO: Check if collection changes can happen before this is executed
                    Entries.CollectionChanged += OnEntriesChanged;
                });
            });
        }

        private void OnEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (Entry entry in e.NewItems)
                        {
                            await Database.AddEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (Entry entry in e.OldItems)
                        {
                            await Database.RemoveEntry(entry.Secret);
                        }

                        foreach (Entry entry in e.NewItems)
                        {
                            await Database.AddEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (Entry entry in e.OldItems)
                        {
                            await Database.RemoveEntry(entry.Secret);
                        }

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        await Database.RemoveEntries();
                        break;
                }
            });
        }

        private bool UpdateEntries()
        {
            long timestamp = Time.GetCurrent();
            foreach (Entry entry in _visibleEntries)
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

        public void OnEntryAppearing(Entry entry)
        {
            if (entry == null)
            {
                return;
            }

            entry.UpdateCode(Time.GetCurrent(), true);
            _visibleEntries.Add(entry);
        }

        public void OnEntryDisappearing(Entry entry)
        {
            if (entry == null || (Device.RuntimePlatform == Device.UWP && !_shouldUpdate))
            {
                return;
            }

            _visibleEntries.Remove(entry);
        }
    }
}
