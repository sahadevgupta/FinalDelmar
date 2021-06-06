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

        public string Otp { get; private set; }

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


                    if (AppResources.Culture.IetfLanguageTag == "ar-EG")
                    {
                        otpComp.OtpTimer = $"تنتهي الصلاحية في {string.Format("{0:00}: {1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds)} دقيقة";
                    }
                    else
                        otpComp.OtpTimer = $"Expires in {string.Format("{0:00}:{1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds)} mins";
                }
                else
                    otpComp.OtpTimer = string.Empty;


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
            DependencyService.Get<INotify>().ShowSnackBar($"Your OTP : {Otp}");

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
                    if (Otp.Equals(otpComp.OtpValue))
                    {
                        var response = await _commonModel.VerifyPhoneAsync(MobileNumber);
                        if (response == false)
                        {
                            await NavigationService.NavigateAsync($"../{nameof(SignUpPage)}");
                        }
                        else if(response == true)
                            GoToMainScreen();
                        else
                            await App.dialogService.DisplayAlertAsync("Error!!", AppData.Msg, "OK");
                    }
                    else
                        await App.dialogService.DisplayAlertAsync("Error!!", AppResources.txtInvalidOtp, "OK");

                   
                    
                }
                else
                {
                    var Otp = await _commonModel.GenerateOTPAsync(MobileNumber);
                    DependencyService.Get<INotify>().ShowSnackBar($"Your OTP : {Otp}");
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
            IsPageEnabled = true;
            SendFCMTokenToServer();
            if (FromItemPage)
            {
                MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "LoggedIn");

                await NavigationService.NavigateAsync("../../");
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


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            FromItemPage = parameters.GetValue<bool>("itemPage");
            MobileNumber = parameters.GetValue<string>("number");
            Otp = parameters.GetValue<string>("otp");
        }
    }
}
