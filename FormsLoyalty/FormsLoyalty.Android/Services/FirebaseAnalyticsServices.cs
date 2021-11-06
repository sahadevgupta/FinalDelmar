using Android.OS;
using FormsLoyalty.Droid.Services;
using FormsLoyalty.Interfaces;
using System.Collections.Generic;
[assembly: Xamarin.Forms.Dependency(typeof(FirebaseAnalyticsServices))]
namespace FormsLoyalty.Droid.Services
{
    public class FirebaseAnalyticsServices : IFirebaseAnalytics
    {
        public void SendEvent(string eventId)
        {
            SendEvent(eventId, null);
        }

        public void SendEvent(string eventId, string paramName, string value)
        {
            SendEvent(eventId, new Dictionary<string, string>
            {
                {paramName, value}
            }); ;
        }

        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            //Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventId, parameters);

            //var firebaseAnalytics = Firebase.Analytics.FirebaseAnalytics.GetInstance(Xamarin.Essentials.Platform.AppContext);

            //if (parameters == null)
            //{
            //    firebaseAnalytics.LogEvent(eventId, null);
            //    return;
            //}

            //var bundle = new Bundle();
            //foreach (var param in parameters)
            //{
            //    bundle.PutString(param.Key, param.Value);
            //}

            // firebaseAnalytics.LogEvent(eventId, bundle);
        }
    }
}