using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Services
{
    public interface ILocationService
    {
        Location? GetLocationFromIPAddress(string ipAddress);
    }
}