using LSRetail.Omni.Domain.DataModel.Base.Retail;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Helpers
{
    public  class DbHelper
    {
        public const string DatabaseFilename = "Delmar_mobile_app.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache|
            SQLiteOpenFlags.FullMutex;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        static SQLiteAsyncConnection sqLiteConnection;


        public async static void InitializeDatabase()
        {
            var instance = new DbHelper();
            var databaseInfo = await sqLiteConnection.GetTableInfoAsync(nameof(PublishedOffer));
            if (!databaseInfo.Any())
            {
                await sqLiteConnection.CreateTableAsync<PublishedOffer>();
            }
        }


        //public static readonly AsyncLazy<DbHelper> InitializeDatabase = new AsyncLazy<DbHelper>(async () =>
        //{
        //    var instance = new DbHelper();

        //    var databaseInfo = await sqLiteConnection.GetTableInfoAsync(nameof(PublishedOffer));
        //    if (!databaseInfo.Any())
        //    {
        //       await sqLiteConnection.CreateTableAsync<PublishedOffer>();
        //    }

        //    return instance;
        //});

        public DbHelper()
        {
            sqLiteConnection = new SQLiteAsyncConnection(DatabasePath, Flags);
        }

    }

   
    public class AsyncLazy<T>
    {
        readonly Lazy<Task<T>> instance;

        public AsyncLazy(Func<T> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public AsyncLazy(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }

        public void Start()
        {
            var unused = instance.Value;
        }
    }

}
