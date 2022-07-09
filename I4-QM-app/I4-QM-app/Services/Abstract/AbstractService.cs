using Xamarin.Essentials;

namespace I4_QM_app.Services.Abstract
{
    public class AbstractService : IAbstractService
    {
        public string GetPreferences(string key, string fallback)
        {
            return Preferences.Get(key, fallback);
        }

        public void SetPreferences(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public void ClearPreferences()
        {
            Preferences.Clear();
        }
    }
}
