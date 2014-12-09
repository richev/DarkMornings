using System;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Services
{
    public class GeoService : IGeoService
    {
        public Location? GetLocationFromIPAddress(string ipAddress)
        {
            Location? location;

            try
            {
                var url = string.Format("http://api.hostip.info/get_xml.php?ip={0}&position=true", ipAddress);

                var doc = XDocument.Load(url);

                location = ResponseParser.Parse(doc);
            }
            catch (Exception) // TODO: Less generic catch
            {
                throw new UserDisplayableException(string.Format("Sorry, but your location could not be obtained from your IP address {0}.", ipAddress));
            }

            return location;
        }

        public string GetTimeZoneId(Location location)
        {
            var middayToday = DateTime.UtcNow.Date.AddHours(12);

            var url = string.Format("https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp={2}", location.Latitude, location.Longitude, ToUnixTimeStamp(middayToday));

            using (var client = new WebClient())
            {
                var responseJson = client.DownloadString(url);

                var response = JsonConvert.DeserializeObject<TimeZoneResponse>(responseJson);

                if (response.Status != "OK")
                {
                    var message = response.Status == "ZERO_RESULTS" ?
                        "Sorry, but the time zone for your location could not be obtained. Please make sure your location is on land." :
                        string.Format("Sorry, but the time zone for your location could not be obtained ({0}: {1}).", response.Status, response.ErrorMessage);

                    throw new UserDisplayableException(message);
                }

                var id = TimeZones.OlsonTimeZoneToTimeZoneInfo(response.TimeZoneId);

                if (id == null)
                {
                    throw new UserDisplayableException(string.Format("Sorry, but the time zone information for your location could not be found ({0}).", response.TimeZoneId));
                }

                return id;
            }
        }

        private static string ToUnixTimeStamp(DateTime dateTime)
        {
            var unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);

            var unixTimeSeconds = (dateTime - unixTimeStart).TotalSeconds;

            return Convert.ToString((int)unixTimeSeconds);
        }
    }
}