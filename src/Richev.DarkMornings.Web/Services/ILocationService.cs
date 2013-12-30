namespace Richev.DarkMornings.Web.Services
{
    public interface ILocationService
    {
        void GetLocationFromIPAddress(string ipAddress, out double? latitude, out double? longitude);
    }
}