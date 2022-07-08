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
    /// <summary>
    /// Main App class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();

            DependencyService.Register<OrderService>();
            DependencyService.Register<RecipeService>();
            DependencyService.Register<AdditiveService>();
            DependencyService.Register<NotificationService>();
            DependencyService.Register<ConnectionService>();

            MainPage = new AppShell();

            Task.Run(async () => await App.ConnectionService.ConnectClient());
        }

        /// <summary>
        /// User reference when logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// Gets database.
        /// </summary>
        public static ILiteDatabase DB => Db.Value;

        // https://blog.eugen.page/post/use_litedb_with_xamarin/
        // Threadsichere Singleton-Implementierung mit Lazy
        private static readonly Lazy<ILiteDatabase> Db = new Lazy<ILiteDatabase>(CreateDatabase, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets orders service.
        /// </summary>
        public static IDataService<Order> OrdersDataService => DependencyService.Get<IDataService<Order>>();

        /// <summary>
        /// Gets recipes service.
        /// </summary>
        public static IDataService<Recipe> RecipesDataService => DependencyService.Get<IDataService<Recipe>>();

        /// <summary>
        /// Gets additives service.
        /// </summary>
        public static IDataService<Additive> AdditivesDataService => DependencyService.Get<IDataService<Additive>>();

        /// <summary>
        /// Gets notifications service.
        /// </summary>
        public static INotificationService NotificationService => DependencyService.Get<INotificationService>();

        /// <summary>
        /// Gets connection service.
        /// </summary>
        public static IConnectionService ConnectionService => DependencyService.Get<IConnectionService>();

        /// <summary>
        /// On Start.
        /// </summary>
        protected override void OnStart()
        {
            // empty
        }

        /// <summary>
        /// On Sleep.
        /// </summary>
        protected override void OnSleep()
        {
            // empty
        }

        /// <summary>
        /// On Resume.
        /// </summary>
        protected override void OnResume()
        {
            // empty
        }

        /// <summary>
        /// Create LiteDb database.
        /// </summary>
        /// <returns>Database.</returns>
        private static ILiteDatabase CreateDatabase()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydb.db");
            var connection = new ConnectionString
            {
                Filename = path,
                Connection = ConnectionType.Direct,
            };

            return new LiteDatabase(connection);
        }
    }
}
