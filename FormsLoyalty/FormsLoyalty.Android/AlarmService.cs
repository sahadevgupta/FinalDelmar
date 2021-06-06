using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using Java.Util;
using Newtonsoft.Json;

namespace FormsLoyalty.Droid
{
    [Service(Exported = true, Name = "com.LinkedGates.LinkedCommerce.AlarmService")]
    public class AlarmService : Service
    {

        public override void OnCreate()
        {
            base.OnCreate();
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");
            var notificatonId = intent.GetIntExtra("id", 0);
            var time = intent.GetStringExtra("time");
            var date = intent.GetStringExtra("date");
            var days = intent.GetStringExtra("days");

            TimeSpan t = JsonConvert.DeserializeObject<TimeSpan>(time);
            DateTime d = JsonConvert.DeserializeObject<DateTime>(date);

            #region Change time format 12hr
            var hours = t.Hours;
            var minutes = t.Minutes;
            var amPmDesignator = "AM";
            if (hours == 0)
                hours = 12;
            else if (hours == 12)
                amPmDesignator = "PM";
            else if (hours > 12)
            {
                hours -= 12;
                amPmDesignator = "PM";
            }
            var formattedTime =
              String.Format("{0}:{1:00} {2}", hours, minutes, amPmDesignator);

            #endregion


            


            Intent broadcastIntent = new Intent(Xamarin.Essentials.Platform.CurrentActivity, typeof(AlarmReceiver));
            broadcastIntent.PutExtra("title", title);
            broadcastIntent.PutExtra("message", message);
            

            Calendar setcalendar = Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("UTC"));

            //setcalendar.Set(CalendarField.)
            setcalendar.Set(CalendarField.HourOfDay, t.Hours);
            setcalendar.Set(CalendarField.Minute, t.Minutes);
            setcalendar.Set(CalendarField.Second, 0);

            if (!string.IsNullOrEmpty(days))
            {
                var weekdays = days.Split(",");
                for (int i = 0; i < weekdays.Count(); i++)
                {
                    broadcastIntent.PutExtra("id", notificatonId+i);
                    SetForDayAlarm(Convert.ToInt32(weekdays[i]) + 1, setcalendar, broadcastIntent);
                } 
               
            }
            else
            {
                broadcastIntent.PutExtra("id", notificatonId);

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, broadcastIntent, PendingIntentFlags.CancelCurrent);
                AlarmManager am = (AlarmManager)GetSystemService(AlarmService);
                am.SetInexactRepeating(AlarmType.RtcWakeup, setcalendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
            }

            return StartCommandResult.Sticky;
        }

        private void SetForDayAlarm(int week,Calendar calSet,Intent broadcastIntent)
        {
            calSet.Set(CalendarField.DayOfWeek, week);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, broadcastIntent, PendingIntentFlags.CancelCurrent);
            AlarmManager am = (AlarmManager)GetSystemService(AlarmService);
            am.SetInexactRepeating(AlarmType.RtcWakeup, calSet.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override IBinder OnBind(Intent intent)
        {
            throw null;
        }
    }
}