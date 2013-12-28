namespace Richev.DarkMornings.Web.Models
{
    public class CommuteInfo
    {
        public CommuteInfo()
        {
            MorningCommute = new CommuteTime();
            EveningCommute = new CommuteTime();
            WorkingDays = new WorkingDays();
        }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public WorkingDays WorkingDays { get; set; }

        public CommuteTime MorningCommute { get; set; }

        public CommuteTime EveningCommute { get; set; }

        public string Message { get; set; }
    }
}