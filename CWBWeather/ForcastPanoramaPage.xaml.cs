#define DEBUG_AGENT

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
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using Visifire.Charts;
using Visifire.Commons;
using Microsoft.Phone.Net.NetworkInformation;
using CWBWeather.ViewModel;
using System.Xml.Linq;
using CWBWeather.Model;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Scheduler;
using WeatherForecast.Util;
using WeatherForecast;
using System.Device.Location;

namespace CWBWeather
{
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        PeriodicTask mPeriodicTask;
        int mCityIndex = -1;

        int mRowHeight;
        int mMaxValue;
        int mMinValue;
        int mStepSize;

        int mPoPMaxValue = 100;
        int mPoPMinValue = 0;
        int mPoPStepSize = 10;

        //string mPattern = "s";
        //CountyWeekdaysForecast mData = null;
        //Area mAreaForecastData = null;
        //List<Value> mForcastValues = null;

        string XML_FOLDER = "http://www.cwb.gov.tw/township/XML/";
        //string XML_WEEKDAY = "TAIWAN_Weekday_CH.xml";
        string XML_WEEKDAY_POSTFIX = "_Weekday_CH.xml";
        string XML_72HR_POSTFIX = "_72hr_CH.xml";
        string TILE_NAME = "CWBWeather";

        private ForecastCityViewModel mForecastCityViewModel;
        private WeekDayViewModel mWeekDayViewModel;
        private WeekDayTemperatureViewModel mWeekDayTemperatureViewModel;
        private WeekDayRainViewModel mWeekDayRainViewModel;
        private ForecastViewModel mForecastViewModel;
        private MainPageViewModel mMainPageViewModel;
        private String mBackgroundImage = "Image/01.jpg";

        private IList<ForecastData> mWeekdayForecastDataList;
        private ForecastData m72hrForecastData;

        private GeoCoordinateWatcher mWatcher;
        private bool mIsGeoCoordinateReady = false;

        public PanoramaPage1()
        {
            InitializeComponent();

            StartPeriodicAgent();

            Loaded += OnLoaded;

            mForecastCityViewModel = new ForecastCityViewModel();

            mWeekDayViewModel = new WeekDayViewModel();
            WeekDayViewOnPage.DataContext = mWeekDayViewModel;

            mForecastViewModel = new ForecastViewModel();
            ForecastViewOnPage.DataContext = mForecastViewModel;

            mMainPageViewModel = new MainPageViewModel();
            FirstPivot.DataContext = mMainPageViewModel;
            DateTime now = DateTime.Now;
            mMainPageViewModel.DateText = now.ToString("        M/dd dddd", new CultureInfo("zh-TW"));
            mMainPageViewModel.BackgroundImage = mBackgroundImage;
            MainPageLayoutRoot.DataContext = mMainPageViewModel;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("lastLaunchTime")) {
                TimeSpan difference = DateTime.Now - (DateTime)settings["lastLaunchTime"];
                if (difference.Days >= 13)
                {
                    MessageBox.Show("因為系統會自動關閉不常用的程式背景作業，請至少每兩週開啟一次晴時多雲偶陣雨，讓動態磚能自動更新！");
                }
            }
            settings["lastLaunchTime"] = DateTime.Now;
            settings.Save();

