using Author.UI.ViewModels;
using System;
using System.Collections.Generic;

namespace Author.UI
{
    public static class Factory
    {
        private static readonly Dictionary<byte, Action<EntryPageViewModel>> EntryPageSetups =
            new Dictionary<byte, Action<EntryPageViewModel>>
        {
            {OTP.Type.Hash, vm =>
            {
                vm.Length = 6;
                vm.LengthPickerEnabled = true;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Time, vm =>
            {
                vm.Length = 6;
                vm.LengthPickerEnabled = true;

                vm.Period = 30;
                vm.PeriodSliderEnabled = true;
            }},
            {OTP.Type.Steam, vm =>
            {
                vm.Length = 5;
                vm.LengthPickerEnabled = false;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Blizzard, vm =>
            {
                vm.Length = 8;
                vm.LengthPickerEnabled = false;

                vm.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Authy, vm =>
            {
                vm.Length = 7;
                vm.LengthPickerEnabled = false;

                vm.Period = 10;
                vm.PeriodSliderEnabled = false;
            }}
        };

        public static void SetupEntryPage(byte type, EntryPageViewModel vm)
        {
            if (EntryPageSetups.TryGetValue(type, out var setup))
                setup(vm);
        }
    }
}
