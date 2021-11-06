using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.Models
{
    public class BaseModel
    {
        public async Task<string> HandleUIExceptionAsync(Exception ex, bool displayAlert = true, bool showAsToast = false, bool showToastOnNetworkError = true, bool checkSecurityTokenException = true)
        {
            ex = GetInnerException(ex);

            System.Diagnostics.Debug.WriteLine("Model Error", ex.GetType().ToString());
            System.Diagnostics.Debug.WriteLine("Model Error", ex.ToString());
            System.Diagnostics.Debug.WriteLine("Model Error", ex.Message);
            System.Diagnostics.Debug.WriteLine("Model Error", ex.StackTrace);

            string msg = string.Empty;


            msg = ex.Message;

            if (ex is LSOmniException)
            {
                var lsOmniException = (LSOmniException)ex;

                if (lsOmniException.StatusCode == StatusCode.AuthFailed)
                {
                    msg = AppResources.ResourceManager.GetString("ModelAuthenticationFailed", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.CommunicationFailure)
                {
                    if (ex.Message.Contains("/ucjson.svc/AppSettingsGetByKey"))
                        return msg;

                    showAsToast = true;

                    msg = AppResources.ResourceManager.GetString("ModelNetworkException", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.AccessNotAllowed)
                {
                    msg = AppResources.ResourceManager.GetString("ModelAccessNotAllowed", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.EmailExists)
                {
                    msg = AppResources.ResourceManager.GetString("ModelEmailExists", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.EmailInvalid)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewEmailInvalid", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.UserNameExists)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewUserNameExists", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.UserNameInvalid)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewUserNameInvalid", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.PasswordInvalid)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewPasswordInvalid", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.FacebookIsExist)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewFacebookIsExist", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.invalidFacebook)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewFacebookInvalid", AppResources.Culture);
                    return msg;
                }
                else if (lsOmniException.StatusCode == StatusCode.GoogleIsExist)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewGoogleIsExist", AppResources.Culture);
                }
                else if (lsOmniException.StatusCode == StatusCode.invalidGoogle)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewGoogleInvalid", AppResources.Culture);
                    return msg;

                }
                else if (lsOmniException.StatusCode == StatusCode.InvalidMobileNo)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewMobileInvalid", AppResources.Culture);
                    showAsToast = true;
                }
                else if (lsOmniException.StatusCode == StatusCode.InvalidPhoneNo)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewPhoneInvalid", AppResources.Culture);
                    showAsToast = true;

                }
                else if (lsOmniException.StatusCode == StatusCode.ExistMobileNo)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewExistMobileNo", AppResources.Culture);
                    showAsToast = true;

                }
                else if (lsOmniException.StatusCode == StatusCode.ExistPhoneNo)
                {
                    msg = AppResources.ResourceManager.GetString("AccountViewExistPhoneNo", AppResources.Culture);
                    showAsToast = true;

                }
                else if (lsOmniException.StatusCode == StatusCode.SecurityTokenInvalid || lsOmniException.StatusCode == StatusCode.UserNotLoggedIn)
                {
                    msg = AppResources.ResourceManager.GetString("ModelSecurityToken", AppResources.Culture);

                    if (checkSecurityTokenException)
                    {
                        //displayAlert = false;
                        //var intent = new Intent();
                        //intent.SetClass(Context, typeof(LoginActivity));
                        //intent.PutExtra(BundleConstants.IsInsideApp, true);
                        //Context.StartActivity(intent);
                    }
                }
            }
            // 
            if (displayAlert)
            {
                if (showAsToast)
                {
                   // if (msg != AppResources.ResourceManager.GetString("ModelGenericException", AppResources.Culture))
                        DependencyService.Get<INotify>().ShowToast(msg);
                }
                else
                {
                    DependencyService.Get<INotify>().ShowToast(msg);

                }
            }

            AppData.Msg = msg;
            return msg;
        }

        private Exception GetInnerException(Exception exception)
        {
            if (exception is AggregateException)
            {
                return exception.InnerException;
            }

            return exception;
        }

        protected void ShowToast(string description)
        {

            DependencyService.Get<INotify>().ShowToast(description);
        }

        protected async void ShowSnackBar(string description)
        {
            await MaterialDialog.Instance.SnackbarAsync(message: description,
                                            msDuration: MaterialSnackbar.DurationLong);
            
        }
    }
}
