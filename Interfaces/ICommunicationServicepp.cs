//using OS.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESP32_Android_Controller.Interfaces
{
    public interface ICommunicationServicepp
    {
        bool Initialize();
        Task<bool> Send(string text);
        bool Open();
        bool Close();
        bool IsOpen { get; }
        string DeviceName { get; set; }
    }
}
