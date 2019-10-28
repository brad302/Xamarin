using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AnimatedSearchBar
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private double _navigationBarHeightDefault = 150;
        private bool _isSearchVisible;

        private double _navigationBarHeight;
        public double NavigationBarHeight
        {
            get { return _navigationBarHeight; }
            set { _navigationBarHeight = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ListItem> _listSource;
        public ObservableCollection<ListItem> ListSource
        {
            get { return _listSource; }
            set { _listSource = value; OnPropertyChanged(); }
        }

        public MainPage()
        {
            NavigationBarHeight = _navigationBarHeightDefault;

            ListSource = new ObservableCollection<ListItem>();

            for (int i = 1; i <= 100; i++)
                ListSource.Add(new ListItem($"Item {i.ToString()}"));

            this.BindingContext = this;
            InitializeComponent();
        }

        private async void SearchButton_Clicked(object sender, EventArgs e)
        {
            await ToggleSearchEntry();
        }

        private async void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        {
            await ToggleSearchEntry();
        }

        public async Task ToggleSearchEntry()
        {
            uint titleSpeed = 400;
            uint fadeSpeed = 250;
            var easing = Easing.CubicInOut;

            if (_isSearchVisible)
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    SearchEntry.FadeTo(0, fadeSpeed, Easing.CubicInOut);
                    await Task.Delay(50);
                    await TitleLabel.TranslateTo(0, -8, titleSpeed, easing);
                    _isSearchVisible = !_isSearchVisible;
                }
                else
                {
                    await DoSearch();
                }
            }
            else
            {
                TitleLabel.TranslateTo(0, -50, titleSpeed, Easing.CubicInOut);
                await Task.Delay(100);
                await SearchEntry.FadeTo(1, fadeSpeed, easing);
                _isSearchVisible = !_isSearchVisible;
            }
        }

        private async void SearchEntry_Completed(object sender, EventArgs e)
        {
            await DoSearch();
        }

        private async Task DoSearch()
        {
            await DisplayAlert("Message", $"Search for \"{SearchText}\"!", "OK");
        }
    }
}
