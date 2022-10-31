using Author.ViewModels;

namespace Author.Messages;

public class RequestEditEntry
{
    public readonly MainPageEntryViewModel Entry;

    public RequestEditEntry(MainPageEntryViewModel entry)
    {
        Entry = entry;
    }
}
