using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CWBWeather.Model
{
    public class XmlCityTownRepository
    {
        //private static List<CityTown> countryList = null;

        public XmlCityTownRepository()
        {

        }

        public System.Collections.Generic.IList<CityTown> GetCountryList()
        {
            DateTime startTime = DateTime.Now;

            XDocument loadedData = XDocument.Load("Model/CityTownSp.xml");
            var townData = from c1 in loadedData.Descendants("City")
                           select new CityTown
                           {
                               ID = (string)c1.Attribute("COUN_ID").Value,
                               Name = (string)c1.Attribute("COUN_NA").Value
                           };
            List<CityTown> countryList = townData.ToList();

            DateTime stopTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

            return countryList;
        }

        public System.Collections.Generic.IList<CityTown> GetTownList(CityTown selectedCity)
        {
            DateTime startTime = DateTime.Now;
            //XmlReader reader = XmlReader.Create("Model/CityTownSp.xml");
            //countryList = ((CityTown)Utils.deserialize(reader, typeof(CityTown))).Cities;
            XDocument loadedGT_Town368Data = XDocument.Load("Model/GT_Town368Data.xml");
            var town368Data = from query in loadedGT_Town368Data.Descendants("Area")
                              select new Town368Data
                              {
                                  ID = (string)query.Attribute("AreaID").Value,
                                  Longitude = (string)query.Attribute("lon").Value,
                                  Latitude = (string)query.Attribute("lat").Value
                              };

            XDocument loadedData = XDocument.Load("Model/CityTownSp.xml");

            var cityData = from query in loadedData.Descendants("City")
                           where query.Attribute("COUN_ID").Value == selectedCity.ID
                           select query;

            var townData = from c1 in cityData.Descendants("Town")
                           join c2 in town368Data on c1.Attribute("TOWN_ID").Value equals c2.ID
                           select new CityTown
                           {
                               ID = (string)c1.Attribute("TOWN_ID").Value,
                               Name = (string)c1.Attribute("TOWN_NA").Value,
                               Latitude = c2.Latitude,
                               Longitude = c2.Longitude
                           };
            List<CityTown> countryList = townData.ToList();

            DateTime stopTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

            return countryList;
        }

        public System.Collections.Generic.IList<Town368> GetTownList()
        {
            List<Town368> townList = new List<Town368>();

            DateTime startTime = DateTime.Now;
            //XmlReader reader = XmlReader.Create("Model/CityTownSp.xml");
            //countryList = ((CityTown)Utils.deserialize(reader, typeof(CityTown))).Cities;
            XDocument loadedGT_Town368Data = XDocument.Load("Model/GT_Town368Data.xml");
            var town368Data = from query in loadedGT_Town368Data.Descendants("Area")
                              select new Town368Data
                              {
                                  ID = (string)query.Attribute("AreaID").Value,
                                  Longitude = (string)query.Attribute("lon").Value,
                                  Latitude = (string)query.Attribute("lat").Value
                              };

            XDocument loadedData = XDocument.Load("Model/CityTownSp.xml");

            var cityData = from query in loadedData.Descendants("City")
                           select query;

            foreach (var city in cityData)
            {
                var townData = from c1 in city.Descendants("Town")
                               join c2 in town368Data on c1.Attribute("TOWN_ID").Value equals c2.ID
                               select new Town368
                               {
                                   City = new CityTown
                                   {
                                       ID = (string)city.Attribute("COUN_ID").Value,
                                       Name = (string)city.Attribute("COUN_NA").Value
                                   },
                                   Town = new CityTown
                                   {
                                       ID = (string)c1.Attribute("TOWN_ID").Value,
                                       Name = (string)c1.Attribute("TOWN_NA").Value,
                                       Latitude = c2.Latitude,
                                       Longitude = c2.Longitude
                                   }
                               };
                townList.AddRange(townData);
            }

            DateTime stopTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Elapsed: {0}", (stopTime - startTime).TotalMilliseconds);

            return townList;
        }
    }
}
