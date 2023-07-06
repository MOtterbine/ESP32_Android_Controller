using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Communication;
using ESP32_Android_Controller.Models;
using Models;

namespace ESP32_Android_Controller.ViewModels;

public class MainPageViewModel : ViewModelBase, IViewModel
{

    public List<SimpleCommand> CommandList => this._commands;
    private List<SimpleCommand> _commands = new List<SimpleCommand> {
        new SimpleCommand("LED On","AT01\r"),
        new SimpleCommand("LED Off","AT02\r"),
        new SimpleCommand("Get Device Name","ATN\r"),
        new SimpleCommand("Get LED Status","ATS\r"),
        new SimpleCommand("Get Version","ATV\r"),
        new SimpleCommand("Reset","ATZ\r")
    };
    public SimpleCommand SelectedCommand
    {
        get => this._SelectedComand;
        set => base.SetProperty(ref this._SelectedComand, value);
    }
    private SimpleCommand _SelectedComand = null;
    ICommunicationDevice comDev => AppShellModel.Instance.CommunicationService;

    public ICommand SendCommand { get; }
    public ICommand LEDOnCommand { get; }
    public ICommand LEDOffCommand { get; }
    public ICommand GoToSettingsPageCommand { get; }

    private TimerCallback _CommTimeoutHandler = null;


    public MainPageViewModel()
    {

        SendCommand = new Command(async () =>
        {
            this.InputIsBlocked = true;
            await Task.Run(() => {
                if (Open())
                {
                    if (SelectedCommand.Name == "Reset") wasReset = true;
                    this.comDev.Send(SelectedCommand.Command);
                }
            });
        }, () => SelectedCommand != null);

        LEDOnCommand = new Command(async () =>
        {
            this.InputIsBlocked = true;
            await Task.Run(() => {
                if (Open())
                {

                    this.comDev.Send(CommandList[0].Command);
                }
            });
        }, () => SelectedCommand != null);

        LEDOffCommand = new Command(async () =>
        {
            this.InputIsBlocked = true;
            await Task.Run(() =>
            {
                if (Open())
                {
                    this.comDev.Send(CommandList[1].Command);
                }
            });
        }, () => !this.InputIsBlocked);

        GoToSettingsPageCommand = new Command(async () => {
            InputIsBlocked = true;
            this.IsBusy = true;
              await Task.Run(() => {
                if (this.ModelEvent != null)
                {
                    this.IsBusy = true;
                    using (ViewModelEventArgs evt = new ViewModelEventArgs())
                    {
                        evt.EventType = ViewModelEventEventTypes.NavigateTo;
                        evt.dataObject = "SettingsPage";
                        this.ModelEvent(this, evt);
                    }
                }
               });
        }, () => !this.InputIsBlocked);


        // just set command to first on the list...
        this.SelectedCommand = this.CommandList[0];
       
        
        this.InputIsBlocked = false;
        this.IsBusy = false;

        this._CommTimeoutHandler = this.OnCommTimeout;
        _CommTimer = new System.Threading.Timer(_CommTimeoutHandler, null, Timeout.Infinite, Timeout.Infinite);
    }

    #region IViewModel

    public event ViewModelEvent ModelEvent;
    public event RequestPopup NeedYesNoPopup;
    public void CloseCommService()
    {
        CancelCommTimout();
        this.comDev?.Close();
        this.InputIsBlocked = false;
    }
    /// <summary>
    /// Disable entire screen
    /// </summary>
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            SetProperty(ref isBusy, value);
            OnPropertyChanged("InputIsBlocked");

        }
    }
    private bool isBusy = false;

    /// <summary>
    /// Disable user inputs
    /// </summary>
    public bool InputIsBlocked
    {
        get => inputIsBlocked;
        set => SetProperty(ref inputIsBlocked, value);
    }
    private bool inputIsBlocked = false;

    public void Start()
    {
        if (this.comDev == null)
        {
            AppShellModel.Instance.SetCommMethod();
        }
        this.IsBusy = false;
    }
    public void Stop()
    { 
        this.CloseCommService(); 
    }

    #endregion IViewModel

    private bool wasReset = false;

    private async System.Threading.Tasks.Task OnCommEvent(object sender, ChannelEventArgs e)
    {
        await Task.Delay(0);
        OnPropertyChanged("InputIsBlocked");

        switch (e.Event)
        {
            case CommunicationEvents.Receive:
                StatusDescription += Encoding.ASCII.GetString(e.data);//.Replace("\r", string.Empty);
                ResetCommTimout();
                break;
            case CommunicationEvents.ReceiveEnd:
                StatusDescription += Encoding.ASCII.GetString(e.data);//.Replace("\r", string.Empty);
                CloseCommService();
                break;
            case CommunicationEvents.ConnectedAsClient:
                CancelCommTimout();
                //StatusDescription = "Connected";
                break;
            case CommunicationEvents.Disconnected:
                CancelCommTimout();
                this.comDev.CommunicationEvent -= OnCommEvent;
                //StatusDescription = "Disconnected";
                this.InputIsBlocked = false;
                break;
            case CommunicationEvents.Error:
                this.comDev.CommunicationEvent -= OnCommEvent;
                this.CloseCommService();
                if (!string.IsNullOrEmpty(e.Description)) StatusDescription = e.Description;
                break;
        }
        //wasReset = false;
    }

    protected void OnCommTimeout(object sender)
    {

        this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
        // Retrying
        if (++this._RetryCounter < Constants.DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT)
        {
            this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);

            return;
        }

        // Timed out
        this._RetryCounter = 0;
        this.ErrorExists = true;
        this.CloseCommService();
        this.StatusDescription = Constants.MSG_NO_RESPONSE_SYSTEM;
    }

    protected void ResetCommTimout()
    {
        // Reset RX timeout timer
        this._CommTimer.Change(Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT, Constants.DEFAULT_COMM_NO_RESPONSE_TIMEOUT);
        this._RetryCounter = 0;
    }
    protected void CancelCommTimout()
    {
        // Reset RX timeout timer
        this._CommTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this._RetryCounter = 0;
    }
    protected Timer _CommTimer = null;
    protected int _RetryCounter = 0;
    protected int ConnectTimeoutCount = 0;
    public bool ErrorExists
    {
        get { return errorExists; }
        set
        {
            SetProperty(ref errorExists, value);
        }
    }
    private bool errorExists = false;












    public string BigDescription { get => "ESP32 Controller"; }
    public string SmallDescription { get => "Customizable App"; }


    public void SetName(string title)
    {
        Title = title;
    }




    #region RunCommand

    public ICommand OpenCommand => new RelayCommand(param =>
    {
        Task.Run(() =>
        {
            this.Open();
        });
    })
    {

    };

    private bool Open()
    {
        if (this.comDev == null) return false;
        this.StatusDescription = string.Empty;
        this.comDev.CommunicationEvent += OnCommEvent;
        if (this.comDev.Open())
        {
            ResetCommTimout();
            return true;
        }
        this.comDev.CommunicationEvent -= OnCommEvent;
        StatusDescription = $"Unable to open {this.comDev.DeviceName}";
        return false;
        }

    public ICommand CloseCommand => new RelayCommand(param => 
    {
        AppShellModel.Instance.CommunicationService.Close();
        this.comDev.CommunicationEvent -= OnCommEvent;
    });

    #endregion RunCommand




}
