namespace Richev.DarkMornings.Web.Models
{
    public class CommuteTime
    {
        public int Hour { get; set; }

        public int Minute { get; set; }

        public DaylightInfo Daylights { get; set; }
    }
}