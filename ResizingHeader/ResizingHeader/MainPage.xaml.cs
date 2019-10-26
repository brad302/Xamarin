using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ResizingHeader
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private double _navigationBarHeightDefault = 200;

        private double _navigationBarHeight;
        public double NavigationBarHeight
        {
            get { return _navigationBarHeight; }
            set { _navigationBarHeight = value; OnPropertyChanged(); }
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

        private void MyListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            var height = (e.ScrollY > 100) ? 100 : (e.ScrollY < 0) ? 0 : e.ScrollY;

            NavigationBarHeight = _navigationBarHeightDefault - height;
        }
    }
}
