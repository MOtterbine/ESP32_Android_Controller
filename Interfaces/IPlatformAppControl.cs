
namespace ESP32_Android_Controller.Interfaces
{
    // For basic control of the platform
    public interface IPlatformAppControl
    {
        void Close();
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        int ScreenDPI { get; }
        double ScreenDensity { get; }
        void RequestPermissions(string[] permissions, int requestID = 0);
        bool CheckSelfPermission(string permission);
        void ConfigureUI();
        void InvokeHapticFeedback();

    }
}
