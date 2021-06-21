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

            // zoomLabel.Text = string.Format("Zoom: {0}", zoomSlider.Value);
            CameraOptions = CameraOptions.Front;
            FlashMode = CameraFlashMode.Off;
        }

        void ZoomSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            //cameraView.Zoom = (float)zoomSlider.Value;
           // zoomLabel.Text = string.Format("Zoom: {0}", Math.Round(zoomSlider.Value));
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
            //doCameraThings.Text = cameraView.CaptureMode == CameraCaptureMode.Video
            //             ? "Stop Recording"
            //             : "Snap Picture";
        }
        void CameraView_OnAvailable(object sender, bool e)
        {
            //if (e)
            //{
            //    zoomSlider.Value = cameraView.Zoom;
            //    var max = cameraView.MaxZoom;
            //    if (max > zoomSlider.Minimum && max > zoomSlider.Value)
            //        zoomSlider.Maximum = max;
            //    else
            //        zoomSlider.Maximum = zoomSlider.Minimum + 1; // if max == min throws exception
            //}

            //doCameraThings.IsEnabled = e;
            //zoomSlider.IsEnabled = e;
        }

        void CameraView_MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            switch (cameraView.CaptureMode)
            {
                default:
                case CameraCaptureMode.Default:
                case CameraCaptureMode.Photo:

                   var bytes =  DependencyService.Get<IMediaService>().CompressImage(e.ImageData);

                    _viewModel.NavigateToScanSendPage(bytes);

                   

                    break;
                case CameraCaptureMode.Video:
                   // previewPicture.IsVisible = false;
                    break;
            }
        }

       
        private void cameraView_MediaCaptureFailed(object sender, string e)
        {

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        //private void retakeBtn_Clicked(object sender, EventArgs e)
        //{
        //    previewPicture.IsVisible = false;
        //    _viewModel.CapturedImage = null;
        //}
    }
}