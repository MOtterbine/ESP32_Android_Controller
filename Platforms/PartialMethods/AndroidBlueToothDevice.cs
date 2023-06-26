using System.Text;
using Java.Util;
using Android.Bluetooth;
using ESP32_Android_Controller;
using Communication;
using ESP32_Android_Controller.Interfaces;

//[assembly: Microsoft.Maui.Controls.Dependency(typeof(OS.OBDII.Droid.AndroidBlueToothDevice))]

namespace ESP32_Android_Controller.PartialMethods;

public partial class AndroidBlueToothDevice : IBlueToothService, ICommunicationDevice
{

    // ELM327 uart rx buffer is 512 bytes
    public const int BUFFER_SIZE = 2048;
    public event BluetoothEvent DeviceEvent;

    public AndroidBlueToothDevice();

    public bool IsEnabled => this.bluetoothAdapter.IsEnabled;

    public IList<string> GetDeviceList();

    public bool IsOpen => this.bluetoothSocket == null ? false : this.bluetoothSocket.IsConnected;
    
    public string DeviceName { get; set; }

    public bool IsConnected => this.bluetoothSocket == null ? false : this.bluetoothSocket.IsConnected;

    public string Description => $"Bluetooth device: {this.DeviceName}";

    public partial bool Open();

    public partial bool Close();

    public event DeviceEvent CommunicationEvent;

    public partial bool Initialize();

    public override string ToString()
    {
        
        return this.DeviceName;
    }

    public partial byte[] Read();

    public partial async Task<bool> Send(string text)

    protected IAsyncResult _WriteAsyncResult = null;

    public partial async Task<bool> Send(byte[] buffer, int offset, int count);

}
