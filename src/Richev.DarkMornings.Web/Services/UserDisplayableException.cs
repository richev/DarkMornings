using System;

namespace Richev.DarkMornings.Web.Services
{
    /// <summary>
    /// An exception containing information that will be shown to the user.
    /// </summary>
    public class UserDisplayableException : Exception
    {
        public UserDisplayableException(string message)
            : base(message)
        {

        }
    }
}