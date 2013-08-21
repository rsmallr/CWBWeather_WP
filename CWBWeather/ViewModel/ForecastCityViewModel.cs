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
    public class ForecastCityViewModel : ViewModelBase
    {
        private CityTown _forecastCity;

        public CityTown ForecastCity
        {
            get
            {
                return _forecastCity;
            }
            set
            {
                if (_forecastCity != value)
                {
                    _forecastCity = value;
                }

                OnPropertyChanged("ForecastCity");
            }
        }
    }
}
