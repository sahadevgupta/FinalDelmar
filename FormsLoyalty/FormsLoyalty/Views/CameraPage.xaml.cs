using FormsLoyalty.Interfaces;
using FormsLoyalty.ViewModels;
using Rg.Plugins.Popup.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {

        CameraPageViewModel _viewModel;

        public static CameraOptions CameraOptions;

        public static CameraFlashMode FlashMode { get; private set; }
     
        public CameraPage()
        {
            InitializeComponent();

            _viewModel = BindingContext as CameraPageViewModel;

            CameraOptions = CameraOptions.Front;
            FlashMode = CameraFlashMode.Off;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            cameraView.FlashMode = FlashMode;
        }

        // You can also set it to Default and External
        void FrontCameraSwitch_Toggled(object sender, EventArgs e)
        {
            var animation = new Animation(v => rotateIcon.Rotation = v, 0, 360);
            animation.Commit(this, "RotateAnimation", 16, 250, Easing.Linear, (v, c) => rotateIcon.Scale = 1, () => false);

            cameraView.CameraOptions = CameraOptions == CameraOptions.Front ? CameraOptions.Back : CameraOptions.Front;
            CameraOptions = cameraView.CameraOptions;
            
        }
             

        // You can also set it to Torch (always on) and Auto
        void FlashSwitch_Toggled(object sender, EventArgs e)
        {

            cameraView.FlashMode = FlashMode == CameraFlashMode.Off ? CameraFlashMode.On : CameraFlashMode.Off;
            FlashMode = cameraView.FlashMode;

            switch (cameraView.FlashMode)
            {
                case CameraFlashMode.Off:
                    flashIcon.Source = "iconFlashOff";
                    break;
                case CameraFlashMode.On:
                    flashIcon.Source = "iconFlashOn";
                    break;
                case CameraFlashMode.Auto:
                    break;
                case CameraFlashMode.Torch:
                    break;
                default:
                    break;
            }
           
        }
     

        void DoCameraThings_Clicked(object sender, EventArgs e)
        {
            cameraView.Shutter();
        }

        void CameraView_MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            switch (cameraView.CaptureMode)
            {
                case CameraCaptureMode.Default:
                case CameraCaptureMode.Photo:

                   var bytes =  DependencyService.Get<IMediaService>().CompressImage(e.ImageData);

                    _viewModel.NavigateToScanSendPage(bytes);

                   

                    break;
            }
        }

    }
}