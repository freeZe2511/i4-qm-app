using I4_QM_app.Models;
using I4_QM_app.Services;
using LiteDB;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            DependencyService.Register<OrderService>();
            DependencyService.Register<RecipeService>();
            DependencyService.Register<AdditiveService>();

            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;

            MainPage = new AppShell();
            //MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            Task.Run(async () => { await MqttConnectionService.ConnectClient(); });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        // https://blog.eugen.page/post/use_litedb_with_xamarin/
        // Threadsichere Singleton-Implementierung mit Lazy
        private static Lazy<ILiteDatabase> _db = new Lazy<ILiteDatabase>(CreateDatabase, LazyThreadSafetyMode.PublicationOnly);

        private static ILiteDatabase CreateDatabase()
        {
            // Datenbank initialisieren
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydb.db");
            var connection = new ConnectionString
            {
                Filename = path,
                Connection = ConnectionType.Direct
            };

            var db = new LiteDatabase(connection);
            var orders = db.GetCollection<Order>("orders");
            var recipes = db.GetCollection<Order>("recipes");
            var additives = db.GetCollection<Order>("additives");

            return db;
        }

        // Eigenschaft für den Zugriff
        public static ILiteDatabase DB => _db.Value;

        public static IDataStore<Order> OrdersDataStore => DependencyService.Get<IDataStore<Order>>();
        public static IDataStore<Recipe> RecipesDataStore => DependencyService.Get<IDataStore<Recipe>>();
        public static IDataStore<Additive> AdditivesDataStore => DependencyService.Get<IDataStore<Additive>>();

        private void LoadPageFromNotification(NotificationEventArgs e)
        {
            var data = e.Request.ReturningData;

            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            // TODO
            Page page = null;
            if (data == "OrdersPage") page = new Views.OrdersPage();

            Shell.Current.Navigation.PushAsync(page);

            //await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");


        }


    }
}
