using FormsLoyalty.Controls;
using FormsLoyalty.Helpers;
using FormsLoyalty.Services;
using FormsLoyalty.ViewModels;
using System;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class LoginOtpView : ContentPage
    {
        LoginOtpViewViewModel _viewModel;
        public LoginOtpView()
        {
            InitializeComponent();
            _viewModel = BindingContext as LoginOtpViewViewModel;
            
            OtpComponentView otpcomp = new OtpComponentView("Enter OTP", new Binding("OtpTimerBinding",mode:BindingMode.TwoWay,source:this), _viewModel,6);
            otpcomp.Margin = new Thickness(10, 0, 0, 0);
            _viewModel.otpComp = otpcomp;
            _viewModel.StartTimer();
            mainGrid.Children.Add(otpcomp, 0, 0);
            Action<string> printOTP = ReadOTP;
            CommonHelper.Subscribe(this, Events.SmsRecieved, printOTP);
        }

        /// <summary>
        /// The ReadOTP.
        /// </summary>
        /// <param name="otp">The otp<see cref="string"/>.</param>
        private void ReadOTP(string otp)
        {
            _viewModel.otpComp.FillReceivedOTP(otp);
            CommonServices.ListenToSmsRetriever();
        }
    }
}