            //WeekDayViewOnPage.DataContext = forecastCityViewModel;
            //WeekDayViewOnPage.DataContext = new WeekDayViewModel();
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            ThemeManager.ToDarkTheme();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains("forecastCity") || !settings.Contains("forecastTown"))
            {
                this.NavigationService.Navigate(new Uri("/View/SelectCityPage.xaml", UriKind.Relative));
                return;
            }
            else
            {
                CityTown forecastCity = (CityTown)settings["forecastCity"];
                CityTown forecastTown = (CityTown)settings["forecastTown"];

                mForecastCityViewModel.ForecastCity = forecastTown;
                mMainPageViewModel.ForecastTown = forecastTown;
                mMainPageViewModel.ForecastCity = forecastCity;

                Navigation.ForecastCity = forecastTown;
                SetForecastViewModel();
            }

            GetCWBWeekdayForecast();
            Get72HrForecast();

            //TemptureItem.Width = TemptureItem.ActualWidth * 2;
            //PoPItem.Width = PoPItem.ActualWidth * 2;

            //if (NetworkInterface.GetIsNetworkAvailable())
            //{
            //    try
            //    {
            //        WebClient downloader = new WebClient();
            //        Uri rssUri = new Uri(XML_FOLDER + XML_WEEKDAY, UriKind.Absolute);
            //        downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloaded);
            //        downloader.DownloadStringAsync(rssUri);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
            //        loadWeatherData();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
            //    loadWeatherData();
            //}
        }

        //void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    if (e.Result == null || e.Error != null)
        //    {
        //        MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
        //    }
        //    else
        //    {
        //        saveXML(XML_WEEKDAY, e.Result);
        //    }

        //    try
        //    {
        //        loadWeatherData();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("讀檔錯誤。");
        //    }
        //}

        void loadWeatherData()
        {
            //StreamReader streamReader = readXML(XML_WEEKDAY);
            //XmlReader XMLReader = XmlReader.Create(streamReader);
            //mData = (CountyWeekdaysForecast)Utils.deserialize(XMLReader, typeof(CountyWeekdaysForecast));

            //streamReader.Close();
            //streamReader.Dispose();
            //XMLReader.Close();
            //XMLReader.Dispose();

            //City forecastCity = (Application.Current as App).forecastCity;
            //mCityIndex = -1;
            //int idx = 0;
            //if (forecastCity != null)
            //{
            //    foreach (var city in mData.forecastData.areas)
            //    {
            //        if (city.areaID == forecastCity.COUN_ID)
            //        {
            //            mCityIndex = idx;
            //            break;
            //        }
            //        idx++;
            //    }
            //}

            //mAreaForecastData = mData.forecastData.areas[mCityIndex];
            //mForcastValues = mAreaForecastData.values;
            //WeekdatForcastList.ItemsSource = mForcastValues;
            //PanoramaTitle.Title = mData.forecastData.areas[mCityIndex].chName + PanoramaTitle.Title;

            //drawTempture();
            //drawPoP();
        }

        public void GetCWBWeekdayForecast()
        {
            mMainPageViewModel.IsLoading = true;

            String uri = XML_FOLDER + Utils.GetXmlFileName(Navigation.ForecastCity, XML_WEEKDAY_POSTFIX);

            ForecastDownloader forecastDownloader = new ForecastDownloader();
            forecastDownloader.DownloadStringCompleted += ((sender, e) =>
            {
                if (e.Error != null)
                {
                    if (e.Error.Message == WeatherForecast.Util.DownloadStringCompletedEventArgs.NETWORK_UNAVAILABLE)
                    {
                        MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                        LoadForecastData();
                    }
                    else
                    {
                        MessageBox.Show("無法讀取最新的天氣資訊。");
                        LoadForecastData();
                    }
                }
                else if (e.Result == null )
                {
                    LoadForecastData();
                }
                else
                {
                    Utils.saveXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_WEEKDAY_POSTFIX), e.Result);
                    LoadForecastData();
                }
            });
            forecastDownloader.DownloadStringAsync(new Uri(uri, UriKind.Absolute));

            /*if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    WebClient downloader = new WebClient();
                    //Uri rssUri = new Uri(XML_FOLDER + XML_WEEKDAY, UriKind.Absolute);
                    String uri = "";
                    if (Navigation.ForecastCity.ID.StartsWith("630"))
                        uri = XML_FOLDER + GetXmlFileName(XML_WEEKDAY_POSTFIX);
                    else
                        uri = XML_FOLDER + GetXmlFileName(XML_WEEKDAY_POSTFIX);
                    Uri rssUri = new Uri(uri, UriKind.Absolute);
                    downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloaded);
                    downloader.DownloadStringAsync(rssUri);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                    LoadForecastData();
                }
            }
            else
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                LoadForecastData();
            }*/
        }

