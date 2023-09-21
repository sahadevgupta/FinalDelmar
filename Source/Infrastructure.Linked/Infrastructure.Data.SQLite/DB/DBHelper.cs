
#if NET4
#define SILVERLIGHT
#endif

using System;
using System.Linq;
using Infrastructure.Data.SQLite.DB.DTO;
using SQLite;

namespace Infrastructure.Data.SQLite.DB
{
    public static class DBHelper
    {
        private static string dbName = "LSRetailLoyaltyDB.db3";
        private static SQLiteConnection mySQLiteDBConnection = null;

        public static SQLiteConnection DBConnection
        {
            get
            {
                if (mySQLiteDBConnection == null)
                    OpenDBConnection();
                return mySQLiteDBConnection;
            }
        }

        public static string DatabaseFilePath
        {
            get
            {
#if SILVERLIGHT || WP
                var path = dbName;
#else

                //JIJ __ANDROID__ shall always be defined by the Android toolchain, without a need for special flags in project.
#if __ANDROID__
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //Environment.SpecialFolder.Personal -> /data/data/AndroidApplication1.AndroidApplication1/files/LsRetailDB.db3
                //use adb.exe shell     or pull file to desktop machine
                //adb.exe pull /data/data/AndroidApplication1.AndroidApplication1/files/LsRetailDB.db3 c:\temp 
                 
#else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = System.IO.Path.Combine(documentsPath, "../Library/");
#endif

                var path = System.IO.Path.Combine(libraryPath, dbName);
#endif
                return path;
            }
        }

        public static void OpenDBConnection()
        {
  
            if (mySQLiteDBConnection == null)
            {
                mySQLiteDBConnection = new SQLiteConnection(DatabaseFilePath); //opens connection in construtor

                var deviceDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(DeviceData));
                if (!deviceDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<DeviceData>();
                }

                var webserviceDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(WebserviceData));
                if (!webserviceDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<WebserviceData>();
                }

                var offerDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(OfferData));
                if (!offerDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<OfferData>();
                }

                var memberContactDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(MemberContactData));
                if (!memberContactDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<MemberContactData>();
                }

                var couponDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(CouponData));
                if (!couponDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<CouponData>();
                }

                var notificationDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(NotificationData));
                if (!notificationDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<NotificationData>();
                }

                var transactionDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(TransactionData));
                if (!transactionDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<TransactionData>();
                }

                var addressessDataInfo = mySQLiteDBConnection.GetTableInfo(nameof(AddressessData));
                if (!addressessDataInfo.Any())
                {
                    mySQLiteDBConnection.CreateTable<AddressessData>();
                }
            }
        }

        public static void CloseDBConnection()
        {
            if (mySQLiteDBConnection != null)
            {
                mySQLiteDBConnection.Dispose();
                mySQLiteDBConnection = null;
            }
        }

    }
}

