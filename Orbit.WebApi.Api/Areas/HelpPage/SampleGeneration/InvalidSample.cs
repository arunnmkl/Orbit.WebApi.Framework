using System;

namespace Orbit.WebApi.Api.Areas.HelpPage
{
    /// <summary>
    /// This represents an invalid sample on the help page. There's a display template named InvalidSample associated with this class.
    /// </summary>
    public class InvalidSample
    {
        /// <summary>
        /// Invalid Sample
        /// </summary>
        /// <param name="errorMessage"></param>
        public InvalidSample(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            ErrorMessage = errorMessage;
        }
        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// for Equals Method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            InvalidSample other = obj as InvalidSample;
            return other != null && ErrorMessage == other.ErrorMessage;
        }
        /// <summary>
        /// for GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ErrorMessage.GetHashCode();
        }
        /// <summary>
        /// for ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ErrorMessage;
        }
    }
}