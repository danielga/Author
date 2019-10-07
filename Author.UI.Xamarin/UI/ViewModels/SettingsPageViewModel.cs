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

            try
            {
                Notification.Create("Saved settings")
                    .SetDuration(TimeSpan.FromSeconds(3))
                    .SetPosition(Notification.Position.Bottom)
                    .Show();
            }
            catch
            { }
        }
    }
}
