using Android.Content.Res;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ESP32_Android_Controller.Interfaces;



namespace ESP32_Android_Controller.PartialClasses;

public partial class ActivityControlService : IPlatformAppControl
{
    void IPlatformAppControl.Close()
    {
        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.FinishAndRemoveTask();

        // doesn't free all memory - use android studio profiler to verify
        //  Xamarin.Essentials.Platform.CurrentActivity.FinishAndRemoveTask();

    }

    int IPlatformAppControl.ScreenWidth =>  Convert.ToInt32(DeviceDisplay.Current.MainDisplayInfo.Width);
    int IPlatformAppControl.ScreenHeight => Convert.ToInt32(DeviceDisplay.Current.MainDisplayInfo.Height);
    double IPlatformAppControl.ScreenDensity => Resources.System.DisplayMetrics.Density;
    int IPlatformAppControl.ScreenDPI => (int)(DeviceDisplay.Current.MainDisplayInfo.Width/DeviceDisplay.Current.MainDisplayInfo.Density);

    void IPlatformAppControl.RequestPermissions(string[] permissions, int requestID)
    {
        Platform.CurrentActivity.RequestPermissions(permissions, requestID);
    }

    bool IPlatformAppControl.CheckSelfPermission(string permission)
    {

        var j = Platform.CurrentActivity.CheckSelfPermission(permission);
        return j == 0;
    }

    void IPlatformAppControl.ConfigureUI()
    {
        // allows for scrolling when android keyboard impinges on the screen
        // Microsoft.Maui.Controls.Application.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    void IPlatformAppControl.InvokeHapticFeedback()
    {
        try
        {
            // Perform click feedback
            //  HapticFeedback.Perform(HapticFeedbackType.Click);

            // Or use long press    
            //HapticFeedback.Perform(HapticFeedbackType.LongPress);

            this.Vibrate(40);

        }
        catch (FeatureNotSupportedException )
        {
            // Feature not supported on device
        }
        catch (Exception)
        {
            // Other error has occurred.
        }
    }

    private void Vibrate(int milliseconds)
    {
        try
        {
            Vibration.Default.Vibrate(milliseconds);
        }
        catch (FeatureNotSupportedException ex)
        {
            // Feature not supported on device
        }
        catch (Exception ex)
        {
            // Other error has occurred.
        }
    }

}