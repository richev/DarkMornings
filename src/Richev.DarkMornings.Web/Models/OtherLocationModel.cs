namespace Richev.DarkMornings.Web.Models
{
    public class OtherLocationModel
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double TimeZoneOffset { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DaylightInfoModel ToWorkDaylights { get; set; }

        public DaylightInfoModel FromWorkDaylights { get; set; }
    }
}