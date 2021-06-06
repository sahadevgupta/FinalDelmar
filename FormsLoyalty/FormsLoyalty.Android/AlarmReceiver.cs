using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using AndroidApp = Android.App.Application;
namespace FormsLoyalty.Droid
{
    [BroadcastReceiver(Enabled = true,Name = "com.LinkedGates.LinkedCommerce.AlarmReceiver", Label = "Local Notifications Broadcast Receiver")]
    public class AlarmReceiver : BroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";
        bool channelInitialized = false;

        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications."; 
        NotificationManager manager;

        [Obsolete]
        public override void OnReceive(Context context, Intent intent)
        {
            // NotificationManager nManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");
            var notificatonId = intent.GetIntExtra("id", 0);


            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.SetFlags(ActivityFlags.ClearTop);

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);
            var resultPendingIntent =
                stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // get notification sound
            Android.Net.Uri alarmUri = Android.Net.Uri.Parse($"{ ContentResolver.SchemeAndroidResource}://{MainApplication.ActivityContext.PackageName}/{Resource.Drawable.Whistle}");

            if (!channelInitialized)
            {
                CreateNotificationChannel(alarmUri);
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(resultPendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSound(alarmUri)
                .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.logo))
                .SetSmallIcon(Resource.Drawable.logo);


            var notification = builder.Build();
            manager.Notify(notificatonId, notification);

            //var notificationBuilder = new Notification.Builder(context)
            //    .SetSmallIcon(Resource.Drawable.ic_action_email)
            //    .SetContentTitle("FCM Message")
            //    .SetContentText("messageBody")
            //    .SetAutoCancel(true)
            //     .SetAutoCancel(true)
            //    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
            //    .SetContentIntent(resultPendingIntent);

            //manager.Notify(10, notificationBuilder.Build());

            // nManager.Notify(10, notificationBuilder.Build());


        }
        void CreateNotificationChannel(Android.Net.Uri alarmUri)
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

                channel.SetSound(alarmUri, alarmAttributes);

                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
    }
}