using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CameraVision
{
    public enum TextRecognitionLevelEnum
    {
        Accurate = 10,
        Fast = 1
    }

    public partial class CameraVisionPage : ContentPage
    {
        public Action ShowDocumentViewController;
        public TextRecognitionLevelEnum TextRecognitionLevel;

        private List<RecognizedTextItem> _itemList;
        public List<RecognizedTextItem> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; OnPropertyChanged(); }
        }

        public CameraVisionPage()
        {
            this.BindingContext = this;

            ItemList = new List<RecognizedTextItem>();

            InitializeComponent();
        }

        private async void CameraClicked(object sender, EventArgs e)
        {
            var recognitionLevel = await DisplayActionSheet("Select Recognition Level", "Cancel", null, new string[] { "Accurate", "Fast" });
            TextRecognitionLevel = (TextRecognitionLevelEnum)Enum.Parse(typeof(TextRecognitionLevelEnum), recognitionLevel);

            ShowDocumentViewController.Invoke();
        }

        public void LoadRecognizedTextItems(List<List<string>> items)
        {
            var itemList = items.Select(item => new RecognizedTextItem(item));
            ItemList = new List<RecognizedTextItem>(itemList);
        }
    }
}
