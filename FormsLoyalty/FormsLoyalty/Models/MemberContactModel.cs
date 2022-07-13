using Infrastructure.Data.SQLite.Devices;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Prism;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.Ioc;
using System.Threading.Tasks;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
using Xamarin.Forms;
using FormsLoyalty.Interfaces;
using XF.Material.Forms.UI.Dialogs;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Microsoft.AppCenter.Crashes;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Infrastructure.Data.SQLite.MemberContacts;
using Device = LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device;
using System.Linq;

namespace FormsLoyalty.Models
{
    public class MemberContactModel : BaseModel
    {
        private MemberContactService service;
        private IDeviceLocalRepository deviceRepo;
        private MemberRepository repository;
        private MemberContactLocalService memberContactLocalService;
        private SharedService sharedService;

        public async Task RegisterDevice()
        {

            BeginWsCall();
            //ProgressDialog.Title = Context.GetString(Resource.String.MemberContactModelRegisterDevice);
            //ProgressDialog.Message = Context.GetString(Resource.String.MemberContactModelRegisteringDevice);
            //ProgressDialog.Show();

            var device = AppData.Device;
            if (string.IsNullOrEmpty(device.Manufacturer) || string.IsNullOrEmpty(device.Platform) || string.IsNullOrEmpty(device.OsVersion) || string.IsNullOrEmpty(device.Model))
                FormsLoyalty.Utils.Utils.FillDeviceInfo(device);

            try
            {
                await service.DeviceSaveAsync(repository, device.Id, device.DeviceFriendlyName, device.Platform, device.OsVersion, device.Manufacturer, device.Model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }


        }


      

        public async Task<bool> Logout()
        {
            BeginWsCall();
            
           var isSuccess =  await LogoutOnServer(AppData.Device);
            if (isSuccess)
            {
                AppData.IsLoggedIn = false;
                Xamarin.Essentials.Preferences.Set("IsLoggedIn", false);
                AppData.Basket.Clear();
                AppData.Device.UserLoggedOnToDevice = null;
                AppData.Device.CardId = string.Empty;
                AppData.Device.SecurityToken = "";
                deviceRepo.SaveDevice(AppData.Device);
            }
            return isSuccess;


        }

        private async Task<bool> LogoutOnServer(LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device device)
        {
            BeginWsCall();

            try
            {
                await service.LogoutAsync(repository, device.UserLoggedOnToDevice.UserName, device.Id);
                var contactRepo = new MemberContactRepository();
                contactRepo.DeleteMemberContact();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        public async Task<bool> Login(string username, string password, Action<string> onError)
        {
            bool success = false;

            BeginWsCall();

            var loading = await MaterialDialog.Instance.LoadingDialogAsync(message: string.Format(AppResources.ResourceManager.GetString("MemberContactModelLoggingInUser", AppResources.Culture), username));


            //ProgressDialog.Title = Context.GetString(Resource.String.MemberContactModelLogin);
            //ProgressDialog.Message = string.Format(Context.GetString(Resource.String.MemberContactModelLoggingInUser), username);
            //ProgressDialog.Show();

            try
            {
                var contact = await service.MemberContactLogonAsync(repository, username, password, AppData.Device.Id);
                if (contact == null)
                {
                    success = false;
                    return success;
                }
                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Device.UserLoggedOnToDevice.Cards = contact.Cards;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                AppData.Device.CardId = contact.Cards[0].Id;
                AppData.Basket = contact.GetBasket(AppData.Device.CardId);

                SaveLocalMemberContact(contact);

                deviceRepo.SaveDevice(AppData.Device);

               // await PushNotificationSave();  need to check

                if (string.IsNullOrEmpty(contact.LoggedOnToDevice.Manufacturer) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Platform) || string.IsNullOrEmpty(contact.LoggedOnToDevice.OsVersion) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Model))
                {
                    await RegisterDevice();
                }

                success = true;
            }
            catch (Exception ex)
            {
                var errorMessage = await HandleUIExceptionAsync(ex, showToastOnNetworkError: false, displayAlert: false);

                onError(errorMessage);
                await loading.DismissAsync();
                return success;
            }

            await loading.DismissAsync();
            return success;
        }

        public async Task<bool> FacbookLoginAsync(string FacebookID, string FacebookEmail, Action<string> onError)
        {
           
            bool success = false;

            BeginWsCall();


            var loading = await MaterialDialog.Instance.LoadingDialogAsync(message: string.Format(AppResources.ResourceManager.GetString("MemberContactModelLoggingInUser", AppResources.Culture), FacebookEmail));

            ////ProgressDialog.Title = Context.GetString(Resource.String.MemberContactModelLogin);
            ////ProgressDialog.Message = string.Format(Context.GetString(Resource.String.MemberContactModelLoggingInUser), FacebookEmail);
            ////ProgressDialog.Show();

            try
            {
                var contact = service.MemberContactLogonWithFacebook(repository, FacebookID, FacebookEmail, AppData.Device.Id);
                if (contact == null)
                {
                    success = false;
                    return success;
                }
                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Device.UserLoggedOnToDevice.Cards = contact.Cards;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                AppData.Device.CardId = contact.Cards[0].Id;
                AppData.Basket = contact.GetBasket(AppData.Device.CardId);

                SaveLocalMemberContact(contact);

                deviceRepo.SaveDevice(AppData.Device);

               // await PushNotificationSave();

                if (string.IsNullOrEmpty(contact.LoggedOnToDevice.Manufacturer) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Platform) || string.IsNullOrEmpty(contact.LoggedOnToDevice.OsVersion) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Model))
                {
                    await RegisterDevice();
                }

                success = true;
            }
            catch (Exception ex)
            {
                var errorMessage = await HandleUIExceptionAsync(ex, showToastOnNetworkError: false, displayAlert: false);

                onError(errorMessage);
                await loading.DismissAsync();
                return success;
            }

