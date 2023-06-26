using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media.Metrics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Java.Lang;
using Kotlin.Reflect;
using Microsoft.Maui.Platform;
using ESP32_Android_Controller.Interfaces;
using Renderers;


namespace ESP32_Android_Controller;

public delegate void OnConfigurationChanged(Configuration config);

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public event OnConfigurationChanged ConfigurationChanged =  null;

    public void FirePause()
    {
        if (this.Paused == null) return;
        this.Paused(this, EventArgs.Empty);
    }
    public event EventHandler Paused;

    public void FireResume()
    {
        if (this.Resumed == null) return;
        this.Resumed(this, EventArgs.Empty);
    }
    public event EventHandler Resumed;





    // public 
    public MainActivity()
    {
    }
    public override void OnConfigurationChanged(Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);
        if (this.ConfigurationChanged == null) return;
        this.ConfigurationChanged(newConfig);
    }

    protected override void OnResume()
    {
        base.OnResume();
        this.FireResume();
    }

    protected override void OnPause()
    {
        this.FirePause();
        base.OnPause();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {

        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {

                // Start permissions check...
                RequestPermissions(new string[] { "android.permission.BLUETOOTH_CONNECT" }, 15001); // the value 15001 is arbitrary and random

                //StartBluetoothPermissionCheck();
            }
            else
            {
                pApp.HasPermissions = true;
                pApp.FirePermissionsReadyEvent();
            }

            Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            Window.SetStatusBarColor(Android.Graphics.Color.Black);


            Window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds, Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);

            Window.SetNavigationBarColor(Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00));
            Window.SetStatusBarColor(Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00));
            //DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
        }

        SetupCustomControlMapping();

        base.OnCreate(savedInstanceState);

    }


    bool cancelHaptic = false;

    private void SetupCustomControlMapping()
    {

        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("CustomButtonEntry", (handler, view) =>
        {

            if (view is Controls.HapticButton)
            {
                handler.PlatformView.Touch += (s, e) =>
                {
                    switch(e.Event.Action)
                    {
                        case MotionEventActions.Down:
                            cancelHaptic = false;
                            if ((view as Controls.HapticButton).Vibrate)
                            {
                                Vibration.Vibrate(15);
                            }
                            break;
                        case MotionEventActions.Up:
                            if (cancelHaptic)
                            {
                                break;
                            }
                           // Vibration.Vibrate(15);
                            (view as Controls.HapticButton).SendClicked();
                            break;
                        case MotionEventActions.Cancel:
                            cancelHaptic = true;
                            break;
                    }
                };
                //handler.PlatformView.Click += (s, e) =>
                //{
                //    (view as ICustomButtonController)?.SendTouched();
                //};
              //  (handler.PlatformView).SetBackgroundColor(Android.Graphics.Color.Green);
            }


        });
    }


    App pApp = ((App)App.Current);

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {

      //  Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        switch (requestCode)
        {
            case 15001:
                // Check for Android sdk 31, or higher bluetooth permissions
                if (grantResults.Length > 0)
                {
                    if (grantResults[0] == 0) // good permission - this is a sdk 31, or higher, device
                    {
                        pApp.HasPermissions = true;
                        pApp.FirePermissionsReadyEvent();
                        return;
                    }
                }
                pApp.HasPermissions = false; // No device permissions at all...
                pApp.FirePermissionsReadyEvent();
                break;
        }

    }

    private async Task StartBluetoothPermissionCheck()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<OSBluetoothPermissions>();

        switch (status)
        {
            case PermissionStatus.Granted:
                pApp.HasPermissions = true;
                pApp.FirePermissionsReadyEvent();
                break;
            default :
                pApp.HasPermissions = false; // No device permissions at all...
                pApp.FirePermissionsReadyEvent();
                break;
        }
    }
}

public class OSBluetoothPermissions : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            //(global::Android.Manifest.Permission.Bluetooth, true), // pre android 12
            //(global::Android.Manifest.Permission.BluetoothAdmin, true), // pre android 12
            (global::Android.Manifest.Permission.BluetoothConnect, true) // only needed to be checked
        }.ToArray();
}


