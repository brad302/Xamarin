using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;

#pragma warning disable CS4014
namespace BottomDrawer
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private double _penUltimateTranslationY;
        private double _lastTranslationY;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(2000);

                DoShowMenu();
            });
        }

        async void ShowMenu(object sender, System.EventArgs e)
        {
            await DoShowMenu();
        }

        async Task DoShowMenu()
        {
            ToggleOpaqueContainer(true, 300);
            MenuFrame.TranslateTo(0, MenuFrame.CornerRadius, 300, Easing.CubicInOut);
        }

        async void HideMenu(object sender, System.EventArgs e)
        {
            MenuFrame.TranslateTo(0, MenuFrame.Height, 300, Easing.CubicIn);
            ToggleOpaqueContainer(false, 300);
        }

        async void OnMenuFramePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            // For some reason, it's extremely jumpy on Android.
            // I'm yet to investigate or ask the question why.

            double translationY = e.TotalY < 10 ? 10 : e.TotalY;

            if (e.StatusType == GestureStatus.Running)
            {
                System.Diagnostics.Debug.WriteLine(e.TotalY);

                MenuFrame.TranslationY = translationY;

                _penUltimateTranslationY = _lastTranslationY;
                _lastTranslationY = e.TotalY;
            }

            if (e.StatusType == GestureStatus.Completed)
            {
                uint animationLength = 200;

                if (_penUltimateTranslationY < _lastTranslationY && !MenuFrame.TranslationY.Equals(10))
                {
                    await MenuFrame.TranslateTo(0, MenuFrame.Height, animationLength, Easing.CubicInOut);
                    ToggleOpaqueContainer(false, animationLength);
                }
                else
                    MenuFrame.TranslateTo(0, 10, animationLength, Easing.CubicInOut);
            }
        }

        async Task ToggleOpaqueContainer(bool show, uint animationLength)
        {
            if (show)
            {
                OpaqueContainer.IsVisible = true;
                await OpaqueContainer.FadeTo(0.6, animationLength);
            }
            else
            {
                await OpaqueContainer.FadeTo(0, animationLength);
                OpaqueContainer.IsVisible = false;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            width = width < 400 ? width : 400;

            AbsoluteLayout.SetLayoutBounds(MenuFrame, new Rectangle(0.5, 1, width, 300));
            AbsoluteLayout.SetLayoutFlags(MenuFrame, AbsoluteLayoutFlags.PositionProportional);
        }
    }
}
