using Author.ViewModels;

namespace Author.Messages;

public class AddEntry
{
    public readonly MainPageEntryViewModel Entry;

    public AddEntry(MainPageEntryViewModel entry)
    {
        Entry = entry;
    }
}
