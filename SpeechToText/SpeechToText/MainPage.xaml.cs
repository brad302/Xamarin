using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechToText.Interfaces;
using Xamarin.Forms;

namespace SpeechToText
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        ISpeechToText _speechToText;
        bool _isCapturing;

        private string _buttonText;
        public string ButtonText
        {
            get { return _buttonText; }
            set { _buttonText = value; OnPropertyChanged(); }
        }

        private string _transcribedText;
        public string TranscribedText
        {
            get { return _transcribedText; }
            set { _transcribedText = value; OnPropertyChanged(); }
        }

        public MainPage()
        {
            this.BindingContext = this;

            ButtonText = "Start Recording";

            _speechToText = DependencyService.Get<ISpeechToText>();

            InitializeComponent();
        }

        private void RecordingButton_Clicked(object sender, EventArgs e)
        {
            if (!_isCapturing)
            {
                ButtonText = "Stop Recording";

                _isCapturing = true;
                TranscribedText = string.Empty;

                _speechToText.StartRecording((text) =>
                {
                    TranscribedText = text;
                });
            }
            else
            {
                ButtonText = "Start Recording";
                _isCapturing = false;

                _speechToText.StopRecording();
            }
        }
    }
}
