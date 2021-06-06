using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Widget;
using FormsLoyalty.Droid.Services;
using FormsLoyalty.Droid.Services.Media;
using FormsLoyalty.Helpers.Media;
using FormsLoyalty.Interfaces;
using Java.IO;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using MediaFile = FormsLoyalty.Helpers.Media.MediaFile;

[assembly: Xamarin.Forms.Dependency(typeof(MediaService))]
namespace FormsLoyalty.Droid.Services
{
    public class MediaService : IMediaService
    {

        private TaskCompletionSource<MediaFile> _completionSource;
        private int _requestId;

        private static Context Context
        {
            get { return Application.Context; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaService"/> class.
        /// </summary>
        public MediaService()
        {
            IsPhotosSupported = true;

            // Lines added
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
        }

        /// <summary>	
        /// Gets a value indicating whether this instance is camera available.
        /// </summary>
        /// <value><c>true</c> if this instance is camera available; otherwise, <c>false</c>.</value>
        public bool IsCameraAvailable
        {
            get
            {
                var isCameraAvailable = Context.PackageManager.HasSystemFeature(PackageManager.FeatureCamera);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
                {
                    isCameraAvailable |= Context.PackageManager.HasSystemFeature(PackageManager.FeatureCameraFront);
                }

                return isCameraAvailable;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is photos supported.
        /// </summary>
        /// <value><c>true</c> if this instance is photos supported; otherwise, <c>false</c>.</value>
        public bool IsPhotosSupported { get; private set; }

        public async Task OpenGallery(int imageCount)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                }

                if (status == PermissionStatus.Granted)
                {
                    Toast.MakeText(MainApplication.ActivityContext, $"Select Images", ToastLength.Long).Show();
                    var imageIntent = new Intent(
                        Intent.ActionPick);
                    imageIntent.SetType("image/*");
                    imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                    imageIntent.SetAction(Intent.ActionGetContent);
                    ((Activity)MainApplication.ActivityContext).StartActivityForResult(
                        Intent.CreateChooser(imageIntent, "Select photo"), MainActivity.OPENGALLERYCODE);

                }
                else if (status != PermissionStatus.Unknown)
                {
                    Toast.MakeText(MainApplication.ActivityContext, "Permission Denied. Can not continue, try again.", ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                Toast.MakeText(MainApplication.ActivityContext, "Error. Can not continue, try again.", ToastLength.Long).Show();
            }
        }



        /// <summary>
        /// Takes the picture.
        /// </summary>
        /// <param name="options">The storage options.</param>
        /// <returns>Task with a return type of MediaFile.</returns>
        /// <exception cref="System.NotSupportedException">Throws an exception if feature is not supported.</exception>
        public Task<MediaFile> TakePhotoAsync(CameraMediaStorageOptions options)
        {
            if (!IsCameraAvailable)
            {
                throw new NotSupportedException();
            }

            options.VerifyOptions();

            return TakeMediaAsync("image/*", MediaStore.ActionImageCapture, options);
        }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <returns>Request id as integer.</returns>
        private int GetRequestId()
        {
            var id = _requestId;
            if (_requestId == int.MaxValue)
            {
                _requestId = 0;
            }
            else
            {
                _requestId++;
            }

            return id;
        }


        /// <summary>
        /// Takes the media asynchronous.
        /// </summary>
        /// <param name="type">The type of intent.</param>
        /// <param name="action">The action.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task with a return type of MediaFile.</returns>
        /// <exception cref="System.InvalidOperationException">Only one operation can be active at a time.</exception>
        private Task<MediaFile> TakeMediaAsync(string type, string action, MediaStorageOptions options)
        {
            var id = GetRequestId();

            var ntcs = new TaskCompletionSource<MediaFile>(id);
            if (Interlocked.CompareExchange(ref _completionSource, ntcs, null) != null)
            {
                throw new InvalidOperationException("Only one operation can be active at a time");
            }

            Context.StartActivity(CreateMediaIntent(id, type, action, options));

            EventHandler<MediaPickedEventArgs> handler = null;
            handler = (s, e) =>
            {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                MediaPickerActivity.MediaPicked -= handler;

                if (e.RequestId != id)
                {
                    return;
                }

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else if (e.IsCanceled)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult(e.Media);
                }
            };

            MediaPickerActivity.MediaPicked += handler;

            return ntcs.Task;
        }

        /// <summary>
        /// Creates the media intent.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type of intent.</param>
        /// <param name="action">The action.</param>
        /// <param name="options">The options.</param>
        /// <param name="tasked">if set to <c>true</c> [tasked].</param>
        /// <returns>Intent to create media.</returns>
        private Intent CreateMediaIntent(int id, string type, string action, MediaStorageOptions options, bool tasked = true)
        {
            var pickerIntent = new Intent(Context, typeof(MediaPickerActivity));
            pickerIntent.SetFlags(ActivityFlags.NewTask);
            pickerIntent.PutExtra(MediaPickerActivity.ExtraId, id);
            pickerIntent.PutExtra(MediaPickerActivity.ExtraType, type);
            pickerIntent.PutExtra(MediaPickerActivity.ExtraAction, action);
            pickerIntent.PutExtra(MediaPickerActivity.ExtraTasked, tasked);

            if (options != null)
            {
                pickerIntent.PutExtra(MediaPickerActivity.ExtraPath, options.Directory);
                pickerIntent.PutExtra(MediaStore.Images.ImageColumns.Title, options.Name);

                //var vidOptions = options as VideoMediaStorageOptions;
                //if (vidOptions != null)
                //{
                //    pickerIntent.PutExtra(MediaStore.ExtraDurationLimit, (int)vidOptions.DesiredLength.TotalSeconds);
                //    pickerIntent.PutExtra(MediaStore.ExtraVideoQuality, (int)vidOptions.Quality);
                //}
            }

            return pickerIntent;
        }

    }
}