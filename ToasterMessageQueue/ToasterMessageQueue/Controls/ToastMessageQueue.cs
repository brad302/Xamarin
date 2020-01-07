using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToasterMessageQueue
{
    public enum ScrollDirection
    {
        Up,
        Down
    }

    public enum ToastPreformatOption
    {
        Success,
        Danger,
        Warning,
        Information,
        None
    }

    public enum ScaleAdjustmentType
    {
        Change,
        Absolute
    }

    public class MessagePosition
    {
        public float StartPosition { get; set; }
        public ScrollDirection ScrollDirection { get; set; }
        public int MaxMessagesShown { get; set; }
        public int MovementOffset { get; set; }
        public double MinimuHeight { get; set; }
        public double ScaleAdjustment { get; set; }
        public ScaleAdjustmentType ScaleAdjustmentType { get; set; }

        public MessagePosition()
        {
            StartPosition = 0.95f;
            ScrollDirection = ScrollDirection.Up;
            MaxMessagesShown = 8;
            MovementOffset = 1;
            MinimuHeight = 40;
            ScaleAdjustment = -0.01d;
            ScaleAdjustmentType = ScaleAdjustmentType.Change;
        }
    }

    public class MessageFrameFormat
    {
        public Color BorderColor { get; set; }
        public Color BackgroundColor { get; set; }
        public float CornerRadius { get; set; }

        public MessageFrameFormat()
        {
            BorderColor = Color.FromHex("#f0f0f0");
            BackgroundColor = Color.FromHex("#eeeee");
            CornerRadius = 5;
        }
    }

    public class MessageLabelFormat
    {
        public Color TextColor { get; set; }
        public Double FontSize { get; set; }
        public FontAttributes FontAttributes { get; set; }
        public TextDecorations TextDecorations { get; set; }
        public TextAlignment HorizontalTextAlignment { get; set; }
        public TextAlignment VerticalTextAlignment { get; set; }

        public MessageLabelFormat()
        {
            TextColor = Color.Black;
            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
            FontAttributes = FontAttributes.Bold;
            TextDecorations = TextDecorations.None;
            HorizontalTextAlignment = TextAlignment.Center;
            VerticalTextAlignment = TextAlignment.Center;
        }
    }

    public class MessageFormat
    {
        public MessageFrameFormat NewMessageFrameFormat { get; set; }
        public MessageLabelFormat NewMessageLabelFormat { get; set; }
        public ToastPreformatOption PreformatOption { get; set; }

        public MessageFormat()
        {
            NewMessageFrameFormat = new MessageFrameFormat();
            NewMessageLabelFormat = new MessageLabelFormat();
            PreformatOption = ToastPreformatOption.None;
        }
    }

    public class ToastQueueOptions
    {
        public MessageFormat NewMessageFormat { get; set; }
        public MessagePosition NewMessagePosition { get; set; }
        public int LifeInMilliseconds { get; set; }

        public ToastQueueOptions()
        {
            NewMessageFormat = new MessageFormat();
            NewMessagePosition = new MessagePosition();

            LifeInMilliseconds = 2000;
        }
    }

    public class ToastMessageQueue
    {
        private AbsoluteLayout _messageContainer;
        private List<Guid> _messages;

        public ToastQueueOptions Options { get; set; }

        public ToastMessageQueue(AbsoluteLayout container, ToastQueueOptions options = null)
        {
            Options = options == null ? new ToastQueueOptions() : options;
            _messageContainer = container;

            _messages = new List<Guid>();
        }

        public async Task AddMessage(string text)
        {
            var formattedString = new FormattedString();

            var span = new Span()
            {
                Text = text,
                FontAttributes = Options.NewMessageFormat.NewMessageLabelFormat.FontAttributes,
                TextDecorations = Options.NewMessageFormat.NewMessageLabelFormat.TextDecorations,
                TextColor = Options.NewMessageFormat.NewMessageLabelFormat.TextColor,
                FontSize = Options.NewMessageFormat.NewMessageLabelFormat.FontSize
            };

            formattedString.Spans.Add(span);

            await DoAddMessage(formattedString);
        }

        public async Task AddMessage(FormattedString text)
        {
            await DoAddMessage(text);
        }

        private async Task DoAddMessage(FormattedString text)
        {
            var messages = _messageContainer.Children.Where(view => _messages.Any(message => view.Id == message));

            var backgroundColor = Options.NewMessageFormat.NewMessageFrameFormat.BackgroundColor;
            var borderColor = Options.NewMessageFormat.NewMessageFrameFormat.BorderColor;
            Color textColor = new Color();

            switch (Options.NewMessageFormat.PreformatOption)
            {
                case ToastPreformatOption.Danger:
                    textColor = Color.FromHex("#721c24");
                    backgroundColor = Color.FromHex("#f8d7da");
                    borderColor = Color.FromHex("#f5c6cb");
                    break;

                case ToastPreformatOption.Information:
                    textColor = Color.FromHex("#004085");
                    backgroundColor = Color.FromHex("#cce5ff");
                    borderColor = Color.FromHex("#b8daff");
                    break;

                case ToastPreformatOption.Success:
                    textColor = Color.FromHex("#155724");
                    backgroundColor = Color.FromHex("#d4edda");
                    borderColor = Color.FromHex("#c3e6cb");
                    break;

                case ToastPreformatOption.Warning:
                    textColor = Color.FromHex("#856404");
                    backgroundColor = Color.FromHex("#fff3cd");
                    borderColor = Color.FromHex("#ffeeba");
                    break;

                case ToastPreformatOption.None:
                    break;

                default:
                    break;
            }

            if (Options.NewMessageFormat.PreformatOption != ToastPreformatOption.None)
                foreach (var span in text.Spans)
                    span.TextColor = textColor;

            var label = new Label()
            {
                FormattedText = text,
                VerticalTextAlignment = Options.NewMessageFormat.NewMessageLabelFormat.VerticalTextAlignment,
                HorizontalTextAlignment = Options.NewMessageFormat.NewMessageLabelFormat.HorizontalTextAlignment,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var frame = new Frame()
            {
                CornerRadius = Options.NewMessageFormat.NewMessageFrameFormat.CornerRadius,
                BackgroundColor = backgroundColor,
                BorderColor = borderColor,
                HasShadow = false,
                Opacity = 0,
                Padding = new Thickness(5),
                Content = label
            };

            _messages.Add(frame.Id);

            var layoutBounds = new Rectangle(0.5f, Options.NewMessagePosition.StartPosition, 0.95f, Options.NewMessagePosition.MinimuHeight);

            _messageContainer.Children.Add(
                frame,
                layoutBounds,
                AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.YProportional);

            layoutBounds.Height = label.Height + frame.Padding.Top + frame.Padding.Bottom + 15;
            layoutBounds.Height = layoutBounds.Height < Options.NewMessagePosition.MinimuHeight ? Options.NewMessagePosition.MinimuHeight : layoutBounds.Height;

            AbsoluteLayout.SetLayoutBounds(frame, layoutBounds);

            int index = 1;
            double opacityAdjustment = 1d / Options.NewMessagePosition.MaxMessagesShown;

            // Loop through all of the existing messages and translate them up/down.
            try
            {
                for (int i = messages.Count() - 1; i > 0; i--)
                {
                    index++;

                    var existingMessage = (Frame)messages.ElementAt(i - 1);
                    var message = existingMessage.Content as Label;

                    if (Options.NewMessagePosition.ScaleAdjustmentType == ScaleAdjustmentType.Change)
                        existingMessage.Scale = existingMessage.Scale + Options.NewMessagePosition.ScaleAdjustment;
                    else
                        existingMessage.Scale = Options.NewMessagePosition.ScaleAdjustment;

                    var translateToOffset = Options.NewMessagePosition.MovementOffset * existingMessage.Scale;

                    uint animationLength = 50;
                    var easing = Easing.CubicInOut;

                    if (Options.NewMessagePosition.ScrollDirection == ScrollDirection.Up)
                        existingMessage.TranslateTo(0, existingMessage.TranslationY - (existingMessage.Height * existingMessage.Scale) +
                            translateToOffset, animationLength, easing);
                    else
                        existingMessage.TranslateTo(0, existingMessage.TranslationY + (existingMessage.Height * existingMessage.Scale) +
                            translateToOffset, animationLength, easing);

                    if (index <= Options.NewMessagePosition.MaxMessagesShown)
                        existingMessage.Opacity = existingMessage.Opacity == 0.0d ? 0.0d : existingMessage.Opacity - opacityAdjustment;
                    else
                        existingMessage.Opacity = 0.0d;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(150);

            frame.FadeTo(1);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await Task.Delay(Options.LifeInMilliseconds + 250);
                    await frame.FadeTo(0);
                    _messageContainer.Children.Remove(frame);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