/*        public String GetXmlFileName(String postfix)
        {
            String fileName = "";

            if (Navigation.ForecastCity.ID.StartsWith("630"))
                fileName = Navigation.ForecastCity.ID.Substring(0, Navigation.ForecastCity.ID.Length - 4) + "00" + postfix;
            else if (Navigation.ForecastCity.ID.StartsWith("640"))
                fileName = Navigation.ForecastCity.ID.Substring(0, Navigation.ForecastCity.ID.Length - 3) + "0" + postfix;
            else
                fileName = Navigation.ForecastCity.ID.Substring(0, Navigation.ForecastCity.ID.Length - 2) + postfix;

            return fileName;
        }*/

        void OnDownloaded(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
            }
            else
            {
                Utils.saveXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_WEEKDAY_POSTFIX), e.Result);
            }

            try
            {
                LoadForecastData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀檔錯誤。");
            }
        }

        public void LoadForecastData()
        {
            GetWeekdayForecastFromXML();
            SetViewModels();

            mMainPageViewModel.IsLoading = false;
        }

        public void GetWeekdayForecastFromXML()
        {
            StreamReader streamReader = Utils.readXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_WEEKDAY_POSTFIX));

            if (streamReader == null)
            {
                return;
            }

            XDocument loadedData = XDocument.Load(streamReader);

            var timeData = from query in loadedData.Descendants("FcstTime")
                           select (string)query;

            IList<string> timeDataList = timeData.ToList();

            var forecastData = from c1 in loadedData.Descendants("Area")
                               where c1.Attribute("AreaID").Value == Navigation.ForecastCity.ID
                               from c2 in c1.Elements()
                               select new ForecastData
                               {
                                   Temperature = (string)c2.Element("Temperature"),
                                   Wx = (string)c2.Element("Wx"),
                                   WeatherDes = (string)c2.Element("WeatherDes"),
                                   WeatherIcon = (string)c2.Element("WeatherIcon"),
                                   RH = (string)c2.Element("RH"),
                                   Rain = ((string)c2.Element("PoP")).Length>0 ? ((string)c2.Element("PoP")) : "",
                                   WindSpeed = (string)c2.Element("WindSpeed"),
                                   WindDir = (string)c2.Element("WindDir"),
                                   MinCI = (string)c2.Element("MinCI"),
                                   MaxCI = (string)c2.Element("MaxCI"),
                                   MinT = (string)c2.Element("MinT"),
                                   MaxT = (string)c2.Element("MaxT"),
                                   WindLevel = (string)c2.Element("WindLevel")
                               };
            IList<ForecastData> forecastDataList = forecastData.ToList();
            for (int i = 0; i < forecastDataList.Count; i++)
            {
                forecastDataList[i].Time = timeDataList[i];
            }

            DateTime prevDt = Convert.ToDateTime(forecastDataList[0].Time);
            int c = 1;
            while (c < forecastDataList.Count)
            {
                DateTime dt = Convert.ToDateTime(forecastDataList[c].Time);
                System.Diagnostics.Debug.WriteLine("dt: " + dt.Day);
                System.Diagnostics.Debug.WriteLine("prevDt: " + prevDt.Day);
                if (dt.Day == prevDt.Day)
                    forecastDataList.RemoveAt(c);
                else
                    c++;
                prevDt = dt;
            }

            //mWeekDayViewModel.WeekdayForecastItemsSource = forecastDataList;
            mWeekdayForecastDataList = forecastDataList;
            if (forecastDataList.ElementAt(0) != null)
            {
                mForecastViewModel.WeatherIcon = forecastDataList[0].WeatherIcon;
                mForecastViewModel.MaxT = forecastDataList[0].MaxT;
                mForecastViewModel.MinT = forecastDataList[0].MinT;
                SetBackgroundImage(forecastDataList[0].WeatherIcon);
            }

            streamReader.Close();
            streamReader.Dispose();
        }

        public void SetViewModels()
        {
            SetWeekdayForecastViewModel();
        }

        public void SetWeekdayForecastViewModel()
        {
            mWeekDayViewModel.WeekdayForecastItemsSource = mWeekdayForecastDataList;
            mWeekDayViewModel.SelectedCity = Navigation.ForecastCity;
        }

        public void SetForecastViewModel()
        {
            mForecastViewModel.SelectedCity = Navigation.ForecastCity;
        }

        /*-------------------------------------------------------------------------------*/
        public void Get72HrForecast()
        {
            String uri = XML_FOLDER + Utils.GetXmlFileName(Navigation.ForecastCity, XML_72HR_POSTFIX);

            ForecastDownloader forecastDownloader = new ForecastDownloader();
            forecastDownloader.DownloadStringCompleted += ((sender, e) =>
            {
                if (e.Error != null)
                {
                    if (e.Error.Message == WeatherForecast.Util.DownloadStringCompletedEventArgs.NETWORK_UNAVAILABLE)
                    {
                        Load72HrForecastData();
                    }
                }
                else if (e.Result == null)
                {
                    Load72HrForecastData();
                }
                else
                {
                    Utils.saveXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_72HR_POSTFIX), e.Result);
                    Load72HrForecastData();

                    TileUpdateWorker tileUpdateWorker = new TileUpdateWorker();
                    tileUpdateWorker.UpdateFromXml();
                }
            });
            forecastDownloader.DownloadStringAsync(new Uri(uri, UriKind.Absolute));

