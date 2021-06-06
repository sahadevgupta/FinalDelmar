
using System;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Activities.Items;
using Presentation.Activities.Search;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Presentation.Views;
using Android.Widget;
using Android;
using Plugin.Media;
using Android.App;

namespace Presentation.Activities.ScanSend
{
    public class ScanSendFragment : LoyaltyFragment, IBroadcastObserver, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, PopupMenu.IOnMenuItemClickListener
    {
        ImageView thisImageView;
        Presentation.Views.ColoredButton captureButton;
        Presentation.Views.ColoredButton uploadButton;

        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };
        public void BroadcastReceived(string action)
        {
            throw new NotImplementedException();
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            throw new NotImplementedException();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void OnRefresh()
        {
            throw new NotImplementedException();
        }

        public void ShowIndicator(bool show)
        {
            throw new NotImplementedException();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            

            RequestPermissions(permissionGroup, 0);

            HasOptionsMenu = true;

            var isBigScreen = Resources.GetBoolean( Resource.Boolean.BigScreen);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ScanSend);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ScanSendToolbar);

            thisImageView = view.FindViewById<ImageView>(Resource.Id.thisImageView);
            captureButton = view.FindViewById<Presentation.Views.ColoredButton>(Resource.Id.ScanAndSendViewCapture);
            uploadButton = view.FindViewById<Presentation.Views.ColoredButton>(Resource.Id.ScanAndSendViewUploadPhoto);

            captureButton.Click += CaptureButton_Click;
            uploadButton.Click += UploadButton_Click;

            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);



            if (isBigScreen)
            {
            }
            else
            {
            }

            return view;
        }

        private void UploadButton_Click(object sender, System.EventArgs e)
        {
            try {

                UploadPhoto();

            }
            catch (Exception ex)
            {

            }
        }

        private void CaptureButton_Click(object sender, System.EventArgs e)
        {

            try
            {

                TakePhoto();

            }
            catch (Exception ex)
            {

            }

        }
        async void TakePhoto()
        {
            try {
                await CrossMedia.Current.Initialize();

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 40,
                    Name = "myimage.jpg",
                    Directory = "sample"

                });

                if (file == null)
                {

                }
                else
                {

                    // Convert file to byte array and set the resulting bitmap to imageview
                    byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                    thisImageView.SetImageBitmap(bitmap);
                }
            }
            catch (Exception ex)
            { }
           
           

        }

        async void UploadPhoto()
        {
            try {

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    Toast.MakeText(Context, "Upload not supported on this device", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                    CompressionQuality = 40

                });

                // Convert file to byre array, to bitmap and set it to our ImageView

                byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
                Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                thisImageView.SetImageBitmap(bitmap);
            } catch (Exception ex) {


            }
           

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}