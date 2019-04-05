﻿using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using Xamarin.Forms.Xaml;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Author.iOS")]
[assembly: AssemblyDescription("A 2FA cross-platform application.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Author")]
[assembly: AssemblyCopyright("Copyright © 2016-2019 Daniel")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("72bdc44f-c588-44f3-b6df-9aace7daafdd")]

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
[assembly: NeutralResourcesLanguage("en")]

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