/*            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    WebClient downloader = new WebClient();
                    //Uri rssUri = new Uri(XML_FOLDER + XML_WEEKDAY, UriKind.Absolute);
                    String uri = "";
                    if (Navigation.ForecastCity.ID.StartsWith("630"))
                        uri = XML_FOLDER + GetXmlFileName(XML_72HR_POSTFIX);
                    else
                        uri = XML_FOLDER + GetXmlFileName(XML_72HR_POSTFIX);
                    Uri rssUri = new Uri(uri, UriKind.Absolute);
                    downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(On72HrForecastDownloaded);
                    downloader.DownloadStringAsync(rssUri);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                    Load72HrForecastData();
                }
            }
            else
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
                Load72HrForecastData();
            }*/
        }

        void On72HrForecastDownloaded(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("無法下載最新的天氣資訊！請檢查網路連線。");
            }
            else
            {
                Utils.saveXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_72HR_POSTFIX), e.Result);
            }

            try
            {
                Load72HrForecastData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀檔錯誤。");
            }
        }

        public void Load72HrForecastData()
        {
            Get72HrForecastFromXML();
            Set72HrViewModels();
        }

        public void Get72HrForecastFromXML()
        {
            StreamReader streamReader = Utils.readXML(Utils.GetXmlFileName(Navigation.ForecastCity, XML_72HR_POSTFIX));

            if (streamReader == null)
            {
                return;
            }

            XDocument loadedData = XDocument.Load(streamReader);
            streamReader.Close();
            streamReader.Dispose();

            var timeData12hr = from c1 in loadedData.Descendants("Time")
                               where c1.Attribute("slice").Value == "12"
                               from c2 in c1.Descendants("FcstTime")
                               select (string)c2;

            var timeData3hr = from c1 in loadedData.Descendants("Time")
                              where c1.Attribute("slice").Value == "3"
                              from c2 in c1.Descendants("FcstTime")
                              select (string)c2;

            var forecastData12hr = from c1 in loadedData.Descendants("Area")
                                   where c1.Attribute("AreaID").Value == Navigation.ForecastCity.ID
                                   from c2 in c1.Descendants("Value")
                                   where c2.Attribute("layout").Value == "12"
                                   select new ForecastData
                                   {
                                       Temperature = (string)c2.Element("Temperature"),
                                       Wx = (string)c2.Element("Wx"),
                                       WeatherDes = (string)c2.Element("WeatherDes"),
                                       WeatherIcon = (string)c2.Element("WeatherIcon"),
                                       RH = (string)c2.Element("RH"),
                                       Rain = (string)c2.Element("PoP"),
                                       WindSpeed = (string)c2.Element("WindSpeed"),
                                       WindDir = (string)c2.Element("WindDir"),
                                       MinCI = (string)c2.Element("MinCI"),
                                       MaxCI = (string)c2.Element("MaxCI"),
                                       MinT = (string)c2.Element("MinT"),
                                       MaxT = (string)c2.Element("MaxT"),
                                       WindLevel = (string)c2.Element("WindLevel")
                                   };
            IList<ForecastData> forecast12hrDataList = forecastData12hr.ToList();

            for (int i = 0; i < forecast12hrDataList.Count; i++)
            {
                forecast12hrDataList[i].Time = timeData12hr.ElementAt(i);
            }

            var forecastData3hr = from c1 in loadedData.Descendants("Area")
                                  where c1.Attribute("AreaID").Value == Navigation.ForecastCity.ID
                                  from c2 in c1.Descendants("Value")
                                  where c2.Attribute("layout").Value == "3"
                                  select new ForecastData
                                  {
                                      Temperature = (string)c2.Element("Temperature"),
                                      Wx = (string)c2.Element("Wx"),
                                      WeatherDes = (string)c2.Element("WeatherDes"),
                                      WeatherIcon = (string)c2.Element("WeatherIcon"),
                                      RH = (string)c2.Element("RH"),
                                      Rain = (string)c2.Element("PoP"),
                                      WindSpeed = (string)c2.Element("WindSpeed"),
                                      WindDir = (string)c2.Element("WindDir"),
                                      CI = (string)c2.Element("CI"),
                                      MinCI = (string)c2.Element("MinCI"),
                                      MaxCI = (string)c2.Element("MaxCI"),
                                      MinT = (string)c2.Element("MinT"),
                                      MaxT = (string)c2.Element("MaxT"),
                                      WindLevel = (string)c2.Element("WindLevel")
                                  };
            IList<ForecastData> forecast3hrDataList = forecastData3hr.ToList();

            for (int i = 0; i < forecast3hrDataList.Count; i++)
            {
                forecast3hrDataList[i].Time = timeData3hr.ElementAt(i);
            }

            m72hrForecastData = null;
            ForecastData currentData3hr = null;
            ForecastData currentData12hr = null;

            foreach (ForecastData data in forecast3hrDataList)
            {
                if (Convert.ToDateTime(data.Time).CompareTo(DateTime.Now) > 0)
                {
                    currentData3hr = data;
                    break;
                }
            }

            foreach (ForecastData data in forecast12hrDataList)
            {
                if (Convert.ToDateTime(data.Time).CompareTo(DateTime.Now) > 0)
                {
                    currentData12hr = data;
                    break;
                }
            }

            if (currentData3hr != null && currentData12hr != null)
            {
                m72hrForecastData = new ForecastData
                {
                    Temperature = currentData3hr.Temperature,
                    Wx = currentData3hr.Wx,
                    WeatherDes = currentData3hr.WeatherDes,
                    WeatherIcon = currentData3hr.WeatherIcon,
                    RH = currentData3hr.RH,
                    Rain = currentData12hr.Rain,
                    WindSpeed = currentData3hr.WindSpeed,
                    WindDir = currentData3hr.WindDir,
                    CI = currentData3hr.CI,
                    MinCI = currentData12hr.MinCI,
                    MaxCI = currentData12hr.MaxCI,
                    MinT = currentData12hr.MinT,
                    MaxT = currentData12hr.MaxT,
                    Time = currentData12hr.Time
                };
            }
        }

        public void Set72HrViewModels()
        {
            if (m72hrForecastData != null)
                mForecastViewModel.CurrentForecastData = m72hrForecastData;
        }

        public void SetBackgroundImage(string weatherIcon)
        {
            mMainPageViewModel.BackgroundImage = weatherIcon;
            //int prefixIndex = weatherIcon.IndexOf("Weather") + 7;
            //weatherIcon = weatherIcon.Substring(prefixIndex, weatherIcon.Length - prefixIndex);
            //int postfixIndex = weatherIcon.IndexOf(".");
            //weatherIcon = weatherIcon.Substring(0, postfixIndex);

            //string backgroundImage = "";
            //switch (weatherIcon)
            //{
            //    case "01":
            //        backgroundImage = "01";
            //        break;
            //    default:
            //        backgroundImage = "26";
            //        break;
            //}

            //mBackgroundImage = "Image/" + backgroundImage + ".jpg";
            //MainPageLayoutBackground.ImageSource = new BitmapImage(new Uri(mBackgroundImage, UriKind.Relative));
        }
        /*-------------------------------------------------------------------------------*/

        private void saveXML(string fileName, string content)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, myIsolatedStorage))
                {
                    using (StreamWriter writer = new StreamWriter(isoStream))
                    {
                        writer.Write(content, System.Text.Encoding.UTF8);
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                    isoStream.Close();
                    isoStream.Dispose();
                }
            }
        }

        private StreamReader readXML(string fileName)
        {
            StreamReader reader;
            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream isoFileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open);
                    reader = new StreamReader(isoFileStream);
                    return reader;
                }
            }
            catch
            { }

            return null;
        }

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    drawTempture();
        //}

        //private void drawTempture()
        //{
        //    if (mData == null)
        //        return;

        //    PointCollection points = new PointCollection();

        //    Area area = mData.forecastData.areas[mCityIndex];
        //    int columnWidth = (int)(TemptureChart.ActualWidth / (area.values.Count - 1));
        //    int idx = 0;
        //    List<int> dataset = new List<int>();
        //    List<string> xDataSet = new List<string>();

        //    foreach (Value value in area.values)
        //    {
        //        int dataValue = Convert.ToInt32(value.temperature);
        //        dataset.Add(dataValue);
        //        points.Add(new Point(idx * columnWidth, TemptureChart.ActualHeight / 2 - dataValue * 10));

        //        string slice = value.layout;
        //        foreach (Time time in mData.forecastData.metaData.times)
        //        {
        //            //if (time.slice == slice)
        //            {
        //                string x = time.fcstTimes[idx].value;
        //                x = x.Replace("T", " ");
        //                x = x.Replace("+08:00", "");
        //                xDataSet.Add(x);
        //                //break;
        //            }
        //        }

        //        idx++;
        //        System.Diagnostics.Debug.WriteLine("maxT: " + value.maxT);
        //    }

        //    DataSeries ds = TemptureChart.Series[0];

        //    for (int i = 0; i < dataset.Count; i++)
        //    {
        //        DataPoint dp = new DataPoint();
        //        dp.XValue = xDataSet[i];
        //        System.Diagnostics.Debug.WriteLine("dp.XValue: " + dp.XValue);
        //        dp.YValue = dataset[i];
        //        System.Diagnostics.Debug.WriteLine("dp.YValue: " + dp.YValue);
        //        ds.DataPoints.Add(dp);
        //    }
        //}
