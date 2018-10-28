﻿// SunCalculator.cs - Calculator for calculating SunRise, SunSet and
// maximum solar radiation of a specific date and time.
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
// http://www.codeproject.com/Articles/78486/Solar-Calculator-Calculate-Sunrise-Sunset-and-Maxi
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

        private readonly double _latitudeInRadians;

        /// <summary>
        /// The nearest longitude in multiple of 15 of the timezone of which you are calculating the sun rise or sun set.
        /// </summary>
        private readonly double _longitudeTimeZone;

        /// <summary>
        /// <para>For locations that use daylight savings, you should set UseSummerTime to the actual daylight savings status.</para>
        /// <para>For locations that don't use daylight savings, set it to false</para>
        /// </summary>
        private bool _useSummerTime;

        public SunCalculator(double longitude, double latitude)
        {
            _longitude = longitude;
            _latitudeInRadians = ConvertDegreeToRadian(latitude);
            _longitudeTimeZone = (int)(Math.Round(longitude / 15D) * 15);
        }

        public DateTime CalculateSunRise(DateTime dateTime, string timeZoneId)
        {
            _useSummerTime = Utils.IsGmtDaylightSavingTime(dateTime, timeZoneId);

            var dayNumberOfDateTime = ExtractDayNumber(dateTime);
            var differenceSunAndLocalTime = CalculateDifferenceSunAndLocalTime(dayNumberOfDateTime);
            var declinationOfTheSun = CalculateDeclination(dayNumberOfDateTime);
            var tanSunPosition = CalculateTanSunPosition(declinationOfTheSun);
            var sunRiseInMinutes = CalculateSunRiseInternal(tanSunPosition, differenceSunAndLocalTime);

            return CreateDateTime(dateTime, sunRiseInMinutes);
        }

        public DateTime CalculateSunSet(DateTime dateTime, string timeZoneId)
        {
            _useSummerTime = Utils.IsGmtDaylightSavingTime(dateTime, timeZoneId);

            var dayNumberOfDateTime = ExtractDayNumber(dateTime);
            var differenceSunAndLocalTime = CalculateDifferenceSunAndLocalTime(dayNumberOfDateTime);
            var declinationOfTheSun = CalculateDeclination(dayNumberOfDateTime);
            var tanSunPosition = CalculateTanSunPosition(declinationOfTheSun);
            var sunSetInMinutes = CalculateSunSetInternal(tanSunPosition, differenceSunAndLocalTime);

            return CreateDateTime(dateTime, sunSetInMinutes);
        }

        private static double CalculateDeclination(int numberOfDaysSinceFirstOfJanuary)
        {
            return Math.Asin(-0.39795 * Math.Cos(2.0 * Math.PI * (numberOfDaysSinceFirstOfJanuary + 10.0) / 365.0));
        }

        private static int ExtractDayNumber(DateTime dateTime)
        {
            return dateTime.DayOfYear;
        }

        private static DateTime CreateDateTime(DateTime dateTime, int timeInMinutes)
        {
            var hour = timeInMinutes / 60;
            var minute = timeInMinutes - (hour * 60);

            try
            {
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, 00, DateTimeKind.Utc);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new UserDisplayableException("Sorry, it looks like you're somewhere in the Arctic or Antarctic. Dark Mornings is rather buggy in the polar regions.", ex);
            }
        }

        private static int CalculateSunRiseInternal(double tanSunPosition, double differenceSunAndLocalTime)
        {
            var sunRise = (int)(720.0 - 720.0 / Math.PI * Math.Acos(-tanSunPosition) - differenceSunAndLocalTime);
            sunRise = LimitSunRise(sunRise);
            return sunRise;
        }

        private static int CalculateSunSetInternal(double tanSunPosition, double differenceSunAndLocalTime)
        {
            var sunSet = (int)(720.0 + 720.0 / Math.PI * Math.Acos(-tanSunPosition) - differenceSunAndLocalTime);
            sunSet = LimitSunSet(sunSet);
            return sunSet;
        }

        private double CalculateTanSunPosition(double declinationOfTheSun)
        {
            var sinSunPosition = CalculateSinSunPosition(declinationOfTheSun);
            var cosSunPosition = CalculateCosSunPosition(declinationOfTheSun);
            var tanSunPosition = sinSunPosition / cosSunPosition;
            tanSunPosition = LimitTanSunPosition(tanSunPosition);
            return tanSunPosition;
        }

        private double CalculateCosSunPosition(double declinationOfTheSun)
        {
            return Math.Cos(_latitudeInRadians) * Math.Cos(declinationOfTheSun);
        }

        private double CalculateSinSunPosition(double declinationOfTheSun)
        {
            return Math.Sin(_latitudeInRadians) * Math.Sin(declinationOfTheSun);
        }

        private double CalculateDifferenceSunAndLocalTime(int dayNumberOfDateTime)
        {
            var ellipticalOrbitPart1 = 7.95204 * Math.Sin((0.01768 * dayNumberOfDateTime) + 3.03217);
            var ellipticalOrbitPart2 = 9.98906 * Math.Sin((0.03383 * dayNumberOfDateTime) + 3.46870);

            var differenceSunAndLocalTime = ellipticalOrbitPart1 + ellipticalOrbitPart2 + (_longitude - _longitudeTimeZone) * 4;

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