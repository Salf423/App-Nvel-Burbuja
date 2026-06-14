using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace AccelerometerEssential.Droid;

[Activity(Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : MauiAppCompatActivity { }