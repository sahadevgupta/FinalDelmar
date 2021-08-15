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
        const int RQS_1 = 1;
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

            Locale current = Resources.Configuration.Locale;

            Calendar calNow = Calendar.GetInstance(current);
            Calendar calSet = (Calendar)calNow.Clone();


            calSet.Set(CalendarField.HourOfDay, t.Hours);
            calSet.Set(CalendarField.Minute, t.Minutes);
            calSet.Set(CalendarField.Second, 0);
            calSet.Set(CalendarField.Millisecond, 0);


            if (calSet.CompareTo(calNow) <= 0)
            {
                //Today Set time passed, count to tomorrow
                calSet.Add(CalendarField.Date, 1);
            }


            //Calendar setcalendar = Calendar.Instance;
            //setcalendar.TimeInMillis = SystemClock.CurrentThreadTimeMillis();
            //setcalendar.Set(CalendarField.)
            //setcalendar.Set(CalendarField.HourOfDay, t.Hours);
            //setcalendar.Set(CalendarField.Minute, t.Minutes);
            //setcalendar.Set(CalendarField.Second, 0);


            //Calendar cal = Calendar.Instance;
            //int year = cal.Get(CalendarField.Year);
            //int month = cal.Get(CalendarField.Month);
            //int day = cal.Get(CalendarField.Date);

            //DateTime notificateDate = new DateTime(year, month, day).AddHours(t.Hours)
            //                                                         .AddMinutes(t.Minutes).AddSeconds(0);

            //Date curr = new Date();
            //curr.Hours = t.Hours;
            //curr.Minutes = t.Minutes;
            //setcalendar.Time = curr;
            //setcalendar.Set(CalendarField.Second, 0);

            //long triggerTime = setcalendar.TimeInMillis;

            if (!string.IsNullOrEmpty(days))
            {
                var weekdays = days.Split(",");
                for (int i = 0; i < weekdays.Count(); i++)
                {
                    broadcastIntent.PutExtra("id", notificatonId+i);
                    SetForDayAlarm(Convert.ToInt32(weekdays[i]) + 1, calSet, broadcastIntent, notificatonId + i);
                } 
               
            }
            else
            {
                broadcastIntent.PutExtra("id", notificatonId);

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, notificatonId, broadcastIntent, 0);
                AlarmManager am = (AlarmManager)GetSystemService(AlarmService);
               // var interval = (long)TimeSpan.fro(Constant.One).TotalMilliseconds;
                am.SetRepeating(AlarmType.RtcWakeup, calSet.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
            }

            return StartCommandResult.Sticky;
        }
        private long GetNotifyTime(DateTime notifyTime)
        {
            int DefaultDay = 1;
            int DefaultMonth = 1;
            int DefaultYear = 1970;
            int MinYear = 0001;

            DateTime utcTime = notifyTime;
            double epochDiff = (new DateTime(DefaultYear, DefaultMonth, DefaultDay) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime;
        }
        private void SetForDayAlarm(int week,Calendar calSet,Intent broadcastIntent, int id)
        {
            calSet.Set(CalendarField.DayOfWeek, week);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, id, broadcastIntent, PendingIntentFlags.CancelCurrent);
            AlarmManager am = (AlarmManager)GetSystemService(AlarmService);
            am.SetRepeating(AlarmType.RtcWakeup, calSet.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
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