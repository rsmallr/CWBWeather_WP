using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Mvvm.ViewModels;
using CWBWeather.Model;
using System.Collections.Generic;

namespace CWBWeather.ViewModel
{
    public class SelectTownViewModel : ViewModelBase
    {
        public SelectTownViewModel(CityTown selectedCity)
        {
            XmlCityTownRepository countryRepository = new XmlCityTownRepository();

            SelectedCity = selectedCity;

            ListItemsSource = countryRepository.GetTownList(selectedCity);
        }

        private IList<CityTown> _listItemsSource;

        public IList<CityTown> ListItemsSource
        {
            get
            {
                return _listItemsSource;
            }
            set
            {
                if (_listItemsSource != value)
                {
                    _listItemsSource = value;
                }

                OnPropertyChanged("ListItemsSource");
            }
        }

        private CityTown _selectedCity;

        public CityTown SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
                }

                OnPropertyChanged("SelectedCity");
            }
        }
    }
}
