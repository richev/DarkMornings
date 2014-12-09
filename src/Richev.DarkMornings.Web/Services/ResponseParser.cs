using System.Xml.Linq;
using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Services
{
    public static class ResponseParser
    {
        public static Location? Parse(XDocument response)
        {
            Location? location = null;

            XNamespace ns = "http://www.opengis.net/gml";

            var coordinates = GetNestedElementValue(response.Root, new[] { ns + "featureMember", "Hostip", "ipLocation", ns + "pointProperty", ns + "Point", ns + "coordinates" });

            if (!string.IsNullOrEmpty(coordinates))
            {
                var coordParts = coordinates.Split(',');

                var longitude = double.Parse(coordParts[0]);
                var latitude = double.Parse(coordParts[1]);

                location = new Location { Latitude = latitude, Longitude = longitude };
            }

            return location;
        }

        private static string GetNestedElementValue(XElement root, XName[] elementNames)
        {
            var element = root;

            foreach (var elementName in elementNames)
            {
                if (element == null) break;

                element = element.Element(elementName);
            }

            return element != null ? element.Value : null;
        }
    }
}