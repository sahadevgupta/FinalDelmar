using System;
using FormsLoyalty.Interfaces;
using FormsLoyalty.iOS.Services;
using Foundation;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppSettingsServices))]

namespace FormsLoyalty.iOS.Services
{
    public class AppSettingsServices : IAppSettings
    {
        
        public bool CheckIfAppInstalledOrNot(string packageName)
        {
            throw new NotImplementedException();
        }

        public void ClearAllCookies()
        {

            //NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
            //foreach (var cookie in CookieStorage.Cookies)
            //{
            //    CookieStorage.DeleteCookie(cookie);
            //}

            NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast);

            WKWebsiteDataStore.DefaultDataStore.FetchDataRecordsOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, (NSArray records) =>
            {

                for (System.nuint i = 0; i < records.Count; i++)
                {
                    var record = records.GetItem<WKWebsiteDataRecord>(i);
                    WKWebsiteDataRecord[] recordArray = new WKWebsiteDataRecord[record.DataTypes.Count];
                    WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(record.DataTypes, NSDate.DistantPast, () => { });
                }

            });
        }

        public string GetOSVersion()
        {
            throw new NotImplementedException();
        }

        public void SwitchToBackground()
        {
            throw new NotImplementedException();
        }
    }
}

