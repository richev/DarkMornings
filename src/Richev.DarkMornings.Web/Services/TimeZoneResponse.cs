using Newtonsoft.Json;

namespace Richev.DarkMornings.Web.Services
{
    public class TimeZoneResponse
    {
        [JsonProperty("dstOffset")]
        public double DstOffset { get; set; }

        [JsonProperty("rawOffset")]
        public double RawOffset { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeZoneId")]
        public string TimeZoneId { get; set; }

        [JsonProperty("timeZoneName")]
        public string TimeZoneName { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}