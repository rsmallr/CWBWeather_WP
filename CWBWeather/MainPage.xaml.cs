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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization;
using System.Globalization;

namespace CWBWeather
{
    public partial class MainPage : PhoneApplicationPage
    {
        int mRowHeight;
        int mMaxValue;
        int mMinValue;
        int mStepSize;
        string mPattern = "s";


        // 建構函式
        public MainPage()
        {
            InitializeComponent();
            //Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
        }

        string parsedDate(string dateValue)
        {
            DateTime parsedDate;
            dateValue = dateValue.Substring(0, dateValue.LastIndexOf("+")) + ":00";

            if (DateTime.TryParseExact(dateValue, mPattern, null,
                                   DateTimeStyles.None, out parsedDate))
                return Convert.ToString(parsedDate.Day);
            else
                return "";

        }

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    CountyWeekdaysForecast data = getCountyWeekdaysForecast();
        //    PointCollection points = new PointCollection();

        //    Area area = data.forecastData.areas[1];
        //    int columnWidth = (int) (Chart.ActualWidth / (area.values.Count - 1));
        //    int idx = 0;
        //    List<int> dataset = new List<int>();
        //    List<string> xDataSet = new List<string>();

        //    foreach (Value value in area.values)
        //    {
        //        int dataValue = Convert.ToInt32(value.maxT);
        //        dataset.Add(dataValue);
        //        points.Add(new Point(idx * columnWidth, Chart.ActualHeight / 2 - dataValue * 10));

        //        string slice = value.layout;
        //        foreach (Time time in data.forecastData.metaData.times)
        //        {
        //            if (time.slice == slice)
        //            {
        //                xDataSet.Add(parsedDate(time.fcstTimes[idx].value));
        //                break;
        //            }
        //        }

        //        idx++;
        //        System.Diagnostics.Debug.WriteLine("maxT: " + value.maxT);
        //    }

        //    mMaxValue = 30;
        //    mMinValue = 10;
        //    mStepSize = 5;
        //    clear();
        //    drawBackground(dataset.Count, mMaxValue, mMinValue, mStepSize, xDataSet);
        //    drawLineChart(dataset, mMaxValue, mMinValue, mStepSize);
        //}

        public static object deserialize(XmlReader reader, Type serializedObjectType)
        {
            if (serializedObjectType == null || reader == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(serializedObjectType);
            return serializer.Deserialize(reader);
        }


        //public CityTown getCityTown()
        //{
        //    DateTime startTime = DateTime.Now;
        //    XmlReader reader = XmlReader.Create("CityTownSp.xml");
        //    CityTown records = (CityTown)deserialize(reader, typeof(CityTown));
        //    DateTime stopTime = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

        //    return records;
        //}

        //public CountyWeekdaysForecast getCountyWeekdaysForecast()
        //{
        //    DateTime startTime = DateTime.Now;
        //    XmlReader reader = XmlReader.Create("TAIWAN_Weekday_CH.xml");
        //    CountyWeekdaysForecast data = (CountyWeekdaysForecast)deserialize(reader, typeof(CountyWeekdaysForecast));
        //    DateTime stopTime = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

        //    return data;
        //}

        //public void drawLineChart(List<int> dataSet, int maxValue, int minValue, int step)
        //{
        //    PointCollection points = new PointCollection();
        //    int unitHeight = (int)mRowHeight / step;

        //    for (int i = 0; i < dataSet.Count; i++)
        //    {
        //        double columnWidth = Chart.ActualWidth / (dataSet.Count-1);
        //        Point point = new Point(columnWidth * i, (maxValue - dataSet[i]) * unitHeight);
        //        points.Add(point);
        //    }

        //    drawLineChart(points);
        //}

        //public void clear()
        //{
        //    ChartLine.Points.Clear();
        //}


        //public void drawLineChart(PointCollection points)
        //{
        //    foreach (Point p in points)
        //        ChartLine.Points.Add(p);
        //}

        //public void drawBackground(int dataCount, int maxValue, int minValue, int step, List<string> xLabelTexts)
        //{
        //    int rowCount = (maxValue - minValue) / step;
        //    mRowHeight = (int)Chart.ActualHeight / rowCount;
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        Polygon row = new Polygon();
        //        row.Stroke = new SolidColorBrush(Colors.Gray);
        //        row.StrokeThickness = (double)this.Resources["PhoneStrokeThickness"];
        //        row.Points.Add(new Point(0, i * mRowHeight));
        //        row.Points.Add(new Point(Chart.ActualWidth, i * mRowHeight));
        //        row.Points.Add(new Point(Chart.ActualWidth, i * mRowHeight + mRowHeight));
        //        row.Points.Add(new Point(0, i * mRowHeight + mRowHeight));
        //        Chart.Children.Insert(0, row);
        //    }

        //    ChartYLabel.Height = Chart.ActualHeight;
        //    ChartYLabel.RowDefinitions.Clear();
        //    for (int i = 0; i <= rowCount; i++)
        //    {
        //        RowDefinition rowdef = new RowDefinition();
        //        if (i < rowCount)
        //        {
        //            rowdef.Height = new GridLength(1, GridUnitType.Star);
        //        }
        //        else
        //        {
        //            rowdef.Height = new GridLength(1, GridUnitType.Auto);
        //        }
        //        ChartYLabel.RowDefinitions.Add(rowdef);

        //        TextBlock label = new TextBlock();
        //        label.Text = Convert.ToString(maxValue - step * i);
        //        ChartYLabel.Children.Add(label);
        //        Grid.SetRow(label, i);
        //        Grid.SetColumn(label, 0);
        //    }

        //    ChartXLabel.Width = Chart.ActualWidth;
        //    ChartXLabel.ColumnDefinitions.Clear();
        //    int colCount = 0;
        //    for (int i = 0; i < dataCount; i++)
        //    {
        //        if (i > 0 && xLabelTexts[i] == xLabelTexts[i - 1])
        //            continue;

        //        colCount++;
        //    }

        //    for (int i = 0; i < colCount; i++)
        //    {
        //        ColumnDefinition coldef = new ColumnDefinition();
        //        coldef.Width = new GridLength(1, GridUnitType.Star);
        //        if (i < colCount - 1)
        //        {
        //            coldef.Width = new GridLength(1, GridUnitType.Star);
        //        }
        //        else
        //        {
        //            coldef.Width = new GridLength(1, GridUnitType.Auto);
        //        }
        //        ChartXLabel.ColumnDefinitions.Add(coldef);
        //    }

        //    int colIdx = 0;
        //    for (int i = 0; i < dataCount; i++)
        //    {
        //        if (i > 0 && xLabelTexts[i] == xLabelTexts[i - 1])
        //            continue;

        //        TextBlock label = new TextBlock();
        //        label.Text = xLabelTexts[i];
        //        ChartXLabel.Children.Add(label);
        //        Grid.SetRow(label, 0);
        //        Grid.SetColumn(label, colIdx);
        //        colIdx++;
        //    }
        //}
    }
}