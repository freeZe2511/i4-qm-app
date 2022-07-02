using I4_QM_app.Models;
using I4_QM_app.Services;
using LiteDB;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

            Task.Run(async () => await App.ConnectionService.ConnectClient());
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

            return db;
        }


        public static ILiteDatabase DB => _db.Value;

        public static IDataService<Order> OrdersDataService => DependencyService.Get<IDataService<Order>>();

        public static IDataService<Recipe> RecipesDataService => DependencyService.Get<IDataService<Recipe>>();

        public static IDataService<Additive> AdditivesDataService => DependencyService.Get<IDataService<Additive>>();

        public static INotificationService NotificationService => DependencyService.Get<INotificationService>();

        public static IConnectionService ConnectionService => DependencyService.Get<IConnectionService>();
    }
}
