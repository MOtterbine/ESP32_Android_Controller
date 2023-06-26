using ESP32_Android_Controller.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controls
{
    public class CustomVerticalStackLayout :VerticalStackLayout, ICustomButtonController
    {
        public event EventHandler Touched;

        void ICustomButtonController.SendTouched()
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LongClicked;

        void ICustomButtonController.SendLongClicked()
        {
            LongClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Released;

        void ICustomButtonController.SendReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }
    }
}
