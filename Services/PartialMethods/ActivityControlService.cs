using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ESP32_Android_Controller.Interfaces;
using System;
using System.Resources;


namespace ESP32_Android_Controller.PartialClasses;

public partial class ActivityControlService 
{
    // ***** Interface methods don't need to be set as partials *****


    // Non-Interface Methods must be set as partial methods
    public partial void Close();

    public partial void RequestPermissions(string[] permissions, int requestID);

    public partial bool CheckSelfPermission(string permission);

    public partial void ConfigureUI();

    public partial void InvokeHapticFeedback();

}