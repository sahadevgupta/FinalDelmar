using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using FormsLoyalty.Droid.Helper;
using ImageCircle.Forms.Plugin.Droid;
using Java.Lang;
using Java.Security;
using PanCardView.Droid;
using Plugin.FacebookClient;
using Plugin.FirebasePushNotification;
using Plugin.GoogleClient;
using Prism;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Exception = System.Exception;

namespace FormsLoyalty.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTop,
         ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;
        public static int OPENGALLERYCODE = 100;
        public static int PICKFOLDERCODE = 9999;
        public static Context context;
        public static Window _window;
        internal static MainActivity Instance { get; private set; }
        readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            _window = Window;
            // Window.DecorView.LayoutDirection = LayoutDirection.Rtl; // need to assign on culture changed
            base.OnCreate(savedInstanceState);
            FacebookClientManager.Initialize(this);
            GoogleClientManager.Initialize(this);
            context = this;
            // AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            //Rg.Plugins.Popup.Popup.Init(this);

            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //Android.Glide.Forms.Init(this);

            


            Xamarin.FormsMaps.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            CardsViewRenderer.Preserve();
            //Android.Glide.Forms.Init();
            //var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            //notificationManager.CancelAll();
            // setReminder();

            //var apiCallService = new Runnable(async () =>
            //{
            //    await App.CallPCLMethod().ConfigureAwait(false);

            //});
            //apiCallService.Run();


            XF.Material.Droid.Material.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer()));
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            NavigateOnNotification(Intent);
            
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
#if DEBUG
            PrintHashKey(this);
#endif
            Instance = this;
            #region For Add SMS Read


            //string Value = AppHashKeyHelper.GetAppHashKey(this);
            //Console.WriteLine("SMS Read >>>   ------GetAppHashKey Value:" + Value);// Add
            ////Toast.MakeText(this, "GetAppHashKey => " + Value, ToastLength.Long).Show();
            ////Toast.MakeText(this, " GetIPAddress => " + AndroidNativeUtility.GetIPAddress(), ToastLength.Long).Show();
            //Intent intent = new Intent(this, typeof(SmsReceiver));
            //StartService(intent);
            #endregion

        }

        private void NavigateOnNotification(Intent intent)
        {
            var data = intent.GetStringArrayExtra("data");
            if (data != null)
            {
                //var Type = (ObjectType)Enum.Parse(typeof(ObjectType), data[0].ToString(), true);
                //switch (Type)
                //{
                //    case ObjectType.Product:
                //        App.NavigateProduct(data[4], data[5]);
                //        break;
                //    case ObjectType.Category:
                //        break;
                //    case ObjectType.Order:
                //        App.NavigateOrder(data[4]);
                //        break;
                //    case ObjectType.PrivateMessage:
                //        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                //        ISharedPreferencesEditor editor = prefs.Edit();
                //        editor.Clear();
                //        editor.Commit();
                //        App.NavigateChat(null);
                //        break;
                //    case ObjectType.FavoriteContact:
                //        App.NavigateFavorite(data[4]);
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        //public void setReminder()
        //{


        //    Intent intent = new Intent(context, typeof(AlarmService));
        //    intent.PutExtra("title", "title");
        //    intent.PutExtra("message", "body12345555");
        //    intent.PutExtra("id", 13);

        //    StartService(intent);



        //}
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                    Console.WriteLine("Location permissions already granted.");
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == RequestLocationId)
            {
                if ((grantResults.Length == 1) && (grantResults[0] == (int)Android.Content.PM.Permission.Granted))
                {
                    Console.WriteLine("Location permissions granted.");
                }
                else
                {
                    Console.WriteLine("Location permissions denied.");
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            NavigateOnNotification(Intent);
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            try
            {
                FacebookClientManager.OnActivityResult(requestCode, resultCode, intent);
                GoogleClientManager.OnAuthCompleted(requestCode, resultCode, intent);

                if (requestCode == OPENGALLERYCODE && resultCode == Result.Ok)
                {
                    List<Tuple<byte[],string>> ImageData = new List<Tuple<byte[], string>>();
                    if (intent != null)
                    {
                        ClipData clipData = intent.ClipData;
                        if (clipData != null)
                        {
                            for (int i = 0; i < clipData.ItemCount; i++)
                            {
                                ClipData.Item item = clipData.GetItemAt(i);
                                Android.Net.Uri uri = item.Uri;
                                var path = GetRealPathFromURI(uri);
                                var extension = Path.GetExtension(path);
                                var fileName = Path.GetFileNameWithoutExtension(path);
                                if (path != null)
                                {
                                    //Rotate Image
                                    var imageRotated = ImageHelper.RotateImage(path);

                                    ImageData.Add(new Tuple<byte[], string>(imageRotated,extension.Replace(".","")));
                                }
                            }
                        }
                        else
                        {
                            Android.Net.Uri uri = intent.Data;
                            var path = GetRealPathFromURI(uri);
                            var extension = Path.GetExtension(path);
                            var fileName = Path.GetFileNameWithoutExtension(path);
                            if (path != null)
                            {
                                //Rotate Image
                                var imageRotated = ImageHelper.RotateImage(path);

                                ImageData.Add(new Tuple<byte[], string>(imageRotated, extension.Replace(".","")));
                            }
                        }

                        MessagingCenter.Send<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", ImageData);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
           

        }

        [Obsolete]
        public string GetRealPathFromURI(Android.Net.Uri contentURI)
        {
            try
            {
                ICursor imageCursor = null;
                string fullPathToImage = "";

                imageCursor = ContentResolver.Query(contentURI, null, null, null, null);
                imageCursor.MoveToFirst();
                int idx = imageCursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);

                if (idx != -1)
                {
                    fullPathToImage = imageCursor.GetString(idx);
                }
                else
                {
                    ICursor cursor = null;
                    var docID = DocumentsContract.GetDocumentId(contentURI);
                    var id = docID.Split(':')[1];
                    var whereSelect = MediaStore.Images.ImageColumns.Id + "=?";
                    var projections = new string[] { MediaStore.Images.ImageColumns.Data };

                    cursor = ContentResolver.Query(MediaStore.Images.Media.InternalContentUri, projections, whereSelect, new string[] { id }, null);
                    if (cursor.Count == 0)
                    {
                        cursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, projections, whereSelect, new string[] { id }, null);
                    }
                    var colData = cursor.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Data);
                    cursor.MoveToFirst();
                    fullPathToImage = cursor.GetString(colData);
                }
                return fullPathToImage;
            }
            catch (Exception)
            {
                Toast.MakeText(MainApplication.ActivityContext, "Unable to get path", ToastLength.Long).Show();
            }
            return null;
        }


        public static void PrintHashKey(Context pContext)
        {
            try
            {
                PackageInfo info = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, PackageInfoFlags.Signatures);
                foreach (var signature in info.Signatures)
                {
                    MessageDigest md = MessageDigest.GetInstance("SHA");
                    md.Update(signature.ToByteArray());
                    string HashKey = Convert.ToBase64String(md.Digest());
                    Console.WriteLine("HashKey : " + HashKey);
                }
            }
            catch (NoSuchAlgorithmException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }


    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}

