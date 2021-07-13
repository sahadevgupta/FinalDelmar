using FormsLoyalty.Interfaces;
using FormsLoyalty.iOS.Services;
using Foundation;
using StoreKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppRatingService))]
namespace FormsLoyalty.iOS.Services
{
    public class AppRatingService : IAppRating
    {
        public void OpenStoreListing(string appId)
        {
            throw new NotImplementedException();
        }

        public void OpenStoreReviewPage(string appId)
        {
            throw new NotImplementedException();
        }

        public Task RateApp()
        {
            throw new NotImplementedException();
        }

        public void RateAppFromStore()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
                SKStoreReviewController.RequestReview();
            else
            {
                var storeUrl = "itms-apps://itunes.apple.com/app/com.LinkedGates.DelmarAttalla";
                var url = storeUrl + "?action=write-review";

                try
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                }
                catch (Exception ex)
                {
                    // Here you could show an alert to the user telling that App Store was unable to launch

                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}