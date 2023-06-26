
using ESP32_Android_Controller.ViewModels;

namespace ESP32_Android_Controller;

public partial class MainPage : ContentPage
{
	int count = 0;
    private IViewModel viewModel = null;

    public MainPage()
	{
		InitializeComponent();

        Title = "Main Page";
        viewModel = (this.BindingContext as IViewModel);
        //viewModel.ModelEvent += this.OnViewModelEvent;
        ((App)Application.Current).PermissionsReadyEvent += OnPermissionsResultReady;
        picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // this is a hack to force the picker back to the 'normal' state from disabled
        Picker p = (Picker)sender;
        p.Unfocus();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.ModelEvent += this.OnViewModelEvent;
        this.viewModel.Start();
    }

    protected override void OnDisappearing()
    {
        this.viewModel.IsBusy = true;
        viewModel.ModelEvent -= this.OnViewModelEvent;
        this.viewModel.Stop();
        base.OnDisappearing();
    }

    private void OnViewModelEvent(object sender, ViewModelEventArgs e)
    {
        switch (e.EventType)
        {
            case ViewModelEventEventTypes.NavigateTo:
                var curApp = ((App)Application.Current);

                switch ((e.dataObject as string))
                {
                    case "SettingsPage":
                        if (curApp.SettingsPage == null) curApp.SettingsPage = new SettingsPage();
                        Task.Run(async () => { await Shell.Current.GoToAsync("//SettingsPage"); });
                        break;
                }
                break;
            case ViewModelEventEventTypes.ScrollTo:
                break;
            case ViewModelEventEventTypes.PermissionsRequest:
                // AppShellViewModel.Instance.RequestDevicePermissions();
                break;
        }
    }

    // Permissions stuff
    private void OnPermissionsResultReady(object sender, EventArgs e)
    {
        //  unlock things based on permissions
    }

}

