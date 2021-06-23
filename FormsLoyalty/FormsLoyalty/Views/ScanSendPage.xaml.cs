using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class ScanSendPage : ContentPage
    {
        ScanSendPageViewModel _viewModel;
        public ScanSendPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as ScanSendPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var selected = (e as TappedEventArgs).Parameter as GMFileInfo;
            
                //var frame = (Frame)sender;

              // await frame.ScaleTo(2, 2000);
            
           

            

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
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        _viewModel.NavigateToCameraView();
                    }
                    else
                        await _viewModel.TakePickure();
                }
                    

            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                _viewModel.IsPageEnabled = false;
            }
                
        }

    }
}
