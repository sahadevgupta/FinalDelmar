using Android.Gms.Auth.Api.Phone;
using Android.Gms.Tasks;
using FormsLoyalty.Droid;
using FormsLoyalty.Interfaces;
using Java.Lang;

using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ListenToSms))]
namespace FormsLoyalty.Droid
{
    public class ListenToSms : IListenToSmsRetriever
    {
        public void ListenToSmsRetriever()
        {

            SmsRetrieverClient client = SmsRetriever.GetClient(Application.Context);
            Task task = client.StartSmsRetriever();
            task.AddOnSuccessListener(new SuccessListener());
            task.AddOnFailureListener(new FailureListener());
        }
        private class SuccessListener : Object, IOnSuccessListener
        {
            public void OnSuccess(Object result)
            {
            }
        }
        private class FailureListener : Object, IOnFailureListener
        {
            public void OnFailure(Exception e)
            {
            }
        }
    }
}