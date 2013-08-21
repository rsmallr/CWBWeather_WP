using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using CWBWeather.ViewModel;
using CWBWeather.Model;
using Microsoft.Phone.Shell;

namespace CWBWeather
{
    public partial class Page1 : PhoneApplicationPage
    {
        //private CityTown mRecords;
        List<CityTown> mCities;

        public Page1()
        {
            InitializeComponent();

            ThemeManager.ToLightTheme();

            //Loaded += OnLoaded;
            this.DataContext = new CityTownViewModel();
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            CityList.Items.Clear();
            //CityList.SelectionChanged += SelectionChanged;

            //mRecords = Utils.getCityTown();
            //mCities = mRecords.Cities;
/*            foreach (var city in mRecords.Cities)
            {
                TextBlock cityText = new TextBlock();
                //cityText.Text = city.COUN_NA;
                cityText.Style = (Style)Resources["listBoxTextStyle"];
                CityList.Items.Add(cityText);
            }*/
            CityList.ItemsSource = mCities;
        }

        void CityListBox_SelectionChanged(object sender, RoutedEventArgs args)
        {
            CityTown city = (sender as ListBox).SelectedItem as CityTown;
            if (city == null)
                return;

            PhoneApplicationService.Current.State["selectedCity"] = city;

            if (CityList != null)
                CityList.SelectedIndex = -1;

            this.NavigationService.Navigate(new Uri("/View/SelectTownPage.xaml", UriKind.Relative));
        }
    }
}