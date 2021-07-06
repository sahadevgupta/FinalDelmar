using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Com.Google.Android.Play.Core.Review;
using Com.Google.Android.Play.Core.Review.Testing;
using Com.Google.Android.Play.Core.Tasks;
using Plugin.StoreReview.Abstractions;
using System;
using FormsLoyalty.Interfaces;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Plugin.StoreReview;
using FormsLoyalty.Services;
using Application = Xamarin.Essentials.Platform;
using FormsLoyalty.Droid;
using Android.Content.PM;

[assembly:Dependency(typeof(AppRatingService))]
namespace FormsLoyalty.Services
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    [Preserve(AllMembers = true)]
    public class AppRatingService : Java.Lang.Object, IAppRating, IOnCompleteListener
    {
        /// <summary>
        /// Opens the store listing.
        /// </summary>
        /// <param name="appId">App identifier.</param>
        public void OpenStoreListing(string appId) =>
            OpenStoreReviewPage(appId);


        Intent GetRateIntent(string url)
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));

            intent.AddFlags(ActivityFlags.NoHistory);
            intent.AddFlags(ActivityFlags.MultipleTask);
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                intent.AddFlags(ActivityFlags.NewDocument);
            }
            else
            {
                intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            }
            intent.SetFlags(ActivityFlags.ClearTop);
            intent.SetFlags(ActivityFlags.NewTask);
            return intent;
        }

        /// <summary>
        /// Opens the store review page.
        /// </summary>
        /// <param name="appId">App identifier.</param>
        public void OpenStoreReviewPage(string appId)
        {
            var url = $"market://details?id={appId}";
            try
            {
                var intent = GetRateIntent(url);
                Application.AppContext.StartActivity(intent);
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch app store: " + ex.Message);
            }

            url = $"https://play.google.com/store/apps/details?id={appId}";
            try
            {
                var intent = GetRateIntent(url);
                Application.AppContext.StartActivity(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch app store: " + ex.Message);
            }
        }

        IReviewManager manager;
        TaskCompletionSource<bool> tcs;
        /// <summary>
        /// Requests an app review.
        /// </summary>
        public async Task RequestReview(bool testMode)
        {
            tcs?.TrySetCanceled();
            tcs = new TaskCompletionSource<bool>();

            if (testMode)
                manager = new FakeReviewManager(Application.AppContext);
            else
                manager = ReviewManagerFactory.Create(Application.AppContext);

            forceReturn = false;
            var request = manager.RequestReviewFlow();
            request.AddOnCompleteListener(this);
            await tcs.Task;
            manager.Dispose();
            request.Dispose();
        }

        Activity Activity =>
            Xamarin.Essentials.Platform.CurrentActivity ?? throw new NullReferenceException("Current Activity is null, ensure that the MainActivity.cs file is configuring Xamarin.Essentials in your source code so the In App Billing can use it.");

        bool forceReturn;
        Com.Google.Android.Play.Core.Tasks.Task launchTask;
        public void OnComplete(Com.Google.Android.Play.Core.Tasks.Task task)
        {
            //if (!task.IsSuccessful || forceReturn)
            //{
            //    tcs.TrySetResult(forceReturn);
            //    launchTask?.Dispose();
            //    return;
            //}

            try
            {
                var reviewInfo = (ReviewInfo)task.GetResult(Java.Lang.Class.FromType(typeof(ReviewInfo)));
                forceReturn = true;
                launchTask = manager.LaunchReviewFlow(Activity, reviewInfo);
                //launchTask.AddOnCompleteListener(OnNewComplete);
            }
            catch (Exception ex)
            {
                tcs.TrySetResult(false);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        public void OnNewComplete(Com.Google.Android.Play.Core.Tasks.Task task)
        {
        }

        public async Task RateApp()
        {
#if DEBUG
            // FakeReviewManager does not interact with the Play Store, so no UI is shown
            // and no review is performed. Useful for unit tests.
            var manager = new FakeReviewManager(MainActivity.context);
#else         
            var manager = ReviewManagerFactory.Create(MainActivity.context);
#endif            
            var request = manager.RequestReviewFlow();
            request.AddOnCompleteListener(new OnCompleteListener(manager));

        }

        public void RateAppFromStore()
        {

            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            var url = $"market://details?id={(activity as Context)?.PackageName}";

            try
            {
                activity.PackageManager.GetPackageInfo("com.LinkedGates.DelmarAttalla", PackageInfoFlags.Activities);
                Intent intent = new Intent(activity.Intent.Action, Android.Net.Uri.Parse(url));

                activity.StartActivity(intent);
            }
            catch (PackageManager.NameNotFoundException ex)
            {
                // this won't happen. But catching just in case the user has downloaded the app without having Google Play installed.

                Console.WriteLine(ex.Message);
            }
            catch (ActivityNotFoundException)
            {
                // if Google Play fails to load, open the App link on the browser 

                var playStoreUrl = "https://play.google.com/store/apps/details?id=com.yourapplicationpackagename"; //Add here the url of your application on the store

                var browserIntent = new Intent(activity.Intent.Action, Android.Net.Uri.Parse(playStoreUrl));
                browserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);

                activity.StartActivity(browserIntent);
            }

        }
        public class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            FakeReviewManager _fakeReviewManager;
            IReviewManager _reviewManager;
            bool _usesFakeManager;
            void IOnCompleteListener.OnComplete(Com.Google.Android.Play.Core.Tasks.Task p0)
            {

                try
                {
                    LaunchReview(p0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            private void LaunchReview(Com.Google.Android.Play.Core.Tasks.Task p0)
            {

                var review = p0.GetResult(Java.Lang.Class.FromType(typeof(ReviewInfo)));
                if (_usesFakeManager)
                {
                    var x = _fakeReviewManager.LaunchReviewFlow(Xamarin.Essentials.Platform.CurrentActivity, (ReviewInfo)review);
                    x.AddOnCompleteListener(new OnCompleteListener(_fakeReviewManager));
                }
                else
                {
                    var x = _reviewManager.LaunchReviewFlow(MainActivity.Instance, (ReviewInfo)review);
                    x.AddOnCompleteListener(new OnCompleteListener(_reviewManager));
                }

            }

            public OnCompleteListener(FakeReviewManager fakeReviewManager)
            {
                _fakeReviewManager = fakeReviewManager;
                _usesFakeManager = true;
            }
            public OnCompleteListener(IReviewManager reviewManager)
            {
                _reviewManager = reviewManager;
            }
        }
    }
}
