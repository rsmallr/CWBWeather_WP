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
using System.ComponentModel;
using System.Globalization;

namespace CWBWeather.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                ForecastCity = new CityTown
                {
                    Name = "天龍國"
                };

                ForecastTown = new CityTown
                {
                    Name = "天龍區"
                };

                DateText = "    2013/02/23 星期六";
            }

            BackgroundImage = "Weather01.bmp";
        }

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

        private CityTown _forecastTown;

        public CityTown ForecastTown
        {
            get
            {
                return _forecastTown;
            }
            set
            {
                if (_forecastTown != value)
                {
                    _forecastTown = value;
                }

                OnPropertyChanged("ForecastTown");
            }
        }

        private string _dateText;

        public string DateText
        {
            get
            {
                return _dateText;
            }
            set
            {
                if (_dateText != value)
                {
                    _dateText = value;
                }

                OnPropertyChanged("DateText");
            }
        }

        private string _backgroundImage;

        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                }

                OnPropertyChanged("BackgroundImage");
            }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                }

                OnPropertyChanged("IsLoading");
            }
        }   
    }

    public class ForegroundImageConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string weatherIcon = (string)value;
            int prefixIndex = weatherIcon.IndexOf("Weather") + 7;
            weatherIcon = weatherIcon.Substring(prefixIndex, weatherIcon.Length - prefixIndex);
            int postfixIndex = weatherIcon.IndexOf(".");
            weatherIcon = weatherIcon.Substring(0, postfixIndex);

            return "/WeatherForecast;component/Image/Foreground/" + Utils.GetWeatherIcon(weatherIcon) + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BackgroundImageConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string weatherIcon = (string)value;
            int prefixIndex = weatherIcon.IndexOf("Weather") + 7;
            weatherIcon = weatherIcon.Substring(prefixIndex, weatherIcon.Length - prefixIndex);
            int postfixIndex = weatherIcon.IndexOf(".");
            weatherIcon = weatherIcon.Substring(0, postfixIndex);

            return "/WeatherForecast;component/Image/Background/" + Utils.GetWeatherIcon(weatherIcon) + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
