using FormsLoyalty.Controls;
using FormsLoyalty.ViewModels;
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
                        await _viewModel.AssignToMemberContact(_viewModel.UserName);
                    else
                        await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));

                }
                else
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Mobile Number not in correct format", AppResources.ResourceManager.GetString("ApplicationOk", AppResources.Culture));
                }
            }
            
        }

        private bool ValidateData()
        {
            

            if (string.IsNullOrEmpty(firstName.Text))
            {
                firstName.IsError = true;
            }
            if (string.IsNullOrEmpty(lastName.Text))
            {
                lastName.IsError = true;
            }
            if (string.IsNullOrEmpty(mobile.Text))
            {
                mobile.IsError = true;
            }
            if (string.IsNullOrEmpty(streetlbl.Text))
            {
                streetlbl.IsError = true;
            }
            if (string.IsNullOrEmpty(numberlbl.Text))
            {
                numberlbl.IsError = true;
            }
            if (string.IsNullOrEmpty(floorlbl.Text))
            {
                floorlbl.IsError = true;
            }
            if (string.IsNullOrEmpty(Apartmentlbl.Text))
            {
                Apartmentlbl.IsError = true;
            }


            if (string.IsNullOrEmpty(_viewModel.SelectedCity?.City))
            {
                citypicker.IsError =  true;


            }

            if (string.IsNullOrEmpty(_viewModel.SelectedArea?.Area))
            {
                areapicker.IsError = true;
            }

            if (string.IsNullOrEmpty(firstName.Text)|| string.IsNullOrEmpty(lastName.Text) || string.IsNullOrEmpty(mobile.Text) ||
                string.IsNullOrEmpty(_viewModel.SelectedArea?.Area) || string.IsNullOrEmpty(_viewModel.SelectedCity?.City)  
                || string.IsNullOrEmpty(floorlbl.Text) || string.IsNullOrEmpty(streetlbl.Text) || string.IsNullOrEmpty(numberlbl.Text)
                || string.IsNullOrEmpty(Apartmentlbl.Text))
            {
                return false;

            }
            else
                return true;
        }

       

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await _viewModel.GoBack();
        }


        private void ExtendedEntry_OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (ExtendedEntry)sender;
            if (!string.IsNullOrEmpty(view.Text))
            {

                view.IsError = false;
            }
            else
            {
                view.IsError = true;
            }
        }
    }
}
