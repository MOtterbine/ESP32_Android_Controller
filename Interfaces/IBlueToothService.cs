//using OS.Communication;
using Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESP32_Android_Controller.Interfaces
{
    public interface IBlueToothService : ICommunicationDevice
    {
        IList<string> GetDeviceList();
        bool IsEnabled { get; }
        event BluetoothEvent DeviceEvent;
    }



}
