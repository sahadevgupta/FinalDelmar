using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading;
using FormsLoyalty.Interfaces;
using FormsLoyalty.iOS.Services;
using FormsLoyalty.Models;
using Foundation;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using UIKit;
using Xamarin.Forms;
[assembly:Dependency(typeof(MediaService_iOS))]
namespace FormsLoyalty.iOS.Services
{
    public class MediaService_iOS : IMediaService
    {
        public async Task OpenGallery(int imageCount)
        {
            var picker = ELCImagePickerViewController.Create(maxImages:imageCount);
            picker.MaximumImagesCount = imageCount;

            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }
            topController.PresentViewController(picker, true, null);

            await picker.Completion.ContinueWith(data =>
            {
                picker.BeginInvokeOnMainThread(async () =>
                {
                    //dismiss the picker
                    picker.DismissViewController(true, null);

                    if (data.IsCanceled || data.Exception != null)
                    {
                    }
                    else
                    {
                        
                         var items = data.Result as List<MediaFile>;
                        List<Tuple<byte[], string>> ImageData = new List<Tuple<byte[], string>>();
                        foreach (var item in items)
                        {
                            var extension = Path.GetExtension(item.Path);

                            var bytes = item.GetStream().ToByteArray();
                            ImageData.Add(new Tuple<byte[], string>(bytes, extension.Replace(".", "")));

                        }

                        MessagingCenter.Send<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", ImageData);
                    }
                });
            });
        }

       
        Task<Helpers.Media.MediaFile> IMediaService.TakePhotoAsync(CameraMediaStorageOptions options)
        {
            throw new NotImplementedException();
        }
    }
}