           await loading.DismissAsync();

            return success;
        }
        public async Task<bool> GoogleLogin(string GoogleID, string GoogleEmail, Action<string> onError)
        {
            bool success = false;

            BeginWsCall();



            try
            {
                var contact = service.MemberContactLogonWithGoogle(repository, GoogleID, GoogleEmail, AppData.Device.Id);
                if (contact == null)
                {
                    success = false;
                    return success;
                }
                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Device.UserLoggedOnToDevice.Cards = contact.Cards;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                AppData.Device.CardId = contact.Cards[0].Id;
                AppData.Basket = contact.GetBasket(AppData.Device.CardId);

                SaveLocalMemberContact(contact);

                deviceRepo.SaveDevice(AppData.Device);

                //await PushNotificationSave();

                if (string.IsNullOrEmpty(contact.LoggedOnToDevice.Manufacturer) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Platform) || string.IsNullOrEmpty(contact.LoggedOnToDevice.OsVersion) || string.IsNullOrEmpty(contact.LoggedOnToDevice.Model))
                {
                    await RegisterDevice();
                }

                success = true;
            }
            catch (Exception ex)
            {
                var errorMessage = await HandleUIExceptionAsync(ex, showToastOnNetworkError: false, displayAlert: false);

                onError(errorMessage);
            }

           // ProgressDialog.Dismiss();

