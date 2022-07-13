using Xamarin.Forms;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using FormsLoyalty.Controls;
using Prism.Navigation;
using System.Threading.Tasks;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Views;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class LoginOtpViewViewModel : ViewModelBase
    {

        private string _mobileNumber;

        public bool FromItemPage { get; private set; }

        public string MobileNumber
        {
            get { return _mobileNumber; }
            set { SetProperty(ref _mobileNumber, value); }
        }


        private bool _isResendEnabled = false;
        public bool IsResendEnabled
        {
            get { return _isResendEnabled; }
            set { SetProperty(ref _isResendEnabled, value); }
        }

        public string Otp { get; private set; }

        private FacebookProfile fbProfile;
        private GoogleProfile googleProfile;
        private int _TimerCount;
        public int TimerCount
        {
            get { return _TimerCount; }
            set
            {
                _TimerCount = value;
                TimeSpan _TimeSpan = TimeSpan.FromSeconds(_TimerCount);
                if (_TimerCount > 0)
                {


                    if (Settings.RTL)
                    {
                        otpComp.OtpTimer = $"تنتهي الصلاحية في {string.Format("{0:00}: {1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds)} دقيقة";
                    }
                    else
                        otpComp.OtpTimer = $"Expires in {string.Format("{0:00}:{1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds)} mins";
                }
                else
                {
                    IsResendEnabled = true;
                    otpComp.OtpTimer = string.Empty;
                    //Otp = string.Empty;
                }
                   


            }
        }

       

        public OtpComponentView otpComp { get; set; }

        

        public DelegateCommand OnVerifyCommand { get; set; }
        public DelegateCommand ResendOtpCommand { get; set; }

        CommonModel _commonModel;
        private System.Timers.Timer _timer;
        public LoginOtpViewViewModel(INavigationService navigationService):base(navigationService)
        {
            OnVerifyCommand = new DelegateCommand(async() => await Login());
            ResendOtpCommand = new DelegateCommand(async () => await ResendOtp());
            _commonModel = new CommonModel();
        }

        private async Task ResendOtp()
        {
            if (IsPageEnabled) return;
            IsPageEnabled = true;

            Otp = await new CommonModel().GenerateOTPAsync(MobileNumber);
            if (!string.IsNullOrEmpty(Otp))
            {
                IsResendEnabled = false;
                StartTimer();
            }
              
            if(!AppData.GetSocialMediaStatusResult && Device.RuntimePlatform == Device.iOS)
                MaterialDialog.Instance.SnackbarAsync($"Your OTP : {Otp}", 5000);

            IsPageEnabled = false;
        }

        public void StartTimer()
        {
            TimerCount = 300;

            _timer = new System.Timers.Timer();
            //Trigger event every second
            _timer.Interval = 1000;
            _timer.Elapsed += OnTimedEvent;
            //count down 5 seconds
           

            _timer.Enabled = true;


            //Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            //{
            //    if (TimerCount == 0)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        TimerCount -= 1;
            //        return true;
            //    }
            //});
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerCount--;

            //Update visual representation here
            //Remember to do it on UI thread

            if (TimerCount == 0)
            {
                _timer.Stop();
            }
        }

        private async Task Login()
        {
            IsPageEnabled = true;
            try
            {
                if (string.IsNullOrEmpty(otpComp.OtpValue))
                {
                    otpComp.IsError = true;
                  
                    IsPageEnabled = false;
                    return;
                }

                if (!string.IsNullOrEmpty(Otp))
                {
                    if (Otp.Equals(otpComp.OtpValue) && TimerCount > 0)
                    {
                        var response = await _commonModel.VerifyPhoneAsync(MobileNumber);
                        if (response==null)
                        {
                            var navParam = new NavigationParameters();
                            navParam.Add("number", MobileNumber);

                            if (fbProfile != null)
                            {
                                navParam.Add("fb", fbProfile);
                            }
                            else if (googleProfile != null)
                            {
                                navParam.Add("google", googleProfile);
                            }
                          
                            await NavigationService.NavigateAsync($"../{nameof(SignUpPage)}", navParam);
                        }
                        else if(!string.IsNullOrEmpty(response.Id))
                        {
                            if (response.Cards.Any())
                            {

                                if(fbProfile != null)
                                {
                                   await Task.Run(async() =>
                                    {
                                        IsPageEnabled = true;
                                        response.FacebookID = fbProfile.Id;
                                        response.UserName = MobileNumber;
                                        response.FirstName = string.IsNullOrEmpty(response.FirstName) ? fbProfile.FirstName : response.FirstName;
                                        response.LastName = string.IsNullOrEmpty(response.LastName) ? fbProfile.LastName : response.LastName ;
                                        response.Email = string.IsNullOrEmpty(response.Email) ? fbProfile.Email : response.Email ;
                                        //AppData.Device.UserLoggedOnToDevice = response;
                                        try
                                        {
                                            await new MemberContactModel().UpdateMemberContact(response);
                                        }
                                        catch (Exception)
                                        {

                                            IsPageEnabled = false;
                                        }
                                        
                                    });
                                    
                                }
                                else if (googleProfile != null)
                                {
                                   await Task.Run(async () =>
                                    {
                                        IsPageEnabled = true;

                                        response.GoogleID = googleProfile.Id;
                                        response.UserName = MobileNumber;
                                        response.FirstName = string.IsNullOrEmpty(response.FirstName) ? googleProfile.FirstName : response.FirstName;
                                        response.LastName = string.IsNullOrEmpty(response.LastName) ? googleProfile.LastName : response.LastName;
                                        response.Email = string.IsNullOrEmpty(response.Email) ? googleProfile.Email : response.Email;
                                        //AppData.Device.UserLoggedOnToDevice = response;
                                        try
                                        {
                                            await new MemberContactModel().UpdateMemberContact(response);
                                        }
                                        catch (Exception)
                                        {

                                            IsPageEnabled = false;
                                        }
                                    });
                                }

                                GoToMainScreen();
                            }
                            else
                                await NavigationService.NavigateAsync($"../{nameof(SignUpPage)}", new NavigationParameters { { "edit", true } });
                        } 
                        else
                            await App.dialogService.DisplayAlertAsync("Error!!", AppData.Msg, "OK");
                    }
                    else
                        await App.dialogService.DisplayAlertAsync("Error!!", AppResources.txtInvalidOtp, "OK");

                   
                    
                }
                else
                {
                     Otp = await _commonModel.GenerateOTPAsync(MobileNumber);
                    //DependencyService.Get<INotify>().ShowSnackBar($"Your OTP : {Otp}");
                }
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }

            IsPageEnabled = false;
        }
        private async void GoToMainScreen()
        {
            AppData.IsLoggedIn = true;
            Xamarin.Essentials.Preferences.Set("IsLoggedIn", true);
            IsPageEnabled = true;
            SendFCMTokenToServer();
            if (FromItemPage)
            {
                MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "LoggedIn");

                await NavigationService.NavigateAsync("../../");
            }
            else
            {

                MessagingCenter.Send(new BasketModel(), "CartUpdated");
                await NavigationService.NavigateAsync("../../");
                
                //App.Current.MainPage = new NavigationPage(new MainTabbedPage());
            }
                //await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");

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


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            FromItemPage = parameters.GetValue<bool>("itemPage");
            MobileNumber = parameters.GetValue<string>("number");
            Otp = parameters.GetValue<string>("otp");
            Console.WriteLine("OTP :" + Otp);
            fbProfile = parameters.GetValue<FacebookProfile>("fb");
            googleProfile = parameters.GetValue<GoogleProfile>("google");
        }
    }
}
