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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using CWBWeather.Model;
using System.Collections.Generic;

namespace CWBWeather
{
    public class Utils
    {
        //public static CityTown getCityTown()
        //{
        //    DateTime startTime = DateTime.Now;
        //    XmlReader reader = XmlReader.Create("Model/CityTownSp.xml");
        //    CityTown records = (CityTown)Utils.deserialize(reader, typeof(CityTown));
        //    DateTime stopTime = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

        //    return records;
        //}

/*        public static object deserialize(XmlReader reader, Type serializedObjectType)
        {
            if (serializedObjectType == null || reader == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(serializedObjectType);
            return serializer.Deserialize(reader);
        }*/

        public static void saveXML(string fileName, string content)
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

        public static StreamReader readXML(string fileName)
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

        public static String GetXmlFileName(CityTown forecastCity, string postfix)
        {
            String fileName = "";
            string forecastCityID = forecastCity.ID;

            if (forecastCityID.StartsWith("630") || forecastCityID.StartsWith("640"))
                fileName = forecastCityID.Substring(0, forecastCityID.Length - 4) + "00" + postfix;
            else
                fileName = forecastCityID.Substring(0, forecastCityID.Length - 2) + postfix;

            return fileName;
        }

        public static string GetWeatherIcon(string weatherIcon)
        {
            string fileName = "";
            switch (weatherIcon)
            {
                case "01":
                case "23":
                case "24":
                case "25":
                case "29":
                case "30":
                case "34":
                case "35":
                    fileName = "01";
                    break;
                case "02":
                case "10":
                case "11":
                case "14":
                case "19":
                    fileName = "02";
                    break;
                case "03":
                case "39":
                case "40":
                case "41":
                case "42":
                    fileName = "03";
                    break;
                case "04":
                case "26":
                case "27":
                case "28":
                    fileName = "04";
                    break;
                case "05":
                    fileName = "05";
                    break;
                case "06":
                    fileName = "06";
                    break;
                case "07":
                case "22":
                    fileName = "07";
                    break;
                case "08":
                    fileName = "08";
                    break;
                case "12":
                case "13":
                case "15":
                case "16":
                    fileName = "26";
                    break;
                case "17":
                case "18":
                case "20":
                case "21":
                case "31":
                case "32":
                case "33":
                case "36":
                case "37":
                case "38":
                case "62":
                    fileName = "31";
                    break;
                case "43":
                case "44":
                case "45":
                case "46":
                case "47":
                case "48":
                case "49":
                case "50":
                case "51":
                case "52":
                case "53":
                case "54":
                case "55":
                case "56":
                case "57":
                case "58":
                case "59":
                case "61":
                case "63":
                    fileName = "61";
                    break;
                case "60":
                case "64":
                case "65":
                    fileName = "65";
                    break;
                default:
                    fileName = "02";
                    break;
            }

            return fileName;
        }
    }
}
