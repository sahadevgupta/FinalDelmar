using FormsLoyalty.Interfaces;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using System;
using Xamarin.Forms;
using XF.Material.Forms.UI;

namespace FormsLoyalty.Views
{
    public partial class SignUpPage : ContentPage
    {
        SignUpPageViewModel _viewModel;
        public SignUpPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as SignUpPageViewModel;
            
        }

        private async void nextbtn_Clicked(object sender, System.EventArgs e)
        {
            if (ValidateData())
            {
                if(mobile.Text.Length == 11)
                {
                    var _digit = mobile.Text.Substring(0, 3);
                    if (_digit.Equals("011") || _digit.Equals("012") || _digit.Equals("015") || _digit.Equals("010"))
                        await _viewModel.AssignToMemberContact(userName.Text);
                    else
                        await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));

                }
                else
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                }
            }
            else
                return;
        }

        private bool ValidateData()
        {
            //if (string.IsNullOrEmpty(userName.Text))
            //{
            //    userName.HasError = true;
            //}
            //if (string.IsNullOrEmpty(email.Text))
            //{
            //    email.HasError = true;
            //}

            if (string.IsNullOrEmpty(firstName.Text))
            {
                firstName.HasError = true;
            }
            if (string.IsNullOrEmpty(lastName.Text))
            {
                lastName.HasError = true;
            }
            if (string.IsNullOrEmpty(mobile.Text))
            {
                mobile.HasError = true;
            }
            if (string.IsNullOrEmpty(streetlbl.Text))
            {
                streetlbl.HasError = true;
            }
            if (string.IsNullOrEmpty(numberlbl.Text))
            {
                numberlbl.HasError = true;
            }
            if (string.IsNullOrEmpty(floorlbl.Text))
            {
                floorlbl.HasError = true;
            }
            if (string.IsNullOrEmpty(Apartmentlbl.Text))
            {
                Apartmentlbl.HasError = true;
            }


            //if (!_viewModel.editAccount)
            //{
            //    if (string.IsNullOrEmpty(password.Text))
            //    {
            //        password.HasError = true;
            //    }
            //    if (string.IsNullOrEmpty(confPassword.Text))
            //    {
            //        confPassword.HasError = true;
            //    }
            //    if (password.Text != confPassword.Text)
            //    {
            //        confPassword.HasError = true;
            //        confPassword.ErrorText = "Password does not match";
            //    }
            //}

            if (string.IsNullOrEmpty(_viewModel.SelectedArea?.Area) || string.IsNullOrEmpty(_viewModel.SelectedCity?.City))
            {
                DependencyService.Get<INotify>().ShowToast(AppResources.txtSelectCityArea);
               
            }

            if (/*string.IsNullOrEmpty(email.Text) ||*/
                //(string.IsNullOrEmpty(password.Text) && !_viewModel.editAccount) || (string.IsNullOrEmpty(confPassword.Text) && !_viewModel.editAccount) ||
                string.IsNullOrEmpty(firstName.Text)|| string.IsNullOrEmpty(lastName.Text) || string.IsNullOrEmpty(mobile.Text) ||
                string.IsNullOrEmpty(_viewModel.SelectedArea?.Area) || string.IsNullOrEmpty(_viewModel.SelectedCity?.City)   /*|| ((password.Text != confPassword.Text) && !_viewModel.editAccount)*/
                || string.IsNullOrEmpty(floorlbl.Text) || string.IsNullOrEmpty(streetlbl.Text) || string.IsNullOrEmpty(numberlbl.Text)
                || string.IsNullOrEmpty(Apartmentlbl.Text))
            {
                return false;

            }
            else
                return true;
        }

        private void AddNewAddress(object sender, EventArgs e)
        {

        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await _viewModel.GoBack();
        }

        private void MaterialTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (MaterialTextField)sender;
            if (!string.IsNullOrEmpty(view.Text))
            {
                if (view.HasError)
                {
                    view.HasError = false;
                }
            }
        }
    }
}
