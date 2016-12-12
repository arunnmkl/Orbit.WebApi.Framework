namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface ICSRFValidation
    /// </summary>
    public interface ICSRFValidation
	{
		/// <summary>
		/// Gets the CSRF value.
		/// </summary>
		/// <returns>System.String.</returns>
		string GetCSRFValue();

		/// <summary>
		/// Validates the specified actual.
		/// </summary>
		/// <param name="actual">The actual.</param>
		/// <param name="expected">The expected.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		bool Validate(string actual, string expected);
	}
}
