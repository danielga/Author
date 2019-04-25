using Acr.UserDialogs;
using System;
using System.Drawing;

namespace Author.UI
{
    public class Notification : IDisposable
    {
        public enum Position
        {
            Top = 0,
            Bottom = 1
        }

        public class Action
        {
            public string Text;
            public Color TextColor;
            public System.Action Callback;
        }

        private readonly ToastConfig Config;
        private IDisposable Toast;

        public static Notification Create(string message)
        {
            return new Notification(message);
        }

        private Notification(string message)
        {
            Config = new ToastConfig(message);
        }

        public Notification SetAction(Action action)
        {
            Config.Action = new ToastAction
            {
                Text = action.Text,
                TextColor = action.TextColor,
                Action = action.Callback
            };
            return this;
        }

        public Notification SetAction(System.Action action)
        {
            Config.Action = new ToastAction
            {
                Action = () => action()
            };
            return this;
        }

        public Notification SetDuration(TimeSpan? duration)
        {
            Config.SetDuration(duration);
            return this;
        }

        public Notification SetDuration(int ms)
        {
            Config.SetDuration(ms);
            return this;
        }

        public Notification SetPosition(Position position)
        {
            Config.SetPosition((ToastPosition)position);
            return this;
        }

        public Notification SetBackgroundColor(Color color)
        {
            Config.SetBackgroundColor(color);
            return this;
        }

        public Notification SetMessageTextColor(Color color)
        {
            Config.SetMessageTextColor(color);
            return this;
        }

        public Notification SetIcon(string icon)
        {
            Config.SetIcon(icon);
            return this;
        }

        public void Show()
        {
            Toast = UserDialogs.Instance.Toast(Config);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Toast != null)
            {
                Toast.Dispose();
                Toast = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
