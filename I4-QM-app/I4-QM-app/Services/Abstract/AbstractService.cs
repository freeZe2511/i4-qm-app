using Xamarin.Essentials;

namespace I4_QM_app.Services.Abstract
{
    /// <summary>
    /// Implementation of IAbstractService for Xamarin.
    /// </summary>
    public class AbstractService : IAbstractService
    {
        /// <summary>
        /// Get Preferences from device with Xamarin Essentials.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="fallback">Fallback.</param>
        /// <returns>String value.</returns>
        public string GetPreferences(string key, string fallback)
        {
            return Preferences.Get(key, fallback);
        }

        /// <summary>
        /// Set Preferences on device with Xamarin Essentials.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetPreferences(string key, string value)
        {
            Preferences.Set(key, value);
        }

        /// <summary>
        /// Clear Preferences from device with Xamarin Essentials.
        /// </summary>
        public void ClearPreferences()
        {
            Preferences.Clear();
        }
    }
}
