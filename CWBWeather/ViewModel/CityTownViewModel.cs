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
    public class CityTownViewModel : ViewModelBase
    {
        public CityTownViewModel()
        {
            XmlCityTownRepository countryRepository = new XmlCityTownRepository();

            ListItemsSource = countryRepository.GetCountryList();
        }

        public CityTownViewModel(CityTown selectedCity)
        {
            XmlCityTownRepository countryRepository = new XmlCityTownRepository();

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

        private int _listSelectedIndex = -1;

        public int ListSelectedIndex
        {
            get
            {
                return _listSelectedIndex;
            }
            set
            {
                if (_listSelectedIndex != value)
                {
                    _listSelectedIndex = value;
                }

                OnPropertyChanged("ListSelectedIndex");
            }
        }
    }
}
