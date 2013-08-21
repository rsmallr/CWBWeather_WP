using System;
using System.IO.IsolatedStorage;
using CWBWeather.Model;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using CWBWeather;
using WeatherForecast.Util;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace WeatherForecast
{
    public class TileUpdateWorker
    {
        public delegate void UpdateCompletedHandler(object sender);

        public event UpdateCompletedHandler UpdateCompleted;

        string XML_FOLDER = "http://www.cwb.gov.tw/township/XML/";
        //string XML_72HR_POSTFIX = "_72hr_CH.xml";
        string XML_WEEKDAY_POSTFIX = "_Weekday_CH.xml";
        string DEAD_TILE = "/WeatherForecast;component/Image/Tile/dead.png";

        CityTown mForecastCity;
        CityTown mForecastTown;
        ForecastData mForecastData;
        DateTime mLastLaunchTime;

        public TileUpdateWorker()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("forecastCity") || !settings.Contains("forecastTown"))
            {
                return;
            }

            mForecastCity = (CityTown)settings["forecastCity"];
            mForecastTown = (CityTown)settings["forecastTown"];

            if (settings.Contains("lastLaunchTime"))
                mLastLaunchTime = (DateTime)settings["lastLaunchTime"];
        }

        public void Update()
        {
            if (mForecastCity == null || mForecastTown == null)
            {
                OnUpdateCompleted();
                return;
            }

            if (mLastLaunchTime != null)
            {
                TimeSpan difference = DateTime.Now - mLastLaunchTime;

                if (difference.Days >= 13)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        UpdateDeadTile();
                        OnUpdateCompleted();
                    });
                    return;
                }
            }


            GetLatestForecast();
        }

        public void UpdateFromXml()
        {
            if (mForecastCity == null || mForecastTown == null)
                return;

            mForecastData = GetLatestForecastFromXML();
            if (mForecastData != null)
                UpdateTile(mForecastCity.Name + mForecastTown.Name);
        }

        public void UpdateTile(string title)
        {
            CreateTileBitmap();

            // set the properties to update for the Application Tile
            // Empty strings for the text values and URIs will result in the property being cleared.
            StandardTileData NewTileData = new StandardTileData
            {
                Title = title,
                BackgroundImage = new Uri("isostore:/Shared/ShellContent/Tile.jpg", UriKind.Absolute)
            };

            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile applicationTile = ShellTile.ActiveTiles.First();

            // Update the Application Tile
            applicationTile.Update(NewTileData);
        }

        public void UpdateDeadTile()
        {
            CreateDeadTileBitmap();

            // set the properties to update for the Application Tile
            // Empty strings for the text values and URIs will result in the property being cleared.
            StandardTileData NewTileData = new StandardTileData
            {
                Title = " ",
                BackgroundImage = new Uri("isostore:/Shared/ShellContent/Tile.jpg", UriKind.Absolute)
            };

            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile applicationTile = ShellTile.ActiveTiles.First();

            // Update the Application Tile
            applicationTile.Update(NewTileData);
        }

        public void CreateDeadTileBitmap()
        {
            // load your image
            StreamResourceInfo info = Application.GetResourceStream(new Uri(DEAD_TILE, UriKind.Relative));
            // create source bitmap for Image control (image is assumed to be alread 173x173)
            WriteableBitmap wbmp = new WriteableBitmap(173, 173);
            wbmp.SetSource(info.Stream);

            // save image to isolated storage
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // use of "/Shared/ShellContent/" folder is mandatory!
                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/Tile.jpg", System.IO.FileMode.Create, isf))
                {
                    wbmp.SaveJpeg(imageStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
                }
            }
        }

        public void CreateTileBitmap()
        {
            // grid is container for image and text
            Grid grid = new Grid();

            // load your image
            StreamResourceInfo info = Application.GetResourceStream(new Uri(GetImagePath(), UriKind.Relative));
            // create source bitmap for Image control (image is assumed to be alread 173x173)
            WriteableBitmap wbmp2 = new WriteableBitmap(1, 1);
            wbmp2.SetSource(info.Stream);
            Image img = new Image();
            img.Source = wbmp2;
            // add Image to Grid
            grid.Children.Add(img);

            TextBlock shadow = new TextBlock() {
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(13, 7, 0, 0)
            };
            // your text goes here:
            shadow.Text = mForecastData.MinT + "~" + mForecastData.MaxT + "°\n" + mForecastData.Rain + "%";
            grid.Children.Add(shadow);

            TextBlock text = new TextBlock()
            {
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(12, 6, 0, 0)
            };
            // your text goes here:
            text.Text = mForecastData.MinT + "~" + mForecastData.MaxT +"°\n" + mForecastData.Rain + "%";
            grid.Children.Add(text);

            // this is our final image containing custom text and image
            WriteableBitmap wbmp = new WriteableBitmap(173, 173);

            // now render everything - this image can be used as background for tile
            wbmp.Render(grid, null);
            wbmp.Invalidate();

            // save image to isolated storage
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // use of "/Shared/ShellContent/" folder is mandatory!
                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/Tile.jpg", System.IO.FileMode.Create, isf))
                {
                    wbmp.SaveJpeg(imageStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
                }
            }
        }

        private string GetImagePath()
        {
            if (mForecastData == null)
                return null;

            string weatherIcon = mForecastData.WeatherIcon;
            int prefixIndex = weatherIcon.IndexOf("Weather") + 7;
            weatherIcon = weatherIcon.Substring(prefixIndex, weatherIcon.Length - prefixIndex);
            int postfixIndex = weatherIcon.IndexOf(".");
            weatherIcon = weatherIcon.Substring(0, postfixIndex);

            string icon = Utils.GetWeatherIcon(weatherIcon);

            return "/WeatherForecast;component/Image/Tile/" + Utils.GetWeatherIcon(weatherIcon) + ".png";
        }

        public void GetLatestForecast()
        {
            //String uri = XML_FOLDER + Utils.GetXmlFileName(mForecastTown, XML_72HR_POSTFIX);
            String uri = XML_FOLDER + Utils.GetXmlFileName(mForecastTown, XML_WEEKDAY_POSTFIX);

            ForecastDownloader forecastDownloader = new ForecastDownloader();
            forecastDownloader.DownloadStringCompleted += ((sender, e) =>
            {
                if (e.Error != null || e.Result == null)
                {
                }
                else
                {
                    //Utils.saveXML(Utils.GetXmlFileName(mForecastTown, XML_72HR_POSTFIX), e.Result);
                    Utils.saveXML(Utils.GetXmlFileName(mForecastTown, XML_WEEKDAY_POSTFIX), e.Result);
                    UpdateFromXml();
                }

                OnUpdateCompleted();
            });
            forecastDownloader.DownloadStringAsync(new Uri(uri, UriKind.Absolute));
        }

        public void OnUpdateCompleted()
        {
            if (UpdateCompleted != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    UpdateCompleted(this);
                });
            }
        }

        public ForecastData GetLatestForecastFromXML()
        {
            StreamReader streamReader = Utils.readXML(Utils.GetXmlFileName(mForecastTown, XML_WEEKDAY_POSTFIX));

            if (streamReader == null)
            {
                return null;
            }

            XDocument loadedData = XDocument.Load(streamReader);
            streamReader.Close();
            streamReader.Dispose();

            var timeData = from query in loadedData.Descendants("FcstTime")
                           select (string)query;

            IList<string> timeDataList = timeData.ToList();

            var forecastData = from c1 in loadedData.Descendants("Area")
                               where c1.Attribute("AreaID").Value == mForecastTown.ID
                               from c2 in c1.Elements()
                               select new ForecastData
                               {
                                   Temperature = (string)c2.Element("Temperature"),
                                   Wx = (string)c2.Element("Wx"),
                                   WeatherDes = (string)c2.Element("WeatherDes"),
                                   WeatherIcon = (string)c2.Element("WeatherIcon"),
                                   RH = (string)c2.Element("RH"),
                                   Rain = ((string)c2.Element("PoP")).Length > 0 ? ((string)c2.Element("PoP")) : "",
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


            return forecastData.ElementAt(0);
        }
    }
}
