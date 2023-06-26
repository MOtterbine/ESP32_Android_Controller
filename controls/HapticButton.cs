using ESP32_Android_Controller.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controls
{
    public class HapticButton : Button, ICustomButtonController
    {
        public new event EventHandler Clicked;

        public void SendTouched()
        {
            
            //   this.Clicked.Invoke(this, EventArgs.Empty);
        }

        public void SendLongClicked() { }

        public bool Vibrate
        {
            get { return (bool)GetValue(VibrateProperty); }
            set { SetValue(VibrateProperty, value); }
        }

        public static readonly BindableProperty VibrateProperty =
            BindableProperty.Create("Vibrate", typeof(bool), typeof(HapticButton), true);

    }
}
