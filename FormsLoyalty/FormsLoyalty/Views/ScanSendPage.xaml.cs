using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using System.Threading.Tasks;
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
            _viewModel.CanNavigate = true;
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
                var request = await DisplayActionSheet("Upload a new photo", "Cancel", null, "Camera", "Gallery");
                if (request == "Cancel")
                {
                    return;
                }
                if (request == "Gallery")
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
