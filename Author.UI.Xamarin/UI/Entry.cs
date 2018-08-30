using Author.OTP;
using Author.UI.Messages;
using Xamarin.Forms;

namespace Author.UI
{
    public class Entry : OTP.Entry
    {
        public Command EditCommand { get; private set; }
        public Command DeleteCommand { get; private set; }

        public Entry(Secret secret) : base(secret)
        {
            EditCommand = new Command(() =>
                MessagingCenter.Send(new RequestEditEntry { Entry = this }, "RequestEditEntry"));
            DeleteCommand = new Command(() =>
                MessagingCenter.Send(new DeleteEntry { Entry = this }, "DeleteEntry"));
        }
    }
}
