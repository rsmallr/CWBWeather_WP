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
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using CWBWeather.Model;
using System.IO.IsolatedStorage;

namespace CWBWeather.View
{
    public partial class SelectTownPage : PhoneApplicationPage
    {
        public SelectTownPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("selectedCity"))
            {
                CityTown selectedCity = (CityTown)PhoneApplicationService.Current.State["selectedCity"];

                this.DataContext = new SelectTownViewModel(selectedCity);
            }
        }

        void TownListBox_SelectionChanged(object sender, RoutedEventArgs args)
        {
            CityTown town = (sender as ListBox).SelectedItem as CityTown;

            CityTown forecastCity = (CityTown)PhoneApplicationService.Current.State["selectedCity"];

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["forecastTown"] = town;
            settings["forecastCity"] = forecastCity;
            settings.Save();

            this.NavigationService.RemoveBackEntry();
            this.NavigationService.GoBack();
        }
    }
}