using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CameraVision
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var mainPage = new NavigationPage(new CameraVisionPage());

            mainPage.BarBackgroundColor = Color.White;

            MainPage = mainPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
