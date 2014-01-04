using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Richev.DarkMornings.Core
{
    public static class Utils
    {
        public static double CalculateLongitudeTimeZone(double timezone)
        {
            return timezone * 15;
        }
    }
}
