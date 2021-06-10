using FormsLoyalty.Helpers.Media;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Image;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using Microsoft.AppCenter.Crashes;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using CameraDevice = FormsLoyalty.Interfaces.CameraDevice;

namespace FormsLoyalty.Helpers
{
    public static class ImageHelper
    {
        private static ImageService service;
        private static string securityTokenInUse;

        public static async Task<ImageView> GetImageById(string id, ImageSize imageSize)
        {
            ImageView imageView = null;

            if (imageSize.Height == 0)
            {
                imageSize.Height = Int32.MaxValue;
            }

            if (imageSize.Width == 0)
            {
                imageSize.Width = Int32.MaxValue;
            }

            BeginWsCall();

            try
            {
                imageView = await service.ImageGetByIdAsync(id, imageSize);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            if (imageView == null)
            {
                imageView = new ImageView { Image = "noimage.png" };
            }
            else if (string.IsNullOrEmpty( imageView?.Image))
            {
                imageView.Image = "noimage.png";
            }
            return imageView;
        }

        static void BeginWsCall()
        {
            if (string.IsNullOrEmpty(securityTokenInUse) || AppData.Device.SecurityToken != securityTokenInUse)
            {
                CreateService();
            }

            securityTokenInUse = AppData.Device.SecurityToken;
        }

        private static void CreateService()
        {
            service = new ImageService(new ImageRepository());
        }

        public static async Task<Plugin.Media.Abstractions.MediaFile> TakePictureAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }
                if (status == PermissionStatus.Granted)
                {
                    var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    if (storageStatus != PermissionStatus.Granted)
                    {
                        storageStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    }
                }
                if (status != PermissionStatus.Granted)
                {
                   await  App.dialogService.DisplayAlertAsync("Error!!", "Camera Permission not granted, please go to settings to enable it", "OK");
                    return null;  
                }
               
                if (Device.RuntimePlatform == Device.Android)
                {
                    var file = await DependencyService.Get<IMediaService>().TakePhotoAsync(new CameraMediaStorageOptions()
                    {

                        Name = "test",
                        SaveMediaOnCapture = true,
                        DefaultCamera = CameraDevice.Front,
                        MaxPixelDimension = 400,
                        PercentQuality = 75
                    });

                    return new Plugin.Media.Abstractions.MediaFile(file.Path, () => streamgetter(file));
                }
                else
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await App.dialogService.DisplayAlertAsync("No Camera", ":( No camera available.", "OK");
                        return null;

                    }

                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                    {
                        DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                        SaveToAlbum = true,
                        SaveMetaData = true

                    });


                    if (file == null)
                    {
                        return null;

                    }
                    return file;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("permission denied".ToLower()))
                {
                    await App.dialogService.DisplayAlertAsync("Error!!", "Camera Permission not granted, please go to settings to enable it", "OK");
                }
                
                return null;
            }
            
            
        }

        private static Stream streamgetter(Media.MediaFile file)
        {
            return file.Source;
        }

        public static async Task PickFromGallery(int count)
        {
            await Xamarin.Forms.DependencyService.Get<IMediaService>().OpenGallery(count);
        }
    }
}
