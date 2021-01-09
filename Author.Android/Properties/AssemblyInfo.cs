using System.Reflection;
using System.Runtime.InteropServices;
using Android.App;
using Xamarin.Forms.Xaml;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Author.Android")]
[assembly: AssemblyDescription("A 2FA cross-platform application.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: Application(Debuggable = true)]
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: Application(Debuggable = false)]
#endif
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Author")]
[assembly: AssemblyCopyright("Copyright © 2016-2021 Daniel")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