/*        private void drawTempture()
        {
            if (mData == null)
                return;

            PointCollection points = new PointCollection();

            Area area = mData.forecastData.areas[mCityIndex];
            int columnWidth = (int)(TemptureChart.ActualWidth / (area.values.Count - 1));
            int idx = 0;
            List<int> dataset = new List<int>();
            List<string> xDataSet = new List<string>();

            foreach (Value value in area.values)
            {
                int dataValue = Convert.ToInt32(value.temperature);
                dataset.Add(dataValue);
                points.Add(new Point(idx * columnWidth, TemptureChart.ActualHeight / 2 - dataValue * 10));

                string slice = value.layout;
                foreach (Time time in mData.forecastData.metaData.times)
                {
                    if (time.slice == slice)
                    {
                        xDataSet.Add(parsedDate(time.fcstTimes[idx].value));
                        break;
                    }
                }

                idx++;
                System.Diagnostics.Debug.WriteLine("maxT: " + value.maxT);
            }

            mMaxValue = 30;
            mMinValue = 10;
            mStepSize = 5;
            clear(TemptureChartLine);
            drawBackground(dataset.Count, mMaxValue, mMinValue, mStepSize, xDataSet, TemptureChart, TemptureChartXLabel, TemptureChartYLabel);
            drawLineChart(dataset, mMaxValue, mMinValue, mStepSize, TemptureChart, TemptureChartLine);
        }*/

        //private void button2_Click(object sender, RoutedEventArgs e)
        //{
        //    drawPoP();
        //}

        //private void drawPoP()
        //{
        //    if (mData == null)
        //        return;

        //    PointCollection points = new PointCollection();

        //    Area area = mData.forecastData.areas[mCityIndex];
        //    int columnWidth = (int)(PoPChart.ActualWidth / (area.values.Count - 1));
        //    int idx = 0;
        //    List<int> dataset = new List<int>();
        //    List<string> xDataSet = new List<string>();

        //    foreach (Value value in area.values)
        //    {
        //        int dataValue = -1;
        //        if (value.poP.Length > 0)
        //            dataValue = Convert.ToInt32(value.poP);

        //        dataset.Add(dataValue);
        //        points.Add(new Point(idx * columnWidth, PoPChart.ActualHeight / 2 - dataValue * 10));

        //        string slice = value.layout;
        //        foreach (Time time in mData.forecastData.metaData.times)
        //        {
        //            //if (time.slice == slice)
        //            {
        //                string x = time.fcstTimes[idx].value;
        //                x = x.Replace("T", " ");
        //                x = x.Replace("+08:00", "");
        //                xDataSet.Add(x);
        //                //break;
        //            }
        //        }

        //        idx++;
        //    }

        //    DataSeries ds = PoPChart.Series[0];

        //    for (int i = 0; i < dataset.Count; i++)
        //    {
        //        if (dataset[i] != -1)
        //        {
        //            DataPoint dp = new DataPoint();
        //            dp.XValue = xDataSet[i];
        //            System.Diagnostics.Debug.WriteLine("dp.XValue: " + dp.XValue);
        //            dp.YValue = dataset[i];
        //            System.Diagnostics.Debug.WriteLine("dp.YValue: " + dp.YValue);
        //            ds.DataPoints.Add(dp);
        //        }
        //    }
        //}

        /*private void drawPoP()
        {
            if (mData == null)
                return;

            PointCollection points = new PointCollection();

            Area area = mData.forecastData.areas[mCityIndex];
            int columnWidth = (int)(PoPChart.ActualWidth / (area.values.Count - 1));
            int idx = 0;
            List<int> dataset = new List<int>();
            List<string> xDataSet = new List<string>();

            foreach (Value value in area.values)
            {
                int dataValue = -1;
                if (value.poP.Length > 0)
                    dataValue = Convert.ToInt32(value.poP);

                dataset.Add(dataValue);
                points.Add(new Point(idx * columnWidth, PoPChart.ActualHeight / 2 - dataValue * 10));

                string slice = value.layout;
                foreach (Time time in mData.forecastData.metaData.times)
                {
                    if (time.slice == slice)
                    {
                        xDataSet.Add(parsedDate(time.fcstTimes[idx].value));
                        break;
                    }
                }

                idx++;
            }

            mMaxValue = 30;
            mMinValue = 10;
            mStepSize = 5;
            clear(PoPChartLine);
            drawBackground(dataset.Count, mPoPMaxValue, mPoPMinValue, mPoPStepSize, xDataSet, PoPChart, PoPChartXLabel, PoPChartYLabel);
            drawLineChart(dataset, mPoPMaxValue, mPoPMinValue, mPoPStepSize, PoPChart, PoPChartLine);
        }*/

        //string parsedDate(string dateValue)
        //{
        //    DateTime parsedDate;
        //    dateValue = dateValue.Substring(0, dateValue.LastIndexOf("+")) + ":00";

        //    if (DateTime.TryParseExact(dateValue, mPattern, null,
        //                           DateTimeStyles.None, out parsedDate))
        //        return Convert.ToString(parsedDate.Day);
        //    else
        //        return "";

        //}

        //public CityTown getCityTown()
        //{
        //    DateTime startTime = DateTime.Now;
        //    XmlReader reader = XmlReader.Create("CityTownSp.xml");
        //    CityTown records = (CityTown)Utils.deserialize(reader, typeof(CityTown));
        //    DateTime stopTime = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

        //    return records;
        //}

        //public CountyWeekdaysForecast getCountyWeekdaysForecast()
        //{
        //    DateTime startTime = DateTime.Now;
        //    XmlReader reader = XmlReader.Create("TAIWAN_Weekday_CH.xml");
        //    CountyWeekdaysForecast data = (CountyWeekdaysForecast)Utils.deserialize(reader, typeof(CountyWeekdaysForecast));
        //    DateTime stopTime = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

        //    return data;
        //}

        public void clear(Polyline ChartLine)
        {
            ChartLine.Points.Clear();
        }

        public void drawLineChart(List<int> dataSet, int maxValue, int minValue, int step, Canvas Chart, Polyline ChartLine)
        {
            PointCollection points = new PointCollection();
            int unitHeight = (int)mRowHeight / step;

            for (int i = 0; i < dataSet.Count; i++)
            {
                if (dataSet[i] < 0)
                    continue;

                double columnWidth = Chart.ActualWidth / (dataSet.Count - 1);
                Point point = new Point(columnWidth * i, (maxValue - dataSet[i]) * unitHeight);
                points.Add(point);
            }

            drawLineChart(points, ChartLine);
        }


        public void drawLineChart(PointCollection points, Polyline ChartLine)
        {
            foreach (Point p in points)
                ChartLine.Points.Add(p);
        }

        public void drawBackground(int dataCount, int maxValue, int minValue, int step, List<string> xLabelTexts, Canvas Chart, Grid ChartXLabel, Grid ChartYLabel)
        {
            int rowCount = (maxValue - minValue) / step;
            mRowHeight = (int)Chart.ActualHeight / rowCount;
            for (int i = 0; i < rowCount; i++)
            {
                Polygon row = new Polygon();
                row.Stroke = new SolidColorBrush(Colors.Gray);
                row.StrokeThickness = (double)this.Resources["PhoneStrokeThickness"];
                row.Points.Add(new Point(0, i * mRowHeight));
                row.Points.Add(new Point(Chart.ActualWidth, i * mRowHeight));
                row.Points.Add(new Point(Chart.ActualWidth, i * mRowHeight + mRowHeight));
                row.Points.Add(new Point(0, i * mRowHeight + mRowHeight));
                Chart.Children.Insert(0, row);
            }

            ChartYLabel.Height = Chart.ActualHeight;
            ChartYLabel.RowDefinitions.Clear();
            for (int i = 0; i <= rowCount; i++)
            {
                RowDefinition rowdef = new RowDefinition();
                if (i < rowCount)
                {
                    rowdef.Height = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    rowdef.Height = new GridLength(1, GridUnitType.Auto);
                }
                ChartYLabel.RowDefinitions.Add(rowdef);

                TextBlock label = new TextBlock();
                label.Text = Convert.ToString(maxValue - step * i);
                ChartYLabel.Children.Add(label);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
            }

            ChartXLabel.Width = Chart.ActualWidth;
            ChartXLabel.ColumnDefinitions.Clear();
            int colCount = 0;
            for (int i = 0; i < dataCount; i++)
            {
                if (i > 0 && xLabelTexts[i] == xLabelTexts[i - 1])
                    continue;

                colCount++;
            }

            for (int i = 0; i < colCount; i++)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = new GridLength(1, GridUnitType.Star);
                if (i < colCount - 1)
                {
                    coldef.Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    coldef.Width = new GridLength(1, GridUnitType.Auto);
                }
                ChartXLabel.ColumnDefinitions.Add(coldef);
            }

            int colIdx = 0;
            for (int i = 0; i < dataCount; i++)
            {
                if (i > 0 && xLabelTexts[i] == xLabelTexts[i - 1])
                    continue;

                TextBlock label = new TextBlock();
                label.Text = xLabelTexts[i];
                ChartXLabel.Children.Add(label);
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, colIdx);
                colIdx++;
            }
        }

        private void OnSelectCity(object sender, EventArgs args)
        {
            this.NavigationService.Navigate(new Uri("/View/SelectCityPage.xaml", UriKind.Relative));
        }

        private void OnSetting(object sender, EventArgs args)
        {
            this.NavigationService.Navigate(new Uri("/View/SettingPage.xaml", UriKind.Relative));
        }

        private void OnAbout(object sender, EventArgs args)
        {
            this.NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }

        private void StartPeriodicAgent()
        {
            // Obtain a reference to the period task, if one exists
            mPeriodicTask = ScheduledActionService.Find(TILE_NAME) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (mPeriodicTask != null)
            {
                RemoveAgent(TILE_NAME);
            }

            mPeriodicTask = new PeriodicTask(TILE_NAME);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            mPeriodicTask.Description = "將晴時多雲偶陣雨APP釘選到開始畫面後，會自動更新氣象資訊。";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(mPeriodicTask);
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(TILE_NAME, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    //MessageBox.Show("Background agents for this application have been disabled by the user.");
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        private void LocationButtonClick(object sender, EventArgs e)
        {
            bool enableGPS = false;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("enableGPS"))
            {
                enableGPS = (bool)settings["enableGPS"];
            }

            if (!enableGPS)
            {
                MessageBoxResult result = MessageBox.Show("允許晴時多雲偶陣雨程式取得以及使用我的位置資訊，只會拿來用在定位天氣預報地點。\n可以在設定頁面關閉這個功能。\n\n定位的結果是最近的氣象站，和所在地的門牌號碼不一定相同。", "要自動定位嗎？", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    settings["enableGPS"] = true;
                    settings.Save();

                    StartLocationService(GeoPositionAccuracy.High);
                }
            }
            else
            {
                StartLocationService(GeoPositionAccuracy.High);
            }
        }

        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            mMainPageViewModel.IsLoading = true;

            // Reinitialize the GeoCoordinateWatcher
            mWatcher = new GeoCoordinateWatcher(accuracy);
            mWatcher.MovementThreshold = 20;

            // Add event handler
            mWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            mWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            mWatcher.Start();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                case GeoPositionStatus.NoData:
                    mWatcher.Stop();
                    mMainPageViewModel.IsLoading = false;
                    mIsGeoCoordinateReady = false;
                    break;
                case GeoPositionStatus.Initializing:
                    mIsGeoCoordinateReady = false;
                    break;
                case GeoPositionStatus.Ready:
                    mIsGeoCoordinateReady = true;
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!mIsGeoCoordinateReady)
                return;

            mMainPageViewModel.IsLoading = false;
            mWatcher.Stop();

            System.Diagnostics.Debug.WriteLine("Latitude: " + e.Position.Location.Latitude.ToString("0.000"));
            System.Diagnostics.Debug.WriteLine("Longitude: " + e.Position.Location.Longitude.ToString("0.000"));

            double postionLatitude = e.Position.Location.Latitude;
            double postionLongitude = e.Position.Location.Longitude;

            XmlCityTownRepository countryRepository = new XmlCityTownRepository();
            IList<Town368> townList = countryRepository.GetTownList();

            Town368 location = null;
            double minDistance = 9999999999;
            foreach (Town368 town in townList)
            {
                double distance = Math.Abs(System.Convert.ToDouble(town.Town.Latitude) - postionLatitude) + Math.Abs(System.Convert.ToDouble(town.Town.Longitude) - postionLongitude);
                if (distance < minDistance)
                {
                    System.Diagnostics.Debug.WriteLine("minDistance: " + minDistance);
                    System.Diagnostics.Debug.WriteLine("Town: " + town.Town.Name);
                    location = town;
                    minDistance = distance;
                }
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["forecastTown"] = location.Town;
            settings["forecastCity"] = location.City;
            settings.Save();

            OnLoaded(null, null);
        }
    }
}