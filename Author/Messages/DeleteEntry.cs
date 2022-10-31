using Author.ViewModels;

namespace Author.Messages;

public class DeleteEntry
{
    public readonly MainPageEntryViewModel Entry;

    public DeleteEntry(MainPageEntryViewModel entry)
    {
        Entry = entry;
    }
}
