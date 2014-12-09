﻿// SunCalculator.cs - Calculator for calculating SunRise, SunSet and
// maximum solar radiation of a specific date and time.
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//

using System;

namespace Richev.DarkMornings.Core
{
    /// <summary>
    /// This class is responsible for calculating sun related parameters such as
    /// SunRise, SunSet and maximum solar radiation of a specific date and time.
    /// </summary>
    public class SunCalculator
    {
        private readonly double _longitude;

        private readonly double _latituteInRadians;

        /// <summary>
        /// The nearest longitude in multiple of 15 of the timezone of which you are calculating the sun rise or sun set.
        /// </summary>
        private readonly double _longituteTimeZone;

        /// <summary>
        /// <para>For locations that use daylight savings, you should set UseSummerTime to the actual daylight savings status.</para>
        /// <para>For locations that don't use daylight savings, set it to false</para>
        /// </summary>
        private bool _useSummerTime;

        public SunCalculator(double longitude, double latitude)
        {
            _longitude = longitude;
            _latituteInRadians = ConvertDegreeToRadian(latitude);
            _longituteTimeZone = (int)(Math.Round(longitude / 15D) * 15);
        }

        public DateTime CalculateSunRise(DateTime dateTime, string timeZoneId)
        {
            _useSummerTime = Utils.IsGmtDaylightSavingTime(dateTime, timeZoneId);

            int dayNumberOfDateTime = ExtractDayNumber(dateTime);
            double differenceSunAndLocalTime = CalculateDifferenceSunAndLocalTime(dayNumberOfDateTime);
            double declanationOfTheSun = CalculateDeclination(dayNumberOfDateTime);
            double tanSunPosition = CalculateTanSunPosition(declanationOfTheSun);
            int sunRiseInMinutes = CalculateSunRiseInternal(tanSunPosition, differenceSunAndLocalTime);
            return CreateDateTime(dateTime, sunRiseInMinutes);
        }

        public DateTime CalculateSunSet(DateTime dateTime, string timeZoneId)
        {
            _useSummerTime = Utils.IsGmtDaylightSavingTime(dateTime, timeZoneId);

            int dayNumberOfDateTime = ExtractDayNumber(dateTime);
            double differenceSunAndLocalTime = CalculateDifferenceSunAndLocalTime(dayNumberOfDateTime);
            double declanationOfTheSun = CalculateDeclination(dayNumberOfDateTime);
            double tanSunPosition = CalculateTanSunPosition(declanationOfTheSun);
            int sunSetInMinutes = CalculateSunSetInternal(tanSunPosition, differenceSunAndLocalTime);
            return CreateDateTime(dateTime, sunSetInMinutes);
        }

        internal double CalculateDeclination(int numberOfDaysSinceFirstOfJanuary)
        {
            return Math.Asin(-0.39795 * Math.Cos(2.0 * Math.PI * (numberOfDaysSinceFirstOfJanuary + 10.0) / 365.0));
        }

        private static int ExtractDayNumber(DateTime dateTime)
        {
            return dateTime.DayOfYear;
        }

        private static DateTime CreateDateTime(DateTime dateTime, int timeInMinutes)
        {
            int hour = timeInMinutes / 60;
            int minute = timeInMinutes - (hour * 60);

            try
            {
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, 00, DateTimeKind.Utc);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new UserDisplayableException(string.Format("Timeout! {0}", ex.Message), ex);
            }
        }

        private static int CalculateSunRiseInternal(double tanSunPosition, double differenceSunAndLocalTime)
        {
            int sunRise = (int)(720.0 - 720.0 / Math.PI * Math.Acos(-tanSunPosition) - differenceSunAndLocalTime);
            sunRise = LimitSunRise(sunRise);
            return sunRise;
        }

        private static int CalculateSunSetInternal(double tanSunPosition, double differenceSunAndLocalTime)
        {
            int sunSet = (int)(720.0 + 720.0 / Math.PI * Math.Acos(-tanSunPosition) - differenceSunAndLocalTime);
            sunSet = LimitSunSet(sunSet);
            return sunSet;
        }

        private double CalculateTanSunPosition(double declanationOfTheSun)
        {
            double sinSunPosition = CalculateSinSunPosition(declanationOfTheSun);
            double cosSunPosition = CalculateCosSunPosition(declanationOfTheSun);
            double tanSunPosition = sinSunPosition / cosSunPosition;
            tanSunPosition = LimitTanSunPosition(tanSunPosition);
            return tanSunPosition;
        }

        private double CalculateCosSunPosition(double declanationOfTheSun)
        {
            return Math.Cos(_latituteInRadians) * Math.Cos(declanationOfTheSun);
        }

        private double CalculateSinSunPosition(double declanationOfTheSun)
        {
            return Math.Sin(_latituteInRadians) * Math.Sin(declanationOfTheSun);
        }

        private double CalculateDifferenceSunAndLocalTime(int dayNumberOfDateTime)
        {
            double ellipticalOrbitPart1 = 7.95204 * Math.Sin((0.01768 * dayNumberOfDateTime) + 3.03217);
            double ellipticalOrbitPart2 = 9.98906 * Math.Sin((0.03383 * dayNumberOfDateTime) + 3.46870);

            double differenceSunAndLocalTime = ellipticalOrbitPart1 + ellipticalOrbitPart2 + (_longitude - _longituteTimeZone) * 4;

            if (_useSummerTime) differenceSunAndLocalTime -= 60;
            return differenceSunAndLocalTime;
        }

        private static double LimitTanSunPosition(double tanSunPosition)
        {
            if (((int)tanSunPosition) < -1)
            {
                tanSunPosition = -1.0;
            }
            if (((int)tanSunPosition) > 1)
            {
                tanSunPosition = 1.0;
            }
            return tanSunPosition;
        }

        private static int LimitSunSet(int sunSet)
        {
            if (sunSet > 1439)
            {
                sunSet -= 1439;
            }
            return sunSet;
        }

        private static int LimitSunRise(int sunRise)
        {
            if (sunRise < 0)
            {
                sunRise += 1440;
            }
            return sunRise;
        }

        private static double ConvertDegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}