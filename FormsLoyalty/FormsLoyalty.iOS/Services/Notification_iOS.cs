using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FormsLoyalty.Interfaces;
using FormsLoyalty.iOS.Services;
using Foundation;
using Security;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(Notification_iOS))]
namespace FormsLoyalty.iOS.Services
{
    public class Notification_iOS : INotify
    {
        NSTimer alertDelay;
        UIAlertController alert;
        private static UNUserNotificationCenter notificationCenter;
        string notificationIdentifier = "Local";
        string NotificationType = "MedicineReminder";
        public string getDeviceUuid()
        {
            var query = new SecRecord(SecKind.GenericPassword);
            query.Service = NSBundle.MainBundle.BundleIdentifier;
            query.Account = "PhoneId";

            // get the phoneId
            NSData phoneId = SecKeyChain.QueryAsData(query);

            // if the phoneId doesn't exist, we create it
            if (phoneId == null)
            {
                string model = UIDevice.CurrentDevice.Model; //iPhone  iPad

                if (string.IsNullOrWhiteSpace(model))
                {
                    model = "i?";
                }
                else
                {
                    if (model.Length > 8)
                        model = model.Substring(0, 8);
                }

                string versionString = UIDevice.CurrentDevice.SystemVersion.Replace(",", ".");
                Version osVersion = new Version(versionString);

                model = model + "-iOS" + osVersion.Major.ToString() + "-";

                query.ValueData = model + NSData.FromString(System.Guid.NewGuid().ToString());
                //var result = SecKeyChain.Add(query);
                //if ((result != SecStatusCode.Success) && (result != SecStatusCode.DuplicateItem))
                //    throw new Exception("Cannot store PhoneId");

                Console.WriteLine(query.ValueData.Length.ToString());

                return query.ValueData.ToString();
            }
            return null;
        }

        public string GetImageUri(string encodedImage, string name)
        {

            UIImage image;
            if (encodedImage.Contains("noimage", StringComparison.OrdinalIgnoreCase))
            {
                image = new UIImage("iconLogo.jpg");
            }
            else
            {
                var data = new NSData(encodedImage, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                image = new UIImage(data);
            }

            var documentsDirectory = Environment.GetFolderPath
                                  (Environment.SpecialFolder.Personal);
            string jpgFilename = System.IO.Path.Combine(documentsDirectory, name); // hardcoded filename, overwritten each time
            NSData imgData = image.AsJPEG();
            NSError err = null;
            if (imgData.Save(jpgFilename, false, out err))
            {
                return jpgFilename;
            }
            else
            {
                Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
                return null;
            }
        }

        public void SetReminder(string title, string body, int notificationId, TimeSpan time, DateTime startDate, string days)
        {
            int count = 0;
            if (count>0)
            {
                var timespan = new TimeSpan(count, startDate.Hour, startDate.Minute, 2);
                alertDelay = NSTimer.CreateTimer(timespan, (obj) =>
                {
                    //notificationCenter.RemovePendingNotificationRequests();
                });
            }

            var Id = notificationIdentifier + notificationId;

            var content = new UNMutableNotificationContent();
            content.Title = title;
            content.Body = body;
            content.Sound = UNNotificationSound.Default;
            content.UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(Id), NSObject.FromObject(NotificationType));

            var dateComponents = new NSDateComponents();
            dateComponents.Hour = time.Hours;
            dateComponents.Minute = time.Minutes;
            dateComponents.Calendar = NSCalendar.CurrentCalendar;
            if (!string.IsNullOrEmpty(days))
            {
                foreach (var day in days.Split(","))
                {
                    dateComponents.Weekday = Convert.ToInt32(day);

                    RegisterNotificationRequest(content, dateComponents, Id);
                }
            }
            else
              RegisterNotificationRequest(content, dateComponents, Id);


            //UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        private void RegisterNotificationRequest(UNMutableNotificationContent content, NSDateComponents dateComponents,string uuid)
        {
            

            var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, true);
            var request = UNNotificationRequest.FromIdentifier(uuid, content, trigger);

            // Schedule the request with the system.
            notificationCenter = UNUserNotificationCenter.Current;
            notificationCenter.AddNotificationRequest(request, (error) => { });

            notificationCenter.Delegate = new UserNotificationCenterDelegate();
        }

        public void ShowLocalNotification(string title, string body)
        {
            throw new NotImplementedException();
        }

        public void ShowLocalNotification(string title, string body, int id, DateTime notifyTime)
        {
            throw new NotImplementedException();
        }

        public void ShowSnackBar(string description)
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            //notification.FireDate = NSDate.FromTimeIntervalSinceNow(60);

            // configure the alert
            notification.AlertAction = "View Alert";
            notification.AlertBody = "Your one minute alert has fired!";

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

       
        public void ShowToast(string descripition)
            {
                alertDelay = NSTimer.CreateScheduledTimer(1, (obj) =>
                {
                    dismissMessage();
                });
                alert = UIAlertController.Create(null, descripition, UIAlertControllerStyle.Alert);
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            }
        private void dismissMessage()
            {
                if (alert != null)
                {
                    alert.DismissViewController(true, null);
                }
                if (alertDelay != null)
                {
                    alertDelay.Dispose();
                }
            }

        public void ChangeTabBarFlowDirection(bool rtl)
        {
           
        }

        public void DeleteReminderNotification(int notificationId)
        {
            var Id = notificationIdentifier + notificationId;
            if (string.IsNullOrEmpty(Id))
            {
                return;
            }

            var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
            if (notifications is null)
            {
                return;
            }
            var notificationToDelete = notifications.Where(notification => notification.UserInfo.ContainsKey(NSObject.FromObject(NotificationType)))
                .FirstOrDefault(notification => notification.UserInfo[NotificationType].Equals(NSObject.FromObject(Id)));
            if (notificationToDelete is object)
            {
                UIApplication.SharedApplication.CancelLocalNotification(notificationToDelete);
            }
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