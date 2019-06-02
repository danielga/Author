using Author.UI.ViewModels;
using System;
using System.Collections.Generic;

namespace Author.UI
{
    public static class Factory
    {
        private static readonly Dictionary<OTP.Type, Action<EntryPageViewModel>> EntryPageSetups =
            new Dictionary<OTP.Type, Action<EntryPageViewModel>>
        {
            {OTP.Type.Hash, vm =>
            {
                vm.Entry.Secret.Digits = 6;
                vm.LengthPickerEnabled = true;

                vm.Entry.Secret.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Time, vm =>
            {
                vm.Entry.Secret.Digits = 6;
                vm.LengthPickerEnabled = true;

                vm.Entry.Secret.Period = 30;
                vm.PeriodSliderEnabled = true;
            }},
            {OTP.Type.Steam, vm =>
            {
                vm.Entry.Secret.Digits = 5;
                vm.LengthPickerEnabled = false;

                vm.Entry.Secret.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Blizzard, vm =>
            {
                vm.Entry.Secret.Digits = 8;
                vm.LengthPickerEnabled = false;

                vm.Entry.Secret.Period = 30;
                vm.PeriodSliderEnabled = false;
            }},
            {OTP.Type.Authy, vm =>
            {
                vm.Entry.Secret.Digits = 7;
                vm.LengthPickerEnabled = false;

                vm.Entry.Secret.Period = 10;
                vm.PeriodSliderEnabled = false;
            }}
        };

        public static void SetupEntryPage(OTP.Type type, EntryPageViewModel vm)
        {
            if (EntryPageSetups.TryGetValue(type, out var setup))
                setup(vm);
        }
    }
}
