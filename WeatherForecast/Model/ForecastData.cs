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

namespace CWBWeather.Model
{
    public class ForecastData
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Temperature { get; set; } // 溫度

        public string RH { get; set; }  // 相對溼度

        public string Rain { get; set; } // 降雨機率

        //public string Cloud { get; set; } // 雲量

        public string WindSpeed { get; set; } // 風速

        public string WindDir { get; set; } // 風向

        public string MinCI { get; set; } // 低舒適度

        public string MaxCI { get; set; } // 高舒適度

        public string CI { get; set; } // 舒適度描述

        public string MinT { get; set; } // 低溫

        public string MaxT { get; set; } // 高溫

        public string WindLevel { get; set; } // 風力

        public string WeatherDes { get; set; } // 天氣描述

        public string Wx { get; set; } // 天氣狀況

        public string WeatherIcon { get; set; } // 天氣圖示

        public string Time { get; set; }
    }
}
