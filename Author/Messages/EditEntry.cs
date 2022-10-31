using Author.ViewModels;

namespace Author.Messages;

public class EditEntry
{
    public readonly MainPageEntryViewModel Entry;

    public EditEntry(MainPageEntryViewModel entry)
    {
        Entry = entry;
    }
}
