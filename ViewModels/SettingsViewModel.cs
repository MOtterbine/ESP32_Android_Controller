using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Models;
using ESP32_Android_Controller.Interfaces;
using ESP32_Android_Controller.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.Maui.Layouts;

namespace ESP32_Android_Controller.ViewModels
{

    public class SettingsViewModel : ViewModelBase, IViewModel
    {
        public event ViewModelEvent ModelEvent;
        public event RequestPopup NeedYesNoPopup;

        public SettingsViewModel() 
        {
            this.isEditing = false;
            Title = "Settings";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.thoughtpill.com/ODB2AppDownload"));

        }


        public ICommand OpenWebCommand { get; }

        protected SynchronizationContext syncContext;


        public ICommand ReloadBTDevicesCommand => new Command(()=> {
           // this.RefreshBluetoothDevices();
        
        });


        #region NavigateHomeCommand

        public ICommand NavigateHomeCommand => new Command(() =>
        {

            if (this.ModelEvent != null)
            {
                this.IsBusy = true;
                using (ViewModelEventArgs evt = new ViewModelEventArgs())
                {
                    evt.EventType = ViewModelEventEventTypes.NavigateTo;
                    evt.dataObject = "MainPage";
                    this.ModelEvent(this, evt);
                }
            }
        });

        #endregion NavigateHomeCommand

        public string SelectedCommMethod
        {
            get => AppShellModel.Instance.SelectedCommMethod;
            set {
                AppShellModel.Instance.SelectedCommMethod = value;
                OnPropertyChanged("IsBluetooth");
                OnPropertyChanged("PresetBluetoothDevice");
                OnPropertyChanged("EditButtonRow");
            }
        
        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }
        private bool isBusy = false;


        public bool IsBluetooth
        {
            get => AppShellModel.Instance.IsBluetooth;
        }

        public int IPPort
        {
            get => AppShellModel.Instance.IPPort;
            set { AppShellModel.Instance.IPPort = value; }
        }
        public int EditButtonRow => this.IsBluetooth ? 5 : 6;

        public string IPAddress
        {
            get => AppShellModel.Instance.IPAddress;
            set { AppShellModel.Instance.IPAddress = value; }
        }


        public IList<string> DeviceList => AppShellModel.Instance.DeviceList; 
        
        //private IList<string> deviceList = new List<string>();

        private string printMessage = string.Empty;
        public string PrintMessage
        {
            get { return printMessage; }
            set { SetProperty(ref printMessage, value); }
        }

        public string SelectedBluetoothDevice
        {
            get => AppShellModel.Instance.SelectedBluetoothDevice;
            set 
            {
                AppShellModel.Instance.SelectedBluetoothDevice = value;
                OnPropertyChanged("SelectedBluetoothDevice"); 
            }
        }
        public string selectedBluetoothDevice = AppShellModel.Instance.PresetBluetoothDevice;
        //public string SelectedBluetoothDevice
        //{
        //    get => selectedBluetoothDevice;
        //    set { SetProperty(ref selectedBluetoothDevice, value); }
        //}
        //public string selectedBluetoothDevice = AppShellModel.Instance.SelectedBluetoothDevice;

        public string PresetBluetoothDevice => AppShellModel.Instance.PresetBluetoothDevice;

        private string saveCancelButtonText = "Edit";
        public string EditSaveButtonText
        {
            get { return saveCancelButtonText; }
            set { SetProperty(ref saveCancelButtonText, value); }
        }

        private bool isEditing = false;
        public bool IsEditing
        {
            get { return isEditing; }
            set 
            { 
                SetProperty(ref isEditing, value);
                this.EditSaveButtonText = value?"Done":"Edit";
                OnPropertyChanged("SelectedCommMethod");
                OnPropertyChanged("SelectedBluetoothDevice");
                OnPropertyChanged("IPAddress");
                OnPropertyChanged("IPPort");
                OnPropertyChanged("EditButtonRow");


            }
        }


        private bool canSend = false;
        public bool CanSend
        {
            get { return canSend; }
            set { SetProperty(ref canSend, value); }
        }


        /// <summary>
        /// This method will call the first queued command which should result in a bluetooth device event
        /// where the next queued command (if one exists) can be called
        /// </summary>

        public ICommand EditSaveCommand => new Command(async () => {
            if (this.IsEditing)
            {
                await Task.Run(() =>
                {
                    OnPropertyChanged("PresetBluetoothDevice");
                    this.ValidateSettings();
                     AppShellModel.Instance.SelectedBluetoothDevice = this.SelectedBluetoothDevice;
                    this.IsEditing = false;
                    AppShellModel.Instance.SetCommMethod();
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    this.IsEditing = true;
                });
            }
        });

        

        private bool ValidateSettings()
        {
            // add code to ensure all values are within correct ranges
            return true;
        }

        public void Start()
        {

            this.IsEditing = false;
          //  var tSel = this.SelectedBluetoothDevice;
            //OnPropertyChanged("DeviceList");
            //OnPropertyChanged("PresetBluetoothDevice");
           // this.SelectedBluetoothDevice = tSel;

            this.IsBusy = false;
        }
        public void Initialize()
        {
            this.IsBusy = false;
        }

        // to satisfy IViewModel
        public void CloseCommService()
        {

            //AppShellModel.Instance.SendHapticFeedback();

        }
        public void Stop()
        {
            this.CloseCommService();
        }

    }
}



