using I4_QM_app.Models;
using I4_QM_app.Services;
using LiteDB;
using System;
using System.IO;
using System.Threading;
using Xamarin.Forms;

namespace I4_QM_app
{
    public partial class App : Application
    {
        public User CurrentUser;

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            DependencyService.Register<OrderService>();
            DependencyService.Register<RecipeService>();
            DependencyService.Register<AdditiveService>();
            DependencyService.Register<NotificationService>();
            DependencyService.Register<ConnectionService>();

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

        public static IDataService<Order> OrdersDataStore => DependencyService.Get<IDataService<Order>>();
        public static IDataService<Recipe> RecipesDataStore => DependencyService.Get<IDataService<Recipe>>();
        public static IDataService<Additive> AdditivesDataStore => DependencyService.Get<IDataService<Additive>>();
        public static Services.INotificationService NotificationService => DependencyService.Get<Services.INotificationService>();
        public static IConnectionService ConnectionService => DependencyService.Get<IConnectionService>();

    }
}
