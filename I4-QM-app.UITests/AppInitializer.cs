using Xamarin.UITest;

namespace I4_QM_app.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.InstalledApp("com.companyname.i4_qm_app").StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}