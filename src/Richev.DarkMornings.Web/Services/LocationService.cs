using System;
using System.Xml.Linq;
using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Services
{
    public class LocationService : ILocationService
    {
        public Location? GetLocationFromIPAddress(string ipAddress)
        {
            Location? location = null;

            try
            {
                var url = string.Format("http://api.hostip.info/get_xml.php?ip={0}&position=true", ipAddress);

                var doc = XDocument.Load(url);

                location = ResponseParser.Parse(doc);
            }
            catch (Exception) // TODO: Less generic catch
            {
                // a problem during parsing
            }

            return location;
        }

    }
}