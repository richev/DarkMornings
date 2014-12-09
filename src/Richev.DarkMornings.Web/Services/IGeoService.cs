using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Services
{
    /// <summary>
    /// Service wrapping geographic things
    /// </summary>
    public interface IGeoService
    {
        Location? GetLocationFromIPAddress(string ipAddress);

        /// <summary>
        /// Gets the IANA time zone id for the location.
        /// </summary>
        string GetTimeZoneId(Location location);
    }
}