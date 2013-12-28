using System;
using System.Xml.Linq;

namespace Richev.DarkMornings.Web
{
    public static class Utils
    {
        public static void GetLocationFromIPAddress(string ipAddress, out double? latitude, out double? longitude)
        {
            try
            {
                var url = string.Format("http://api.hostip.info/get_xml.php?ip={0}&position=true", ipAddress);

                var doc = XDocument.Load(url);

                XNamespace ns = "gml";

                var coordinates = doc.Root.Element(ns + "featureMember").Element("ipLocation").Element(ns + "pointProperty").Element(ns + "Point").Element(ns + "coordinates").Value;

                var coordParts = coordinates.Split(',');

                latitude = double.Parse(coordParts[0]);
                longitude = double.Parse(coordParts[1]);
            }
            catch (Exception) // a problem during parsing
            {
                latitude = null;
                longitude = null;
            }
        }
    }
}