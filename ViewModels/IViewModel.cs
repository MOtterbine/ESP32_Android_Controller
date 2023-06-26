
using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace ESP32_Android_Controller.ViewModels
{
    public interface IViewModel
    {
        void Start();
        void Stop();
        bool IsBusy { get; set; }
        void CloseCommService();
        event ViewModelEvent ModelEvent;
        event RequestPopup NeedYesNoPopup;

    }
    public interface IEditableViewModel : IViewModel
    {
        void Edit(object editObject);
    }
}
