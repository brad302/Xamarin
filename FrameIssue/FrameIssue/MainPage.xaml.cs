using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FrameIssue
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async Task ShowInformationFrame()
        {             if (InformationFrame.IsVisible)                 return;              InformationFrame.IsVisible = true;             OpaqueContainer.IsVisible = true;             InformationFrame.RotateYTo(-90, 0);             await InformationFrame.TranslateTo(-200, 0, 0);              OpaqueContainer.FadeTo(0.5, 250);             InformationFrame.FadeTo(1, 250);             InformationFrame.RotateYTo(0, 250, Easing.CubicOut);             await InformationFrame.TranslateTo(0, 0, 250, Easing.CubicOut);         }

        async void HideInformationFrame(object sender, System.EventArgs e)
        {
            OpaqueContainer.FadeTo(0, 250);             InformationFrame.TranslateTo(200, 0, 250, Easing.CubicIn);             InformationFrame.RotateYTo(90, 250, Easing.CubicIn);
            await InformationFrame.FadeTo(0, 250);              InformationFrame.IsVisible = false;             OpaqueContainer.IsVisible = false;
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await ShowInformationFrame();
        }
    }
}
