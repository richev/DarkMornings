namespace Richev.DarkMornings.Web.Models
{
    public class CommuteInfoViewModel : CommuteInfoModel
    {
        public CommuteInfoViewModel()
        {
            WorkingDays = new WorkingDays();
        }

        public WorkingDays WorkingDays { get; set; }
    }
}