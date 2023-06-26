using System;
using System.Collections.Generic;
using System.Text;


namespace ESP32_Android_Controller.Interfaces
{

    public interface ICustomButtonController : IViewController
    {
        void SendTouched();

        void SendLongClicked();

        void SendReleased();
    }
}