using System;
using Orbit.WebApi.Core.Interfaces;

namespace Orbit.WebApi.Extensions.Validation
{
    /// <summary>
    /// Class CSRFValidation.
    /// </summary>
    public class CSRFValidation : ICSRFValidation
    {
        /// <summary>
        /// Gets the CSRF value.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCSRFValue()
        {
            return Math.Abs((new Random()).Next(Guid.NewGuid().GetHashCode(), int.MaxValue)).ToString();
        }

        /// <summary>
        /// Validates the specified actual.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="expected">The expected.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Validate(string actual, string expected)
        {
            return !string.Equals(actual, expected);
        }
    }
}