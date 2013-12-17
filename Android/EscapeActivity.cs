using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Android
{
    [Activity (Label = "Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class EscapeActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            Escape.Activity = this;
            var g = new Escape ();
            SetContentView (g.Window);
            g.Run ();
        }
    }
}

