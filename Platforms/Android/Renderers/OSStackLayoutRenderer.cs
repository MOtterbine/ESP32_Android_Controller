using Android.Renderscripts;
using Android.Service.Controls;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using Kotlin.Reflect;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using System;
using System.ComponentModel;
using StackLayout = Microsoft.Maui.Controls.Compatibility.StackLayout;

namespace Renderers;

class OSStackLayoutRenderer : ViewRenderer<StackLayout, Android.Views.View>
{
    public OSStackLayoutRenderer(Android.Content.Context context) : base(context)
    {

    }

    protected override Android.Views.View CreateNativeControl()
    {
        return base.CreateNativeControl();
    }
    protected override int[] OnCreateDrawableState(int extraSpace)
    {
        return base.OnCreateDrawableState(extraSpace);
    }


    protected override void OnElementChanged(ElementChangedEventArgs<StackLayout> e)
    {
        base.OnElementChanged(e);
        
        if (e.OldElement != null)
        {
            Control.LongClick -= Control_LongClick;
        }

        if (e.NewElement != null)
        {
           
            if (Control == null)
            {
                // this native control will convey the long click event back to us
                SetNativeControl(CreateView());
            }

            if (Control != null)
            {
                Control.LongClick += Control_LongClick;
            }
        }
    }

    private void Control_LongClick(object sender, LongClickEventArgs e)
    {
        ((ESP32_Android_Controller.Interfaces.ICustomButtonController)this?.Element)?.SendLongClicked();
    }
    private Android.Views.View CreateView()
    {
        var newView = new Android.Views.View(Context);
        
        newView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

        return newView;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }

        base.Dispose(disposing);
    }

}