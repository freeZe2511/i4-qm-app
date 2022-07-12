using I4_QM_app.Services.Abstract;
using System.Collections.Generic;

namespace I4_QM_app.NUnitTests.MockServices
{
    internal class MockAbstractService : IAbstractService
    {
        private readonly IDictionary<string, string> preferences;

        public MockAbstractService()
        {
            preferences = new Dictionary<string, string>();
        }

        public void ClearPreferences()
        {
            preferences.Clear();
        }

        public string GetPreferences(string key, string fallback)
        {
            if (preferences.ContainsKey(key))
            {
                return preferences[key];
            }
            return fallback;
        }

        public void SetPreferences(string key, string value)
        {
            preferences[key] = value;
        }
    }
}
