using System;
using System.Configuration;
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
                var url = $"http://api.hostip.info/get_xml.php?ip={ipAddress}&position=true";

                var doc = XDocument.Load(url);

                location = ResponseParser.Parse(doc);
            }
            catch (WebException)
            {
                location = null;
            }

            return location;
        }

        public string GetTimeZoneId(Location location)
        {
            var middayToday = DateTime.UtcNow.Date.AddHours(12);

            var apiKey = ConfigurationManager.AppSettings["GoogleTimeZoneApiKey"];

            var url = string.Format(
                "https://maps.googleapis.com/maps/api/timezone/json?key={0}&location={1},{2}&timestamp={3}",
                apiKey,
                location.Latitude,
                location.Longitude,
                ToUnixTimeStamp(middayToday));

            using (var client = new WebClient())
            {
                var responseJson = client.DownloadString(url);

                var response = JsonConvert.DeserializeObject<TimeZoneResponse>(responseJson);

                if (response.Status != "OK")
                {
                    var message = response.Status == "ZERO_RESULTS" ?
                        "Sorry, but the time zone for your location could not be obtained. Please make sure your location is on land." :
                        $"Sorry, but the time zone for your location could not be obtained. {response.ErrorMessage} ({response.Status})";

                    throw new UserDisplayableException(message);
                }

                var id = TimeZones.OlsonTimeZoneToTimeZoneInfo(response.TimeZoneId);

                if (id == null)
                {
                    throw new UserDisplayableException($"Sorry, but the time zone information for your location could not be found ({response.TimeZoneId}).");
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