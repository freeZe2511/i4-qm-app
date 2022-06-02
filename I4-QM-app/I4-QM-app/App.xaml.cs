﻿using I4_QM_app.Models;
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

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            //DependencyService.Register<OrderService>();
            //DependencyService.Register<HistoryService>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            Task.Run(async () => { await MqttConnection.ConnectClient(); });
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
            var history = db.GetCollection<Order>("history");

            return db;
        }

        // Eigenschaft für den Zugriff
        public static ILiteDatabase DB => _db.Value;


        //public static IDataStore<Order> OrdersDataStore => DependencyService.Get<IDataStore<Order>>();
        public static IDataStore<Order> OrdersDataStore => new OrderService();
        //public static IDataStore<Order> HistoryDataStore => DependencyService.Get<IDataStore<Order>>();
        public static IDataStore<Order> HistoryDataStore => new HistoryService();


        //public static int UserId { get; set; }


    }
}
