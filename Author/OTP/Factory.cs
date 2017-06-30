using Author.UI.Pages;
using Author.UI.ViewModels;
using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public static class Factory
    {
        static readonly Dictionary<byte, Func<string, IBaseGenerator>> Constructors =
            new Dictionary<byte, Func<string, IBaseGenerator>>
        {
            {Type.Hash, secret => new HashBased(secret)},
            {Type.Time, secret => new TimeBased(secret)},
            {Type.Steam, secret => new Steam(secret)},
            {Type.Blizzard, secret => new BlizzardApp(secret)},
            {Type.Authy, secret => new Authy(secret)}
        };

        static readonly Dictionary<byte, Action<EntryPage>> EntryPageSetups =
            new Dictionary<byte, Action<EntryPage>>
        {
            {Type.Hash, page =>
            {
                page.UnlockLengthPicker();
                page.LockPeriodSlider();
            }},
            {Type.Time, page =>
            {
                page.UnlockLengthPicker();
                page.UnlockPeriodSlider();
            }},
            {Type.Steam, page =>
            {
                page.LockLengthPicker(5);
                page.LockPeriodSlider(30);
            }},
            {Type.Blizzard, page =>
            {
                page.LockLengthPicker(8);
                page.LockPeriodSlider(30);
            }},
            {Type.Authy, page =>
            {
                page.LockLengthPicker(7);
                page.LockPeriodSlider(10);
            }}
        };

        public static IBaseGenerator CreateGenerator(byte type, string secret)
        {
            Func<string, IBaseGenerator> constructor = null;
            return Constructors.TryGetValue(type, out constructor)
                ? constructor(secret)
                : null;
        }

        public static void SetupEntryPage(byte type, EntryPage page)
        {
            Action<EntryPage> setup = null;
            if (EntryPageSetups.TryGetValue(type, out setup))
                setup(page);
        }
    }
}
