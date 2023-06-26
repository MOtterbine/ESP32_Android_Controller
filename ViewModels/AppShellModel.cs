using Models;
using ESP32_Android_Controller.Interfaces;
using ESP32_Android_Controller;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using Communication;

namespace ESP32_Android_Controller.ViewModels
{

    public sealed class AppShellModel : ViewModelBase
    {
        private static readonly object padlock = new object();
        private ICommunicationDevice _CommunicationService = null;
        private IBlueToothService _BluetoothService = null;
        public int DeviceScreenWidth => this.ActivityControl.ScreenWidth;
        public int DeviceScreenHeight => this.ActivityControl.ScreenHeight;
        public double ScreenDensity => this.ActivityControl.ScreenDensity;
        public int DeviceScreenDPI => this.ActivityControl.ScreenDPI;

        private IPlatformAppControl ActivityControl = null;//{ get; } = DependencyService.Get<IPlatformAppControl>();

        public void CloseApp()
        {
            this.ActivityControl.Close();
        }

        public void RequestDevicePermissions(string[] permissions, int requestId = 0)
        {
            this.ActivityControl.RequestPermissions(permissions, requestId);
        }


        public void InitPlatformUI()
        {
            this.ActivityControl.ConfigureUI();
        }

        public void SetupAnimation(Image img)
        {
            if (img == null) return;
            parentAnimation = null;
            fadeOutAnimation = null;
            fadeInAnimation = null;

            parentAnimation = new Animation();
            fadeOutAnimation = new Animation(d => img.Opacity = d, 1, 0, Easing.Linear);
            fadeInAnimation = new Animation(d => img.Opacity = d, 0, 1, Easing.Linear);
            parentAnimation.Add(0, 0.5, fadeInAnimation);
            parentAnimation.Add(0.5, 1, fadeOutAnimation);
            parentAnimation.Commit(img, Constants.STRING_COMM_BLINKING_VISUAL_ELEMENT, 1, 200, repeat: () => true);
        }

        private Animation parentAnimation = null;
        private Animation fadeOutAnimation = null;
        private Animation fadeInAnimation = null;

        public void RemoveAnimation()
        {
            parentAnimation = null;
            fadeOutAnimation = null;
            fadeInAnimation = null;
    }


