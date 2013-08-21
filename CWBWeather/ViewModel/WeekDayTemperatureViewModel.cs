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
    public class WeekDayTemperatureViewModel : ViewModelBase
    {
        public WeekDayTemperatureViewModel()
        {
        }

        //private IList<KeyValuePair<String, int>> _weekDayTemperatureItemsSource;

        //public IList<KeyValuePair<String, int>> WeekDayTemperatureItemsSource
        ////public IList<CityTown> WeekdayForecastItemsSource
        //{
        //    get
        //    {
        //        return _weekDayTemperatureItemsSource;
        //    }
        //    set
        //    {
        //        if (_weekDayTemperatureItemsSource != value)
        //        {
        //            _weekDayTemperatureItemsSource = value;
        //        }

        //        OnPropertyChanged("WeekDayTemperatureItemsSource");
        //    }
        //}

        private IList<ForecastData> _weekDayTemperatureItemsSource;

        public IList<ForecastData> WeekDayTemperatureItemsSource
        //public IList<CityTown> WeekdayForecastItemsSource
        {
            get
            {
                return _weekDayTemperatureItemsSource;
            }
            set
            {
                if (_weekDayTemperatureItemsSource != value)
                {
                    _weekDayTemperatureItemsSource = value;
                }

                OnPropertyChanged("WeekDayTemperatureItemsSource");
            }
        }
    }
}
