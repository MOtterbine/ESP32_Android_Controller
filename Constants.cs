using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP32_Android_Controller;

public class Constants
{
    public const string MSG_NO_RESPONSE_SYSTEM = "No Response From System";


    public const string DEFAULTS_WIFI_IPADDRESS = "192.168.0.10";
    public const int DEFAULTS_WIFI_PORT = 35000;
    public const int DEFAULT_COMM_NO_RESPONSE_TIMEOUT = 1200;
    public const int DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT = 3;

    public const string PREFS_KEY_BLUETOOTH_DEVICE = "BluetoothDevice";
    public const string PREFS_KEY_IP_ADDRESS = "IPAddress";
    public const string PREFS_KEY_IP_PORT = "IPPort";
    public const string PREFS_KEY_DEVICE_COMM_TYPE = "CommunicationType";
    public const string PREFS_BLUETOOTH_TYPE_DESCRIPTOR = "Bluetooth";


    public const string MSG_NOT_SELECTED = "Not Selected";

    public const string STRING_COMM_BLINKING_VISUAL_ELEMENT = "BlinkingVisualElement";

    public const double STYLES_BREAKPOINT_0 = 3.9;



}