    public int HeaderPadding { get; private set; } = 0;
        public bool DeviceIsInitialized { get; set; }
        public static AppShellModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new AppShellModel();
                        }
                    }
                }
                return instance;
            }
        }
        private static AppShellModel instance = null;

        public ICommunicationDevice CommunicationService => this._CommunicationService;

        private void RefreshBluetoothDevices()
        {
            var btSrvc = DependencyService.Get<IBlueToothService>();
            var list = btSrvc.GetDeviceList();
            DeviceList.Clear();
            foreach (var item in list)
            {
                DeviceList.Add(item);
            }
        }

        public void SendHapticFeedback()
        {
            this.ActivityControl.InvokeHapticFeedback();
        }
        public bool CheckSelfPermission(string permission)
        {
            return this.ActivityControl.CheckSelfPermission(permission);
        }


        private AppShellModel()
        {
            ActivityControl = new ESP32_Android_Controller.PartialClasses.ActivityControlService() as IPlatformAppControl;
          //  ActivityControl.InvokeHapticFeedback();

            // this._BluetoothService = DependencyService.Get<IBlueToothService>();

            //this.SetCommMethod();

            var app = (App.Current as App);
            if (app != null) 
            {
                this.HeaderPadding = app.StatusBarHeight;
            }


    }


    private bool tabsEnabled = true;
        public bool TabsEnabled
        {
            get { return tabsEnabled; }
            set { SetProperty(ref tabsEnabled, value); }
        }



        public int IPPort
        {
            get { return ipPort; }
            set
            {
                SetProperty(ref ipPort, value);
                Preferences.Set(Constants.PREFS_KEY_IP_PORT, value);
                this.SetCommMethod();
            }
        }
        private int ipPort = Preferences.Get(Constants.PREFS_KEY_IP_PORT, Constants.DEFAULTS_WIFI_PORT);


        private string ipAddress = Preferences.Get(Constants.PREFS_KEY_IP_ADDRESS, Constants.DEFAULTS_WIFI_IPADDRESS);
        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                SetProperty(ref ipAddress, value);
                Preferences.Set(Constants.PREFS_KEY_IP_ADDRESS, value);

            }
        }


        public bool IsBluetooth => String.Compare(selectedCommMethod, Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR) == 0;

        private string selectedCommMethod = Preferences.Get(Constants.PREFS_KEY_DEVICE_COMM_TYPE, Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR);
        public string SelectedCommMethod
        {
            get { return selectedCommMethod; }
            set
            {
                SetProperty(ref selectedCommMethod, value);
                Preferences.Set(Constants.PREFS_KEY_DEVICE_COMM_TYPE, value);
                // this.IsBluetooth = (value == Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR);

                this.SetCommMethod();
                this.DeviceIsInitialized = false;
                if (this._CommunicationService == null) return;
                this.CommunicationChannel = this._CommunicationService.DeviceName;
            }
        }



        private string selectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);
        public string SelectedBluetoothDevice
        {
            get => selectedBluetoothDevice;
            set
            {
                SetProperty(ref selectedBluetoothDevice, value);
                Preferences.Set(Constants.PREFS_KEY_BLUETOOTH_DEVICE, value);
                this.DeviceIsInitialized = false;
                if (this._CommunicationService != null)this._CommunicationService.DeviceName = value;
            }
        }

        public string PresetBluetoothDevice => Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);

        private string communicationChannel = string.Empty;
        public string CommunicationChannel
        {
            get { return String.IsNullOrEmpty(this._CommunicationService.DeviceName) ? Constants.MSG_NOT_SELECTED : this._CommunicationService.DeviceName; }
            set
            {
                SetProperty(ref communicationChannel, value);
                this.DeviceIsInitialized = false;
                this._CommunicationService.DeviceName = value;
            }
        }

        public IList<string> DeviceList => this._BluetoothService.GetDeviceList(); 
     
        
        public void SelectDevice1()
        { 

            this.SelectedCommMethod = Preferences.Get(Constants.PREFS_KEY_DEVICE_COMM_TYPE, Constants.PREFS_BLUETOOTH_TYPE_DESCRIPTOR);

            if (this.IsBluetooth)
            {

                this._CommunicationService = this._BluetoothService as ICommunicationDevice;
                this.SelectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);
                this.CommunicationService.DeviceName = this.SelectedBluetoothDevice;

            }
            else
            {
                this._CommunicationService = new TCPSocket(this.IPAddress, this.IPPort, ConnectMethods.Client);
                this.CommunicationChannel = this.CommunicationService.ToString();
            }

        }

        public void SetCommMethod()
        {

                if (this._BluetoothService == null)
                {
                    this._BluetoothService = new ESP32_Android_Controller.Services.PartialMethods.AndroidBlueToothDevice();
                }



            if (this.IsBluetooth)
            {




                    this._CommunicationService = this._BluetoothService as ICommunicationDevice;
                    this.selectedBluetoothDevice = Preferences.Get(Constants.PREFS_KEY_BLUETOOTH_DEVICE, string.Empty);
                    this.CommunicationService.DeviceName = this.SelectedBluetoothDevice;
            }
            else
            {
              //  if (this._CommunicationService is IBlueToothService)
              //  {
                    this._CommunicationService = new TCPSocket(this.IPAddress, this.IPPort, ConnectMethods.Client);
                    this.communicationChannel = this.CommunicationService.ToString();
            //    }
            }

            // device hardware is stateful (as is the ICommunicationService)
            this.DeviceIsInitialized = false;

            this._CommunicationService?.Initialize();

            // Ensure communication object starts as closed
            this.CommunicationService?.Close();

        }

    }
}
