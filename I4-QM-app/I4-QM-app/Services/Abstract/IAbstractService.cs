namespace I4_QM_app.Services.Abstract
{
    public interface IAbstractService
    {
        string GetPreferences(string key, string fallback);

        void SetPreferences(string key, string value);

        void ClearPreferences();
    }
}
