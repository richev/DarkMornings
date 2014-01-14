﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Richev.DarkMornings.Web.Models
{
    public class CommuteTimeModel
    {
        /// <summary>
        /// Hour
        /// </summary>
        [DisplayName("hour")]
        [Range(0, 23)]
        public int h { get; set; }

        /// <summary>
        /// Minutes
        /// </summary>
        [DisplayName("minutes")]
        [Range(0, 59)]
        public int m { get; set; }

        public DaylightInfoModel Daylights { get; set; }
    }
}