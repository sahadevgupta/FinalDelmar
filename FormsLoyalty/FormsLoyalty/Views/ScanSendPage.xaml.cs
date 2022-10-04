﻿using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class ScanSendPage : ContentPage
    {
        ScanSendPageViewModel _viewModel;
        public ScanSendPage()
        {
            InitializeComponent();
            videoPlayer.Play();
            _viewModel = BindingContext as ScanSendPageViewModel;
        }

        private async void UploadImg_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                var request = await DisplayActionSheet(AppResources.txtUploadPhoto, AppResources.ApplicationCancel, null, AppResources.txtCamera, AppResources.txtGallery);
                if (request == null || request.Equals(AppResources.ApplicationCancel))
                {
                    return;
                }
                if (request.Equals(AppResources.txtGallery))
                {
                    await ImageHelper.PickFromGallery(5);
                }
                else
                {
                   await _viewModel.TakePickure();
                }
                    

            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                _viewModel.IsPageEnabled = false;
            }
                
        }

        private void Prescription_Clicked(object sender, System.EventArgs e)
        {
            videoPlayer.Stop();
            _viewModel.IsPrescriptionViewVisible = true;
        }

        private void videoPlayer_MediaOpened(object sender, System.EventArgs e)
        {
            prescriptionBtn.IsEnabled = true;
        }
    }
}