            return success;
        }

        public async Task<string> ForgotPasswordForDeviceAsync(string userName)
        {
            string success = string.Empty;


            BeginWsCall();

            try
            {
                success = await service.ForgotPasswordAsync(repository, userName);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }


            return success;
        }

        public async Task<bool> ResetPassword(string userName, string resetCode, string newPassword)
        {
            bool success = false;

            BeginWsCall();

            try
            {
                success = await service.ResetPasswordAsync(repository, userName, resetCode, newPassword);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }


            return success;
        }

        public async Task UserGetByCardId(string cardId, int retryCounter = 3)
        {

            BeginWsCall();

            try
            {
                MemberContact contact = await service.MemberContactByCardIdAsync(repository, cardId);
                //contact.GetBasket(AppData.Device.CardId).CalculateBasket();

                AppData.Device.UserLoggedOnToDevice = contact;
                //AppData.Basket = contact.GetBasket(AppData.Device.CardId);
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;

                if (contact.Cards.Any())
                    AppData.Device.CardId = contact.Cards[0].Id;

                //AppData.Device.CardId = 
                var uuid = Xamarin.Forms.DependencyService.Get<INotify>().getDeviceUuid();
                Device device = new Device();
                device.Id = uuid;
                device.CardId = AppData.Device.CardId;
                FormsLoyalty.Utils.Utils.FillDeviceInfo(device);
                contact.LoggedOnToDevice = device;


                SaveLocalMemberContact(contact);

                retryCounter = 0;
            }
            catch (Exception ex)
            {
                if (retryCounter == 0)
                {
                    await HandleUIExceptionAsync(ex, showAsToast: true);
                }
                else
                {
                    await UserGetByCardId(cardId, --retryCounter);
                }
            }

        }
        public async Task<bool> CreateMemberContact(MemberContact newContact)
        {
            BeginWsCall();

            //ProgressDialog.Title = Context.GetString(Resource.String.AccountViewCreateAccount);
            //ProgressDialog.Message = Context.GetString(Resource.String.AccountViewCreatingNewAccount);
            //ProgressDialog.Show();

            try
            {
                
                var contact = await service.CreateMemberContactAsync(repository, newContact);
                if (contact == null)
                {
                    return false;
                }
                contact.LoggedOnToDevice.UserLoggedOnToDevice = contact;
                AppData.Device = contact.LoggedOnToDevice;

                if (contact.Cards.Any())
                    AppData.Device.CardId = contact.Cards[0].Id;

                //AppData.Device.CardId = 
                var uuid = Xamarin.Forms.DependencyService.Get<INotify>().getDeviceUuid();
                Device device = new Device();
                device.Id = uuid;
                device.CardId = AppData.Device.CardId;
                FormsLoyalty.Utils.Utils.FillDeviceInfo(device);
                contact.LoggedOnToDevice = device;
                SaveLocalMemberContact(contact);

               // await PushNotificationSave();
            }
            catch(Exception ex)
            {
                await HandleUIExceptionAsync(ex);
                return false;
                
            }
            finally
            {
               // ProgressDialog.Dismiss();
            }
            return true;
        }

        public async Task<bool> UpdateMemberContact(MemberContact updateContact)
        {
            BeginWsCall();

            //ProgressDialog.Title = Context.GetString(Resource.String.AccountViewUpdateAccount);
            //ProgressDialog.Message = Context.GetString(Resource.String.AccountViewUpdatingExistingAccount);
            //ProgressDialog.Show();

            try
            {
                var contact = await service.UpdateMemberContactAsync(repository, updateContact);
                if (contact !=null)
                {
                    contact.LoggedOnToDevice.UserLoggedOnToDevice = updateContact;

                    AppData.Device = updateContact.LoggedOnToDevice;
                    AppData.Device.UserLoggedOnToDevice = updateContact;
                    AppData.Device.UserLoggedOnToDevice.Cards = updateContact.Cards;
                    AppData.Device.SecurityToken = updateContact.LoggedOnToDevice.SecurityToken;
                    AppData.Device.CardId = updateContact.Cards[0].Id;
                    AppData.Basket = updateContact.GetBasket(AppData.Device.CardId);

                    SaveLocalMemberContact(updateContact);

                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("AccountViewUpdateSuccess", AppResources.Culture));
                    return true;
                }
                //MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "MemberContactUpdated");
                
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            finally
            {
              //  ProgressDialog.Dismiss();
            }

            return false;
        }

        public async Task MemberContactGetPointBalance(string cardId)
        {
            BeginWsCall();

            try
            {
                var points = await service.MemberContactGetPointBalanceAsync(repository, cardId);

                AppData.Device.UserLoggedOnToDevice.Account.PointBalance = points;

               // SendBroadcast(Utils.BroadcastUtils.PointsUpdated);

                SaveLocalMemberContact(AppData.Device.UserLoggedOnToDevice);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
        }

        public async Task<bool> ChangePassword(string username, string newPass, string oldPass)
        {
            bool success = false;

            BeginWsCall();

            //ProgressDialog.Title = Context.GetString(Resource.String.ChangePasswordChangePassword);
            //ProgressDialog.Message = Context.GetString(Resource.String.ChangePasswordChangingPassword);
            //ProgressDialog.Show();

            try
            {
                success = await service.ChangePasswordAsync(repository, username, newPass, oldPass);
                if (success)
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("ChangePasswordChangePasswordSuccess",AppResources.Culture));
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            //ProgressDialog.Dismiss();

            return success;
        }

        public void PushNotificationSave(string gcmId, PushStatus status = PushStatus.Enabled)
        {
            //called from background context

            BeginWsCall();

            //var looper = Context.MainLooper;
            //var handler = new Handler(looper);
            //handler.Post(
            //    async () =>
            //    {
            //        var success = await sharedService.PushNotificationSaveAsync(
            //            new PushNotificationRequest()
            //            {
            //                Application = PushApplication.Loyalty,
            //                DeviceId = new DeviceUuidFactory(Context).getDeviceUuid(),
            //                Id = gcmId,
            //                Platform = PushPlatform.Android,
            //                Status = status
            //            });

            //        if (success)
            //        {
            //            if (status == PushStatus.Enabled)
            //            {
            //                var versionCode = Context.PackageManager.GetPackageInfo(Context.PackageName, 0).VersionCode;

            //                Utils.PreferenceUtils.SetInt(Context, Utils.PreferenceUtils.VersionCode, versionCode);
            //                Utils.PreferenceUtils.SetString(Context, Utils.PreferenceUtils.FcmRegistrationId, gcmId);
            //            }
            //        }
            //    });
        }
        //public async Task PushNotificationSave(PushStatus status = PushStatus.Enabled)
        //{
        //    var token = Utils.PreferenceUtils.GetString(Context, Utils.PreferenceUtils.FcmRegistrationId);
        //    if (string.IsNullOrEmpty(token))
        //        return;

        //    BeginWsCall();

        //    try
        //    {
        //        await sharedService.PushNotificationSaveAsync(
        //            new PushNotificationRequest()
        //            {
        //                Application = PushApplication.Loyalty,
        //                DeviceId = new DeviceUuidFactory(Context).getDeviceUuid(),
        //                Id = token,
        //                Platform = PushPlatform.Android,
        //                Status = status
        //            });
        //    }
        //    catch (Exception)
        //    {
        //        //supress
        //    }
        //}


       

        private Exception GetInnerException(Exception exception)
        {
            if (exception is AggregateException)
            {
                return exception.InnerException;
            }

            return exception;
        }
        public void SaveLocalMemberContact(MemberContact contact)
        {
           var contactRepo =  new MemberContactRepository();

            contactRepo.SaveMemberContact(contact);
        }
        public async Task<int> AddNewAddressAsync(string CardId, Address NewAddress)
        {
            BeginWsCall();
            var a =  await service.AddNewAddressAsync(repository, CardId, NewAddress);
            return a;
        }

        public async Task<bool> SendFCMTokenToServer(string fcmToken, string deviceId)
        {
            BeginWsCall();
            var a = await service.SendFCMTokenAsync(repository, fcmToken, deviceId);
            return a;
        }

        private void BeginWsCall()
        {
            service = new MemberContactService();
            repository = new MemberRepository();
            deviceRepo = new DeviceRepository();
            sharedService = new SharedService(new SharedRepository());
        }
    }
}
