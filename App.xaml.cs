using ESP32_Android_Controller.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace ESP32_Android_Controller;


public delegate void PermissionsResultReady(object sender, EventArgs e);
public partial class App : Microsoft.Maui.Controls.Application
{
    public bool HasPermissions { get; set; } = false;
    private int statusBarHeight = 0;
    public int StatusBarHeight
    {
        get => this.statusBarHeight;
        set
        {
            this.statusBarHeight = value;

            OnPropertyChanged("StatusBarHeight");
            this.HeaderPadding = new Thickness(0, this.StatusBarHeight, 0, 0);
        }
    }
    public Thickness HeaderPadding
    {
        get { return (Thickness)GetValue(HeaderPaddingProperty); }
        set { SetValue(HeaderPaddingProperty, value); }
    }

    public static readonly BindableProperty HeaderPaddingProperty =
        BindableProperty.Create("HeaderPadding", typeof(Thickness), typeof(App), new Thickness(0));

    public App()
	{
        InitializeComponent();
        AssignStyles();

        MainPage = new AppShell();
        //MainPage = new NavigationPage(new MainPage());

        Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    private ResourceDictionary customStyles = null;
    void AssignStyles()
    {
        bool isSmall = AppShellModel.Instance.ScreenDensity > Constants.STYLES_BREAKPOINT_0;

        customStyles = new ResourceDictionary();

        if (isSmall)
        {
            customStyles.LoadFromXaml(typeof(Styles_sm));
            Resources.MergedDictionaries.Add(customStyles);
        }
        else
        {
            customStyles.LoadFromXaml(typeof(Styles_lg));
            Resources.Add(customStyles);
        }
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        AppShellModel.Instance.SetCommMethod();

        return base.CreateWindow(activationState);
    }

    public event PermissionsResultReady PermissionsReadyEvent;

    public void FirePermissionsReadyEvent()
    {
        if (this.PermissionsReadyEvent != null)
        {
            this.PermissionsReadyEvent(null, EventArgs.Empty);
        }
    }


    public SettingsPage SettingsPage { get; set; } = null;



}
