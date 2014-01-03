using System;
using System.Xml.Linq;

namespace Richev.DarkMornings.Web.Services
{
    public class LocationService : ILocationService
    {
        public void GetLocationFromIPAddress(string ipAddress, out double? latitude, out double? longitude)
        {
            try
            {
                var url = string.Format("http://api.hostip.info/get_xml.php?ip={0}&position=true", ipAddress);

                var doc = XDocument.Load(url);

                ResponseParser.Parse(doc, out latitude, out longitude);
            }
            catch (Exception) // a problem during parsing
            {
                latitude = null;
                longitude = null;
            }
        }

    }
}