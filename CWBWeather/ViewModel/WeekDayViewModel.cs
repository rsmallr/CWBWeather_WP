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
using System.Globalization;
using System.Text.RegularExpressions;

namespace CWBWeather.ViewModel
{
    public class WeekDayViewModel : ViewModelBase
    {
        private string XML_FOLDER = "http://www.cwb.gov.tw/township/XML/";
        private string XML_WEEKDAY = "TAIWAN_Weekday_CH.xml";

        private CityTown mForecastCity;

        public WeekDayViewModel()
        {
            mForecastCity = Navigation.ForecastCity;

            //GetWeekdayForecastItemsSource();
        }

        public void GetWeekdayForecastItemsSource()
        {
            GetCWBWeekdayForecast();

            //IList<ForecastData> weekdayForecastList = new List<ForecastData>();

            //weekdayForecastList.Add(new ForecastData() {
            //    ID = "10017",
            //    Name = "基隆市"
            //});

            //weekdayForecastList.Add(new ForecastData()
            //{
            //    ID = "63000",
            //    Name = "臺北市"
            //});

            //// Items to collect
            //a.Add(new Accomplishment() { Name = "Potions", Type = "Item" });
            //a.Add(new Accomplishment() { Name = "Coins", Type = "Item" });
            //a.Add(new Accomplishment() { Name = "Hearts", Type = "Item" });
            //a.Add(new Accomplishment() { Name = "Swords", Type = "Item" });
            //a.Add(new Accomplishment() { Name = "Shields", Type = "Item" });

            //// Levels to complete
            //a.Add(new Accomplishment() { Name = "Level 1", Type = "Level" });
            //a.Add(new Accomplishment() { Name = "Level 2", Type = "Level" });
            //a.Add(new Accomplishment() { Name = "Level 3", Type = "Level" });

            //Accomplishments = a;

            //WeekdayForecastItemsSource = weekdayForecastList;
        }

        public void GetCWBWeekdayForecast()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    WebClient downloader = new WebClient();
                    Uri rssUri = new Uri(XML_FOLDER + XML_WEEKDAY, UriKind.Absolute);
                    downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloaded);
                    downloader.DownloadStringAsync(rssUri);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                    GetWeekdayForecastFromXML();
                }
            }
            else
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                GetWeekdayForecastFromXML();
            }
        }

        void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
            }
            else
            {
                Utils.saveXML(XML_WEEKDAY, e.Result);
            }

            try
            {
                GetWeekdayForecastFromXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀檔錯誤。");
            }
        }

        public void GetWeekdayForecastFromXML()
        {
            StreamReader streamReader = Utils.readXML(XML_WEEKDAY);

            XDocument loadedData = XDocument.Load(streamReader);

            var timeData = from query in loadedData.Descendants("FcstTime")
                           select (string)query;

            IList<string> timeDataList = timeData.ToList();

            var forecastData = from c1 in loadedData.Descendants("Area")
                       where c1.Attribute("AreaID").Value == mForecastCity.ID
                       from c2 in c1.Elements()
                       select new ForecastData
                       {
                           Temperature = (string)c2.Element("Temperature"),
                           Wx = (string)c2.Element("Wx"),
                           WeatherDes = (string)c2.Element("WeatherDes"),
                           WeatherIcon = (string)c2.Element("WeatherIcon")
                       };
            IList<ForecastData> forecastDataList = forecastData.ToList();
            for (int i = 0; i < forecastDataList.Count; i++)
            {
                forecastDataList[i].Time = timeDataList[i];
            }

            WeekdayForecastItemsSource = forecastDataList;

            streamReader.Close();
            streamReader.Dispose();
        }

        private IList<ForecastData> _weekdayForecastItemsSource;
        //private IList<CityTown> _weekdayForecastItemsSource;

        public IList<ForecastData> WeekdayForecastItemsSource
        //public IList<CityTown> WeekdayForecastItemsSource
        {
            get
            {
                return _weekdayForecastItemsSource;
            }
            set
            {
                if (_weekdayForecastItemsSource != value)
                {
                    _weekdayForecastItemsSource = value;
                }

                OnPropertyChanged("WeekdayForecastItemsSource");
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

    public class WeekdayTimeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = (string)value;
            DateTime dt = System.Convert.ToDateTime((string)value);
            return dt.ToString("dddd", new CultureInfo("zh-TW"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MaxTConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string maxT = (string)value;
            return "~" + maxT + "°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TemperatureConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            string temperature = (string)value;
            return temperature + "°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RainConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rain = (string)value;
            if (rain.Length > 0)
                return rain + "%";
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IconConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string weatherIcon = "";
            string pattern = @"\D*(?<number>\d+)\D*";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches((string)value);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                weatherIcon = groups["number"].Value;
                break;
            }

            string iconName = "";
            switch (weatherIcon)
            {
                case "01":
                case "02":
                case "03":
                case "05":
                case "06":
                case "44":
                    iconName = weatherIcon;
                    break;
                case "07":
                case "08":
                    iconName = "08";
                    break;
                case "04":
                case "12":
                case "26":
                    iconName = "04";
                    break;
                case "31":
                case "36":
                    iconName = "31";
                    break;
                default:
                    iconName = "02";
                    break;
            }

            return "/WeatherForecast;component/Image/Icon/" + iconName + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
