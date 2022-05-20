using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
