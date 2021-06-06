using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Java.Lang;

using Presentation.Activities.Login;

namespace Presentation.Models
{
    public class SignInResultCallback : Object, IResultCallback
    {
        public LoginFragment Activity { get; set; }

        public void OnResult(Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
            Activity.HideProgressDialog();
            Activity.HandleSignInResult(googleSignInResult);
        }


    }
}