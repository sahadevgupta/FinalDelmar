using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using FormsLoyalty.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormsLoyalty.Droid.Services;
using Android.Content.PM;
using Android.Webkit;

[assembly:Dependency(typeof(AppSettingsService))]
namespace FormsLoyalty.Droid.Services
{
    class AppSettingsService : IAppSettings
    {
        public bool CheckIfAppInstalledOrNot(string packageName)
        {
            try
            {

                PackageManager pm = Xamarin.Essentials.Platform.CurrentActivity.PackageManager;
               
                try
                {
                    pm.GetPackageInfo("com.facebook.katana", PackageInfoFlags.Activities);
                    return true;
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    return false;
                }
                //return flag;

                //Xamarin.Essentials.Platform.CurrentActivity.PackageManager.GetPackageInfo(packageName, PackageInfoFlags.MetaData);
                //return true;
            }
            catch (PackageManager.NameNotFoundException ex)
            {
                return false;
            }
        }

        public void ClearAllCookies()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }

        public string GetOSVersion()
        {
            string version = Android.OS.Build.VERSION.SdkInt.ToString();
            return version;
        }

        public void SwitchToBackground()
        {
            Intent homeIntent = new Intent(Intent.ActionMain);
            homeIntent.AddCategory(Intent.CategoryHome);
            homeIntent.SetFlags(ActivityFlags.NewTask);
            Xamarin.Essentials.Platform.CurrentActivity.StartActivity(homeIntent);
            //return true;
        }
    }
}