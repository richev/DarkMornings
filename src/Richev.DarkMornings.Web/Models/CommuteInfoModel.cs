﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Richev.DarkMornings.Web.Models
{
    public class CommuteInfoModel
    {
        public CommuteInfoModel()
        {
            tw = new CommuteTime();
            fw = new CommuteTime();
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
        /// Working days
        /// </summary>
        [DisplayName("working days (wd)")]
        [StringLength(7)]
        public string wd { get; set; }

        /// <summary>
        /// Commute to work
        /// </summary>
        public CommuteTime tw { get; set; }

        /// <summary>
        /// Commute from work
        /// </summary>
        public CommuteTime fw { get; set; }

        public bool HasDefaultValues()
        {
            return !la.HasValue &&
                   !lo.HasValue &&
                   tw.h == 0 &&
                   tw.m == 0 &&
                   fw.h == 0 &&
                   fw.m == 0;
        }
    }
}