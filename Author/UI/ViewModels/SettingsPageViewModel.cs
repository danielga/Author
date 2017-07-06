using System;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class SettingsPageViewModel
    {
        public Command AcceptCommand { get; private set; }

        public SettingsPageViewModel()
        {
            AcceptCommand = new Command(OnAcceptTapped);
        }

        void OnAcceptTapped()
        {
            // Save settings
            Acr.UserDialogs.UserDialogs.Instance.Toast(
                new Acr.UserDialogs.ToastConfig("Saved settings")
                .SetDuration(TimeSpan.FromSeconds(3))
                .SetPosition(Acr.UserDialogs.ToastPosition.Bottom));
        }
    }
}
