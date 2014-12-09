using System;

namespace Richev.DarkMornings.Core
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

        public UserDisplayableException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}