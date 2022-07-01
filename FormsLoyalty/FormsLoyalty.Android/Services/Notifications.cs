using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using FormsLoyalty.Droid.Services;
using FormsLoyalty.Interfaces;
using Google.Android.Material.Snackbar;

using Java.Util;
using Newtonsoft.Json;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;
[assembly: Dependency(typeof(Notifications))]
namespace FormsLoyalty.Droid.Services
{
    public class Notifications : INotify
    {
        public Java.IO.File file { get; private set; }
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";
        bool channelInitialized = false;
        NotificationManager manager;

        public string getDeviceUuid()
        {
            var uuid = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            return "ANDROID" + uuid;
        }

        [Obsolete]
        public string GetImageUri(string encodedImage,string Description)
        {
            byte[] decodedString;
            if (encodedImage.Contains("noimage",StringComparison.OrdinalIgnoreCase))
            {
                var mDrawable = System.IO.Path.GetFileNameWithoutExtension("iconLogo.jpg");
                var resourceId = (int)typeof(Resource.Drawable).GetField(mDrawable).GetValue(null);
                // int resID = this.Context.Resources.GetIdentifier("Ulogin.png", "drawable", "com.companyname.appname");
                var drawable = ContextCompat.GetDrawable(Xamarin.Essentials.Platform.AppContext, resourceId);
                var bitmap = ((BitmapDrawable)drawable).Bitmap;


               // Bitmap bm = BitmapFactory.DecodeFile("logo.png");
                MemoryStream baos = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, baos); // bm is the bitmap object
                decodedString = baos.ToArray();
            }
            else
                decodedString = Android.Util.Base64.Decode(encodedImage, Base64Flags.Default);

            Android.Graphics.Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);

            Android.Net.Uri bmpUri = null;
           
            try
            {

                file = new Java.IO.File(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), Description.Replace(" ", "") + ".png");

                file.ParentFile.Mkdirs();

                file.Delete();

                using (var os = new FileStream(file.Path, FileMode.CreateNew))
                {
                    decodedByte.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 90, os);
                }

                bmpUri = Android.Net.Uri.FromFile(file);
            }
            catch (System.IO.IOException)
            {
            }
            return file.Path;
        }

        public void ShowLocalNotification(string title, string body, int id, DateTime notifyTime)
        {

        }

        [Obsolete]
        public void ShowLocalNotification(string title, string body)
        {
            var intent = new Intent(Xamarin.Essentials.Platform.AppContext, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(Xamarin.Essentials.Platform.AppContext, 0, intent, PendingIntentFlags.OneShot);


            //Android.Net.Uri alarmUri = Android.Net.Uri.Parse($"{ ContentResolver.SchemeAndroidResource}://{MainApplication.ActivityContext.PackageName}/{Resource.Drawable.Whistle}");
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }
           

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.logo))
                .SetSmallIcon(Resource.Drawable.logo);


            var notification = builder.Build();
            manager.Notify(0, notification);


           



           
              

            //NotificationCompat.Builder builder = new NotificationCompat.Builder(MainApplication.ActivityContext, CHANNEL_ID)
            //                                                           .SetContentTitle(title)
            //                                                           .SetContentText(body)
            //                                                           .SetSmallIcon(Resource.Drawable.AppIcon)


            //// Build the notification:
            //Notification notification = builder.Build();

            //// Get the notification manager:
            //NotificationManager notificationManager =
            //    MainApplication.ActivityContext.GetSystemService(Context.NotificationService) as NotificationManager;

            //// Publish the notification:
            //const int notificationId = 0;
            //notificationManager.Notify(notificationId, notification);
        }


        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.High)
                {
                    Description = channelDescription,
                    LockscreenVisibility = NotificationVisibility.Public,

                };

                // Creating an Audio Attribute
                var alarmAttributes = new AudioAttributes.Builder()
                                       .SetContentType(AudioContentType.Sonification)
                                       .SetUsage(AudioUsageKind.Alarm)
                                       .Build();



                channel.EnableVibration(true);

                //channel.SetSound(alarmUri, alarmAttributes);

                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
        public void ShowSnackBar(string description)
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.RunOnUiThread(() =>
            {
                var v = activity.FindViewById(Android.Resource.Id.Content);
                Snackbar mSnackBar = Snackbar.Make(v, description, Snackbar.LengthLong);
                Android.Views.View view = mSnackBar.View;
                FrameLayout.LayoutParams param = (FrameLayout.LayoutParams)view.LayoutParameters;
                param.Gravity = GravityFlags.Top;
                view.LayoutParameters = param;
                //view.SetBackgroundColor(Color.Red);
                //TextView mainTextView = (TextView)(view).FindViewById(.design.R.id.snackbar_text);
                //mainTextView.setTextColor(Color.WHITE);
                mSnackBar.Show();


               // Snackbar.Make(view, description, 2000).Show();
            });
        }

        public void ShowToast(string description)
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.RunOnUiThread(() =>
            {
                var view = activity.FindViewById(Android.Resource.Id.Content);
                Toast.MakeText(activity, description, ToastLength.Short).Show();
            });
        }
        public void SetReminder(string title, string body, int id, TimeSpan time, DateTime startDate, string days)
        {
            

            var timespan = JsonConvert.SerializeObject(time);
            var date = JsonConvert.SerializeObject(startDate);

            Intent service = new Intent(AndroidApp.Context, typeof(AlarmService));
            service.PutExtra("title", title);
            service.PutExtra("message", body);
            service.PutExtra("id", id);
            service.PutExtra("time", timespan);
            service.PutExtra("date", date);
            service.PutExtra("days", days);

            AndroidApp.Context.StartService(service);

        }

        public void ChangeTabBarFlowDirection(bool rtl)
        {
            if (rtl)
            {
                MainActivity._window.DecorView.LayoutDirection = Android.Views.LayoutDirection.Rtl;
            }
            else
                MainActivity._window.DecorView.LayoutDirection = Android.Views.LayoutDirection.Ltr;
        }

        public void DeleteReminderNotification(int notificationId)
        {
            PendingIntent pendingIntent = GetPendingIntent(notificationId);
            var alarmManager = MainActivity.Instance.GetSystemService(Context.AlarmService) as AlarmManager;
        }

        private PendingIntent GetPendingIntent(int id)
        {
            Intent intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(MainActivity.Instance, id, intent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private Intent CreateIntent(int id)
        {
            return new Intent(MainActivity.Instance, typeof(AlarmReceiver))
                .SetAction("localNotifierIntent" + id);
        }

        public void DeleteAllReminderNotification(List<int> notificationIds)
        {
            foreach (var notificationId in notificationIds)
            {
                DeleteReminderNotification(notificationId);
            }
        }
    }
}