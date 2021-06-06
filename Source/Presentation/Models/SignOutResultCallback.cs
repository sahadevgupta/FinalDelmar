using Android.Gms.Common.Apis;
using Java.Lang;
using Presentation.Activities.Base;
using Presentation.Activities.Login;

namespace Presentation.Models
{
    public class SignOutResultCallback : Object, IResultCallback
    {
        public LoginFragment Activity { get; set; }

        public void OnResult(Object result)
        {
            //Activity.UpdateUI(false);
        }
    }
}