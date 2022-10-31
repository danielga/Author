using Author.OTP;
using Author.Utility;
using Author.ViewModels;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Author;

public class EntryManager
{
    public ObservableCollection<MainPageEntryViewModel> Entries { get; } = new();

    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
    private readonly HashSet<MainPageEntryViewModel> _visibleEntries = new();
    private readonly Database _database = new();
    private bool _shouldUpdate = false;
    private IDispatcherTimer? _timer;

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

                Random random = new();
                for (int i = 0; i < 5; ++i)
                {
                    MainPageEntryViewModel entry = new(new()
                    {
                        Name = "Hello world " + i,
                        Digits = (byte)(4 + i),
                        Period = (byte)(5 + i),
                        Data = new(Enumerable.Repeat(Base32.ValidCharacters, 32).Select(s => s[random.Next(s.Length)]).ToArray())
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

    private void OnEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Task.Run(async () =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        foreach (MainPageEntryViewModel entry in e.NewItems)
                            await _database.AddEntry(entry.Secret);

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (MainPageEntryViewModel entry in e.OldItems)
                            await _database.RemoveEntry(entry.Secret);

                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                        foreach (MainPageEntryViewModel entry in e.OldItems)
                            await _database.RemoveEntry(entry.Secret);

                    if (e.NewItems != null)
                        foreach (MainPageEntryViewModel entry in e.NewItems)
                            await _database.AddEntry(entry.Secret);

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    await _database.RemoveEntries();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }
        });
    }

    private bool UpdateEntries()
    {
        long timestamp = Time.GetCurrent();
        foreach (MainPageEntryViewModel entry in _visibleEntries)
            entry.UpdateCode(timestamp);

        return _shouldUpdate;
    }

    public void EnableUpdate()
    {
        if (_shouldUpdate)
            return;

        _shouldUpdate = true;
        UpdateEntries();

        _timer = Application.Current?.Dispatcher.CreateTimer();
        if (_timer == null)
            return;

        _timer.Interval = _updateInterval;
        _timer.Tick += (_, _) =>
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!UpdateEntries())
                {
                    _timer.Stop();
                    _timer = null;
                }
            });
        };
        _timer.Start();
    }

    public void DisableUpdate()
    {
        _shouldUpdate = false;
    }

    public void OnEntryAppearing(MainPageEntryViewModel entry)
    {
        if (entry == null)
            return;

        entry.UpdateCode(Time.GetCurrent(), true);
        _visibleEntries.Add(entry);
    }

    public void OnEntryDisappearing(MainPageEntryViewModel entry)
    {
        if (entry == null || (DeviceInfo.Platform == DevicePlatform.WinUI && !_shouldUpdate))
            return;

        _visibleEntries.Remove(entry);
    }
}
