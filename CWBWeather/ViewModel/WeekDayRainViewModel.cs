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
using Microsoft.Phone.Net.NetworkInformation;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace CWBWeather.ViewModel
{
    public class WeekDayRainViewModel : ViewModelBase
    {
        public WeekDayRainViewModel()
        {
        }

        private IList<KeyValuePair<String, String>> _weekDayRainItemsSource;

        public IList<KeyValuePair<String, String>> WeekDayRainItemsSource
        //public IList<CityTown> WeekdayForecastItemsSource
        {
            get
            {
                return _weekDayRainItemsSource;
            }
            set
            {
                if (_weekDayRainItemsSource != value)
                {
                    _weekDayRainItemsSource = value;
                }

                OnPropertyChanged("WeekDayRainItemsSource");
            }
        }
    }
}
