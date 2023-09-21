using System;
using System.Linq;
using System.Threading.Tasks;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using Infrastructure.Data.SQLite.Devices;
using Infrastructure.Data.SQLite.MemberContacts;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using Device = LSRetail.Omni.Domain.DataModel.Loyalty.Setup.Device;

namespace FormsLoyalty.Models
{
    public class MemberContactModel : BaseModel
    {
        private MemberContactService service;
        private IDeviceLocalRepository deviceRepo;
        private MemberRepository repository;

        public async Task RegisterDevice()
        {

            BeginWsCall();

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

                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;

                if (contact.Cards.Any())
                    AppData.Device.CardId = contact.Cards[0].Id;

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

                var uuid = Xamarin.Forms.DependencyService.Get<INotify>().getDeviceUuid();
                Device device = new Device();
                device.Id = uuid;
                device.CardId = AppData.Device.CardId;
                FormsLoyalty.Utils.Utils.FillDeviceInfo(device);
                contact.LoggedOnToDevice = device;
                SaveLocalMemberContact(contact);

            }
            catch(Exception ex)
            {
                await HandleUIExceptionAsync(ex);
                return false;
                
            }
            
            return true;
        }

        public async Task<bool> UpdateMemberContact(MemberContact updateContact)
        {
            BeginWsCall();

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
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
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


            return success;
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
        }
    }
}
