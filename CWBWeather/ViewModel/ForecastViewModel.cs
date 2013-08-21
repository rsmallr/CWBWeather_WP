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
using System.ComponentModel;

namespace CWBWeather.ViewModel
{
    public class ForecastViewModel : ViewModelBase
    {
        public ForecastViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                CurrentForecastData = new ForecastData
                {
                    Temperature = "20",
                    Rain = "0",
                    CI = "舒適",
                    WeatherIcon = "Weather02.bmp",
                    Wx = "短暫雨"
                };

                WeatherIcon = "Weather02.bmp";
                MaxT = "28";
                MinT = "17";
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

        private ForecastData _currentForecastData;
        private ForecastData _currentForecastDataEx;

        public ForecastData CurrentForecastData
        {
            get
            {
                return _currentForecastData;
            }
            set
            {
                if (_currentForecastData != value)
                {
                    _currentForecastData = value;
                    _currentForecastDataEx = value;
                    _currentForecastDataEx.Temperature = value.Temperature;
                    _currentForecastDataEx.CI = value.CI + "的一天";
                    _currentForecastDataEx.Rain = "降雨機率" + value.Rain + "%";
                }

                OnPropertyChanged("CurrentForecastData");
            }
        }


        private string _weatherIcon;

        public string WeatherIcon
        {
            get
            {
                return _weatherIcon;
            }
            set
            {
                if (_weatherIcon != value)
                {
                    _weatherIcon = value;
                }

                OnPropertyChanged("WeatherIcon");
            }
        }


        private string _maxT;
        private string _maxTEx;
        public string MaxT
        {
            get
            {
                return _maxTEx;
            }
            set
            {
                if (_maxT != value)
                {
                    _maxT = value;
                    _maxTEx = "高溫" + value;
                }

                OnPropertyChanged("MaxT");
            }
        }

        private string _minT;
        private string _minTEx;
        public string MinT
        {
            get
            {
                return _minTEx;
            }
            set
            {
                if (_minT != value)
                {
                    _minT = value;
                    _minTEx = "低溫" + value;
                }

                OnPropertyChanged("MinT");
            }
        }
    }
}
