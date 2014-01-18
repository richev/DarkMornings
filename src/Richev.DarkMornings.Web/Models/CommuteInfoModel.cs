using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Richev.DarkMornings.Web.Models
{
    public class CommuteInfoModel
    {
        public CommuteInfoModel()
        {
            tw = new CommuteTimeModel();
            fw = new CommuteTimeModel();
            wd = string.Empty;
        }

        /// <summary>
        /// Latitude
        /// </summary>
        [DisplayName("latitude (la)")]
        [Range(-90, 90)]
        public double? la { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [DisplayName("longitude (lo)")]
        [Range(-180, 180)]
        public double? lo { get; set; }

        /// <summary>
        /// Journey duration minutes
        /// </summary>
        [DisplayName("journey duration minutes (d)")]
        [Range(5, 120)]
        public int d { get; set; }

        /// <summary>
        /// Timezone hours offset
        /// </summary>
        [DisplayName("timezone hours offset (wd)")]
        [Required]
        [Range(-12, 13)]
        public double? tz { get; set; } // not an int, because some timezones are half an hour out

        /// <summary>
        /// Working days
        /// </summary>
        [DisplayName("working days (wd)")]
        [StringLength(7, MinimumLength = 7)]
        public string wd { get; set; }

        /// <summary>
        /// Commute to work
        /// </summary>
        public CommuteTimeModel tw { get; set; }

        /// <summary>
        /// Commute from work
        /// </summary>
        public CommuteTimeModel fw { get; set; }

        public string MapImageUrl
        {
            get
            {
                var queryStringParameters = new NameValueCollection
                            {
                                { "center", string.Format("{0},{1}", la, lo) },
                                { "zoom", "6" },
                                { "scale", "2" },
                                { "size", "320x160" },
                                { "markers", string.Format("color:0xf39c12|label:X|{0},{1}", la, lo) },
                                { "sensor", "false" },
                                { "ApiKey", "AIzaSyCBYJC-vYYyh6GtkQHWPirwHprXL9C7ylE" }
                            };

                return string.Format(
                    "http://maps.googleapis.com/maps/api/staticmap{0}",
                    Builders.BuildQueryString(queryStringParameters));
            }
        }

        public bool HasDefaultValues()
        {
            return !la.HasValue &&
                   !lo.HasValue &&
                   !tz.HasValue &&
                   tw.h == 0 &&
                   tw.m == 0 &&
                   fw.h == 0 &&
                   fw.m == 0 &&
                   d == 0;
        }
    }
}