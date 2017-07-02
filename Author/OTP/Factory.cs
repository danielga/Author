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

        static readonly Dictionary<byte, Action<EntryPageViewModel>> EntryPageSetups =
            new Dictionary<byte, Action<EntryPageViewModel>>
        {
            {Type.Hash, vm =>
            {
                vm.Length = 6;
                vm.LengthPickerEnabled = true;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {Type.Time, vm =>
            {
                vm.Length = 6;
                vm.LengthPickerEnabled = true;

                vm.Period = 30;
                vm.PeriodSliderEnabled = true;
            }},
            {Type.Steam, vm =>
            {
                vm.Length = 5;
                vm.LengthPickerEnabled = false;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {Type.Blizzard, vm =>
            {
                vm.Length = 8;
                vm.LengthPickerEnabled = false;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {Type.Authy, vm =>
            {
                vm.Length = 7;
                vm.LengthPickerEnabled = false;

                vm.Period = 10;
                vm.PeriodSliderEnabled = false;
            }}
        };

        public static IBaseGenerator CreateGenerator(byte type, string secret)
        {
            Func<string, IBaseGenerator> constructor = null;
            return Constructors.TryGetValue(type, out constructor)
                ? constructor(secret)
                : null;
        }

        public static void SetupEntryPage(byte type, EntryPageViewModel vm)
        {
            Action<EntryPageViewModel> setup = null;
            if (EntryPageSetups.TryGetValue(type, out setup))
                setup(vm);
        }
    }
}
