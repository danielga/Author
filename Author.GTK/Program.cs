using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace Author.GTK
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            GLib.ExceptionManager.UnhandledException += e => Console.WriteLine(e.ExceptionObject.ToString());

            Gtk.Application.Init("Author", ref args);
            Forms.Init();

            FormsWindow window = new FormsWindow();
            window.LoadApplication(new UI.Pages.App());
            window.SetApplicationTitle("Author");
            window.Show();

            Gtk.Application.Run();
        }
    }
}
