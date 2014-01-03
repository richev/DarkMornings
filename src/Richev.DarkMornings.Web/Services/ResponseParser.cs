using System.Xml.Linq;

namespace Richev.DarkMornings.Web.Services
{
    public static class ResponseParser
    {
        public static void Parse(XDocument response, out double? latitude, out double? longitude)
        {
            XNamespace ns = "http://www.opengis.net/gml";

            var coordinates = GetNestedElementValue(response.Root, new[] { ns + "featureMember", "Hostip", "ipLocation", ns + "pointProperty", ns + "Point", ns + "coordinates" });

            if (!string.IsNullOrEmpty(coordinates))
            {
                var coordParts = coordinates.Split(',');

                longitude = double.Parse(coordParts[0]);
                latitude = double.Parse(coordParts[1]);
            }
            else
            {
                longitude = null;
                latitude = null;
            }
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