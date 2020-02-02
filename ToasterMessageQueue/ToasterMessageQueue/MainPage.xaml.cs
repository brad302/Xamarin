using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToasterMessageQueue
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private ToastMessageQueue _toast;
        private int _index;

        public MainPage()
        {
            InitializeComponent();

            _toast = new ToastMessageQueue(MainContainer, new ToastQueueOptions()
            {
                NewMessageFormat = new MessageFormat()
                {
                    NewMessageFrameFormat = new MessageFrameFormat()
                    {
                        BackgroundColor = Color.White,
                        BorderColor = Color.Silver,
                        CornerRadius = 5
                    },
                    NewMessageLabelFormat = new MessageLabelFormat()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        TextColor = Color.Black,
                        FontAttributes = FontAttributes.None
                    },
                    PreformatOption = ToastPreformatOption.None
                },
                NewMessagePosition = new MessagePosition()
                {
                    StartPosition = 0.95f,
                    ScrollDirection = ScrollDirection.Up,
                    MaxMessagesShown = 6,
                    MovementOffset = -3,
                    ScaleAdjustmentType = ScaleAdjustmentType.Change,
                    ScaleAdjustment = -0.01
                },
                LifeInMilliseconds = 3000
            });
        }

        private async void ShowMessages(object sender, EventArgs e)
        {
            _index++;
            _index = _index * 10;

            await _toast.AddMessage($"This is a new message to test the functionality with. {_index++}");

            var preformatValues = Enum.GetNames(typeof(ToastPreformatOption));
            var preformatValue = preformatValues[(new Random()).Next(0, preformatValues.Length)];

            Console.WriteLine(preformatValue);

            _toast.Options.NewMessageFormat.PreformatOption = (ToastPreformatOption)Enum.Parse(typeof(ToastPreformatOption), preformatValue);
        }
    }
}
