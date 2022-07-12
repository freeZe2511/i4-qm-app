namespace I4_QM_app.Services.Abstract
{
    /// <summary>
    /// Interface for some abstractions from Xamarin.
    /// </summary>
    public interface IAbstractService
    {
        /// <summary>
        /// Get Preferences from device.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="fallback">Fallback.</param>
        /// <returns>String value.</returns>
        string GetPreferences(string key, string fallback);

        /// <summary>
        /// Set Preferences on device.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        void SetPreferences(string key, string value);

        /// <summary>
        /// Clear Preferences from device.
        /// </summary>
        void ClearPreferences();
    }
}
