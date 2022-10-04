using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using NavigationParameters = Prism.Navigation.NavigationParameters;

namespace FormsLoyalty.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
        private MemberContact _memberContact = new MemberContact();
        public MemberContact memberContact
        {
            get { return _memberContact; }
            set { SetProperty(ref _memberContact, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private bool _editAccount;
        public bool editAccount
        {
            get { return _editAccount; }
            set { SetProperty(ref _editAccount, value); }
        }

        private Address _address;
        public Address Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }



        #region Updated Address Section
        private ObservableCollection<AreaModel> _areas;
        public ObservableCollection<AreaModel> Areas
        {
            get { return _areas; }
            set { SetProperty(ref _areas, value); }
        }

        private AreaModel _selectedArea;
        public AreaModel SelectedArea
        {
            get { return _selectedArea; }
            set 
            {
               
               
                    SetProperty(ref _selectedArea, value);
                    if (_selectedArea != null)
                        Address.Area = _selectedArea.Area;
              
               
            }
        }

        private bool _isMobileNumberEnabled;
        public bool IsMobileNumberExist
        {
            get { return _isMobileNumberEnabled; }
            set { SetProperty(ref _isMobileNumberEnabled, value); }
        }

        private string _mobileNumber;
        public string MobileNumber
        {
            get { return _mobileNumber; }
            set { SetProperty(ref _mobileNumber, value); }
        }


        private ObservableCollection<CitiesModel> _cities;
        public ObservableCollection<CitiesModel> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        private CitiesModel _selectedCity;
        public CitiesModel SelectedCity
        {
            get { return _selectedCity; }
            set 
            {
                SetProperty(ref _selectedCity, value);
                
                if (value!=null)
                {

                    LoadAreas();
                    Address.City = value.City;
                    Address.Country = value.Country;
                }
            }
        }

        private void LoadAreas()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Areas = new ObservableCollection<AreaModel>(await new CommonModel().GetAreasAsync(SelectedCity.City));
                if (editAccount && memberContact.Addresses.Any())
                {
                   SelectedArea = Areas.FirstOrDefault(x => x.Area.Equals(memberContact.Addresses[0].Area, StringComparison.OrdinalIgnoreCase));

                }

            });
        }

        #endregion

        public bool FromItemPage { get; private set; }

        readonly MemberContactModel model;
        public SignUpPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            model = new MemberContactModel();
            Address = new Address();
            Title = AppResources.ResourceManager.GetString("SignUpTxt",CultureInfo.InvariantCulture);
           
        }

        private void LoadAddressData()
        {
            Task.Run(async() =>
            {
               
                Cities = new ObservableCollection<CitiesModel>(await new CommonModel().GetCitiessync());
               
                if (editAccount)
                {
                    SelectedCity = Cities.FirstOrDefault(x => x.City.Equals(memberContact.Addresses[0].City, StringComparison.OrdinalIgnoreCase));

                }
                
            });
        }

        private async Task CreateUpdateMemberContact()
        {
            IsPageEnabled = true;

            if (memberContact.Cards.Count <= 0)
            {
                memberContact.Cards.Add(new Card()
                {
                    Id = AppData.Device.CardId
                });
            }

            if (editAccount)
            {
                if (MemberContactAttributes.Manage.Username)
                {
                    memberContact.UserName = AppData.Device.UserLoggedOnToDevice.UserName;
                }
                else
                {
                    memberContact.UserName = memberContact.Email;
                }

                memberContact.Password = AppData.Device.UserLoggedOnToDevice.Password;



                try
                {
                    var success = await model.UpdateMemberContact(memberContact);

                    if (success)
                    {
                        await NavigationService.GoBackAsync(new NavigationParameters { { "updated", true } });

                        SendFCMTokenToServer();

                        await NavigationService.NavigateAsync("../../");
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    await UpdateMemberContactError(ex);
                }
            }
            else
            {
                // sign up
                if (!MemberContactAttributes.Manage.Username)
                    memberContact.UserName = memberContact.Email;
                
                try
                {

                    var success = await model.CreateMemberContact(memberContact);

                    if (success)
                    {
                        Xamarin.Essentials.Preferences.Set("IsLoggedIn", true);
                        if (FromItemPage)
                        {
                            AppData.IsLoggedIn = true;
                            

                            MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "LoggedIn");
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await NavigationService.NavigateAsync("../../");
                            });

                        }
                        else
                        {
                            MessagingCenter.Send(new BasketModel(), "CartUpdated");
                            await NavigationService.NavigateAsync("../../");
                            
                           
                        }
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async() =>
                        {
                           await App.dialogService.DisplayAlertAsync("Error!!", AppData.Msg, "OK");
                        });
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    IsPageEnabled = false;
                    await CreateMemberContactError(ex);
                }
            }
            IsPageEnabled = false;
        }

        /// <summary>
        /// this method helps to registered the device with the server to receive push notification.
        /// It used 2 object.
        /// </summary>
        /// <param name="fcm token">fcm unique token </param>
        /// <param name="deviceId">your device id</param>
        private void SendFCMTokenToServer()
        {
            if (!Settings.KEY_SYNC_FCMTOKEN)
            {
                Task.Run(async () =>
                {
                    var membermodel = new MemberContactModel();
                    var isSuccess = await membermodel.SendFCMTokenToServer(Settings.FCM_Token, AppData.Device.Id);
                    Settings.KEY_SYNC_FCMTOKEN = isSuccess;
                });
            }

        }

        private async Task UpdateMemberContactError(Exception ex)
        {

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email Invaild", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email already Exists", "OK");
            }


        }

        private async Task CreateMemberContactError(Exception ex)
        {

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameInvalid || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameExists)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Username Invaild or Username already exists", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.PasswordInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Password Invaild", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email Invaild or Email already exists", "OK");
            }


        }
        /// <summary>
        /// Assign data to Member contact
        /// As agreed Setting password and email value has static after new login business logic changed (OTP )
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        internal async Task AssignToMemberContact(string username)
        {

            memberContact.Password = "1234567890";
            memberContact.Email = $"{Guid.NewGuid().ToString()}@test.com";
            memberContact.FirstName = FirstName;
            memberContact.LastName = LastName;
            memberContact.Name = $"{FirstName} {LastName}";

            if (memberContact.Addresses.Any())
            {
                memberContact.Addresses.Remove(memberContact.Addresses.First(x => x.LineNO == Address.LineNO));
                memberContact.Addresses.Add(Address);
            }
            else
            {
                memberContact.Addresses = new List<Address> { Address };
            }

            memberContact.LoggedOnToDevice = AppData.Device;
            memberContact.MobilePhone = MobileNumber;
            memberContact.UserName = MobileNumber;
            memberContact.Phone = MobileNumber;

            await CreateUpdateMemberContact();
           
        }

       
        internal void FetchExistingMemberContact()
        {
            Title = AppResources.MenuViewAccountManagementTitle;
            memberContact = AppData.Device.UserLoggedOnToDevice;

            if(memberContact.Addresses.Any())
                Address = memberContact.Addresses[0];

            UserName = memberContact.UserName;
            FirstName = memberContact.FirstName;
            LastName = memberContact.LastName;
            MobileNumber = memberContact.MobilePhone;



        }

        internal async Task GoBack()
        {
            await NavigationService.GoBackAsync();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue<FacebookProfile>("fb",out FacebookProfile fbProfile) )
            {
                memberContact.Name = $"{fbProfile.FirstName} {fbProfile.LastName}";
                memberContact.FirstName = FirstName = fbProfile.FirstName;
                memberContact.LastName = LastName = fbProfile.LastName;
                memberContact.FacebookID = fbProfile.Id;
                memberContact.Email = fbProfile.Email;
                UserName = $"{fbProfile.FirstName}{fbProfile.LastName}";
            }
            else if(parameters.TryGetValue("google", out GoogleProfile googleProfile))
            {
                memberContact.Name = $"{googleProfile.FirstName} {googleProfile.LastName}";
                memberContact.FirstName = FirstName = googleProfile.FirstName;
                memberContact.LastName = LastName = googleProfile.LastName;
                memberContact.GoogleID = googleProfile.Id;
                memberContact.Email = googleProfile.Email;
                UserName = $"{googleProfile.FirstName}{googleProfile.LastName}";
            }
         

        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            editAccount = parameters.GetValue<bool>("edit");
            FromItemPage = parameters.GetValue<bool>("itemPage");
            var MobilePhone = parameters.GetValue<string>("number");
            if (!string.IsNullOrEmpty(MobilePhone))
            {
                IsMobileNumberExist = true;
                MobileNumber = MobilePhone;
            }
            if (editAccount)
            {
                FetchExistingMemberContact();

            }
            LoadAddressData();
            

        }




    }
}
