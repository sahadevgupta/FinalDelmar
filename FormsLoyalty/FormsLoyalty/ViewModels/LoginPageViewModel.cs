using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.FacebookClient;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _username;
        public string userName
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _mobileNumber;
        public string MobileNumber
        {
            get { return _mobileNumber; }
            set { SetProperty(ref _mobileNumber, value); }
        }

        private string _password;
        public string password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _errorText;
        public string ErrorText
        {
            get { return _errorText; }
            set { SetProperty(ref _errorText, value); }
        }

        private bool _IsError;
        public bool IsError
        {
            get { return _IsError; }
            set { SetProperty(ref _IsError, value); }
        }


        private bool _isSocialMediaLoginVisible;
        public bool IsSocialMediaLoginVisible
        {
            get { return _isSocialMediaLoginVisible; }
            set { SetProperty(ref _isSocialMediaLoginVisible, value); }
        }
        public FacebookProfile fbProfile { get; set; }
        public GoogleProfile GoogleProfile { get; set; }

        bool isFbLogin = false;

        public DelegateCommand<string> OnSocialLoginCommand { get; set; }
        public DelegateCommand onLoginCommand { get; set; }

        public DelegateCommand OnOtpCommand { get; set; }
        public bool FromItemPage { get; private set; }

        string[] permisions = new string[] { "email", "public_profile", "user_posts" };
        MemberContactModel memberContactModel;

       
        IGoogleClientManager _googleService = CrossGoogleClient.Current;
        IFacebookClient _facebookService = CrossFacebookClient.Current;
        public LoginPageViewModel(INavigationService navigationService):base(navigationService)
        {
            OnSocialLoginCommand = new DelegateCommand<string>(async (data) => await SocialMediaLogin(data));
            onLoginCommand = new DelegateCommand(async () => { await Login(); }) ;
            OnOtpCommand = new DelegateCommand(async()=> { await NavigateToOtpView(); });
            memberContactModel = new MemberContactModel();

            IsSocialMediaLoginVisible = Device.RuntimePlatform == Device.iOS ? AppData.GetSocialMediaStatusResult : true;
        }

        private async Task NavigateToOtpView()
        {
            IsPageEnabled = true;
            if (string.IsNullOrEmpty(MobileNumber))
            {
                ErrorText = "Mobile Number cannot be empty";
                IsError = true;
                IsPageEnabled = false;
                return;
            }

            if (MobileNumber.Length <= 11)
            {
                var _digit = MobileNumber.Substring(0, 3);
                if (!(_digit.Equals("011") || _digit.Equals("012") || _digit.Equals("015") || _digit.Equals("010")))
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));

                  //  ErrorText = "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012";
                   // IsError = true;
                    IsPageEnabled = false;
                    return;
                }

            }
            try
            {
                var Otp = await new CommonModel().GenerateOTPAsync(MobileNumber);
                await NavigationService.NavigateAsync(nameof(LoginOtpView), new NavigationParameters { { "number", MobileNumber },{"otp", Otp },{ "itemPage", FromItemPage } }, false, true);

                if (!AppData.GetSocialMediaStatusResult && Device.RuntimePlatform == Device.iOS)
                    MaterialDialog.Instance.SnackbarAsync($"Your OTP : {Otp}", 5000);

#if DEBUG
                MaterialDialog.Instance.SnackbarAsync($"Your OTP : {Otp}", 5000);
#endif

                //if (Device.RuntimePlatform == Device.Android)
                //{
                //    DependencyService.Get<INotify>().ShowSnackBar($"Your OTP : {Otp}");
                //}
                //else
                //  MaterialDialog.Instance.SnackbarAsync($"Your OTP : {Otp}", 5000);


            }
            catch (Exception ex)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    DependencyService.Get<INotify>().ShowSnackBar(ex.Message);
                }
                else
                {
                    MaterialDialog.Instance.SnackbarAsync(ex.Message, 5000);
                }
                
            }
            finally
            {
                IsPageEnabled = false;
            }
            
        }

        private async Task Login()
        {
            IsPageEnabled = true;
            try
            {
                if (string.IsNullOrEmpty(userName)|| string.IsNullOrEmpty(password))
                {
                    IsError = true;
                    IsPageEnabled = false;
                    return;
                }

                var success = await memberContactModel.Login(userName, password, (s) => {  });
                if (success)
                { 
                    GoToMainScreen();
                }
                else
                  await  App.dialogService.DisplayAlertAsync("Error!!", AppData.Msg, "OK");
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
           
            IsPageEnabled = false;
        }

        private async Task SocialMediaLogin(string obj)
        {
            IsPageEnabled = true;
            if (obj.Equals("Facebook"))
            {
                isFbLogin = true;
                await FBLoginAsync();
            }
            else
            {
                isFbLogin = false;
                await LoginGoogleAsync();
            }
               

            IsPageEnabled = false;
        }

        private async Task LoginGoogleAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_googleService.AccessToken))
                {
                    //Always require user authentication
                    _googleService.Logout();
                }

                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = async (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    switch (e.Status)
                    {
                        case GoogleActionStatus.Completed:
#if DEBUG
                            var googleUserString = JsonConvert.SerializeObject(e.Data);
                            Debug.WriteLine($"Google Logged in succesfully: {googleUserString}");
#endif

                            var id = e.Data.Id;
                            var picture = e.Data.Picture.AbsoluteUri;
                            var name = e.Data.Name;

                            GoogleProfile = new GoogleProfile
                            {
                                Id = e.Data.Id,
                                Email = e.Data.Email,
                                FirstName = e.Data.GivenName,
                                LastName = e.Data.FamilyName
                            };

                            await GoogleLogon();
                            //await App.Current.MainPage.Navigation.PushModalAsync(new HomePage(socialLoginData));
                            break;
                        case GoogleActionStatus.Canceled:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Canceled", "Ok");
                            break;
                        case GoogleActionStatus.Error:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Error", "Ok");
                            break;
                        case GoogleActionStatus.Unauthorized:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Unauthorized", "Ok");
                            break;
                    }

                    _googleService.OnLogin -= userLoginDelegate;
                };

                _googleService.OnLogin += userLoginDelegate;

                await _googleService.LoginAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                //await MaterialDialog.Instance.SnackbarAsync(message: ex.ToString(),
                //                           msDuration: MaterialSnackbar.DurationLong);
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task FBLoginAsync()
        {
            IsPageEnabled = true;

            if (_facebookService.IsLoggedIn)
            {
                //DependencyService.Get<IFacebookServices>().LogOut();
                _facebookService.Logout();
                CrossFacebookClient.Current.Logout();

            }

            var loginData = await CrossFacebookClient.Current.LoginAsync(new string[] { "email" });
            if (loginData.Status == FacebookActionStatus.Completed)
            {
                var response = await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "id", "email", "first_name", "last_name", "picture" }, new string[] { "email" });
                switch (response.Status)
                {
                    case FacebookActionStatus.Completed:
                        fbProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookProfile>(response.Data));
                        await FacebookLogon();
                        break;
                    case FacebookActionStatus.Canceled:

                        break;
                    case FacebookActionStatus.Unauthorized:
                        await App.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                        break;
                    case FacebookActionStatus.Error:
                        await App.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                        break;
                }
            }
            else
            {
                await MaterialDialog.Instance.SnackbarAsync(message: loginData.Message,
                                           msDuration: MaterialSnackbar.DurationLong);
                Debug.WriteLine(loginData.Message);
            }


            //FacebookResponse<string> response = await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "gender", "last_name", "birthday" }, new string[] { "email", "user_birthday" });
            

            
            IsPageEnabled = false;
        }

        private async Task FacebookLogon()
        {
            try
            {
                var success = await memberContactModel.FacbookLoginAsync(fbProfile.Id, fbProfile.Email, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }
            catch (Exception)
            {
                
            }
        }
        private async Task GoogleLogon()
        {

            try
            {
                var success = await memberContactModel.GoogleLogin(GoogleProfile.Id, GoogleProfile.Email, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }
            catch (Exception)
            {

            }

        }

        private async void GoToMainScreen()
        {
            IsPageEnabled = true;
            SendFCMTokenToServer();
            if (FromItemPage)
            {
                MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "LoggedIn");

               await NavigationService.GoBackAsync();
            }
            else
              await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
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
        private async void ShowError(string errorMessage)
        {

            IsPageEnabled = true;
            if (fbProfile != null && isFbLogin)
            {
                await NavigationService.NavigateAsync(nameof(FormsLoyalty.Views.SocialMediaLogin), new NavigationParameters { { "fb", fbProfile } });
            }
            else if(GoogleProfile !=null)
            {
                await NavigationService.NavigateAsync(nameof(FormsLoyalty.Views.SocialMediaLogin), new NavigationParameters { { "google", GoogleProfile } });
            }


            IsPageEnabled = false;

        }

       

        

      

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            FromItemPage = parameters.GetValue<bool>("itemPage");
        }
    }

    public class FacebookProfile
    {
        public string Email { get; set; }
        public string Id { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
    }
    public class GoogleProfile
    {
        public string Email { get; set; }
        public string Id { get; set; }

       
        public string LastName { get; set; }
        
        public string FirstName { get; set; }
    }
}
