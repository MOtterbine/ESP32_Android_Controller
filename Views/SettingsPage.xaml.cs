using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Models;
using ESP32_Android_Controller.Interfaces;
using ESP32_Android_Controller.ViewModels;
using System.Runtime.CompilerServices;

namespace ESP32_Android_Controller;

// Learn more about making custom code visible in the Xamarin.Forms previewer
// by visiting https://aka.ms/xamarinforms-previewer
[DesignTimeVisible(false)]
public partial class SettingsPage : ContentPage
{
    private IViewModel viewModel = null;
    public SettingsPage()
    {
        InitializeComponent();

        this.viewModel = this.BindingContext as IViewModel;

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

    private void OnViewModelEvent(object sender, ESP32_Android_Controller.ViewModels.ViewModelEventArgs e)
    {
        switch (e.EventType)
        {
            case ESP32_Android_Controller.ViewModels.ViewModelEventEventTypes.NavigateTo:

                switch ((e.dataObject as string))
                {
                    case "MainPage":
                        Task.Run(async()=> { await Shell.Current.GoToAsync("//MainPage"); });
                        break;
                }
                break;

            case ESP32_Android_Controller.ViewModels.ViewModelEventEventTypes.ScrollTo:
                break;
        }
    }

    //protected override bool OnBackButtonPressed()
    //{
    //   // Navigation.PopAsync(false);
    //    return base.OnBackButtonPressed();
    //}

}