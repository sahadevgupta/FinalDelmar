using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Notifications;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Offers;
using Prism;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.Models
{
    public class NotificationModel : BaseModel
    {
        private NotificationService service;
        public async Task<bool> UpdateNotification(Notification updateNotification, NotificationStatus status)
        {
            return await UpdateNotifications(new List<Notification>() { updateNotification }, status);
        }

        public async Task<bool> UpdateNotifications(List<Notification> updateNotifications, NotificationStatus status)
        {
            bool success = false;


            BeginWsCall();

            var updateNotificationIndexes = new List<int>();

            try
            {
                success = await service.UpdateStatusAsync(AppData.Device.UserLoggedOnToDevice.Id, updateNotifications.Select(x => x.Id).ToList(), status);

                if (success)
                {
                    foreach (var changedNotification in updateNotifications)
                    {
                        foreach (var notification in AppData.Device.UserLoggedOnToDevice.Notifications)
                        {
                            if (notification.Id == changedNotification.Id)
                            {
                                if (status == NotificationStatus.Closed)
                                {
                                    updateNotificationIndexes.Add(AppData.Device.UserLoggedOnToDevice.Notifications.IndexOf(notification));
                                    AppData.Device.UserLoggedOnToDevice.Notifications.Remove(notification);
                                }
                                else
                                {
                                    notification.Status = status;
                                }
                                break;
                            }
                        }
                    }

                    SaveLocalNotifications();




                    if (status == NotificationStatus.Closed)
                    {

                        var action = await MaterialDialog.Instance.SnackbarAsync(message: AppResources.ResourceManager.GetString("ApplicatioItemDeleted", AppResources.Culture),
                                           actionButtonText: AppResources.ResourceManager.GetString("ApplicationUndo", AppResources.Culture),
                                           msDuration: 3000);
                        if (action)
                        {
                            for (int i = 0; i < updateNotifications.Count; i++)
                            {
                                var updateNotification = updateNotifications[i];
                                updateNotification.Status = NotificationStatus.Closed;

                                AppData.Device.UserLoggedOnToDevice.Notifications.Insert(updateNotificationIndexes[i], updateNotification);
                            }

                            await UpdateNotifications(updateNotifications, NotificationStatus.New);
                        }

                       
                    }

                    MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "NotificationStatusChanged");
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }
            return success;
        }

        public async Task GetNotificationsByCardId()
        {
            BeginWsCall();

            try
            {
                var notifications = await service.GetNotificationsAsync(AppData.Device.CardId, Int32.MaxValue);

                AppData.Device.UserLoggedOnToDevice.Notifications = notifications;

                SaveLocalNotifications();

                //SendBroadcast(Utils.BroadcastUtils.NotificationsUpdated); Need to check
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

        }

        public void GetNotificationsByCardId(Action<List<Notification>> onSuccess)
        {
            //from background context

            //BeginWsCall();

            //var looper = Context.MainLooper;
            //var handler = new Handler(looper);
            //handler.Post(async () =>
            //{
            //    try
            //    {
            //        var notifications = await service.GetNotificationsAsync(AppData.Device.CardId, Int32.MaxValue);
            //        if (notifications == null)
            //        {
            //            notifications = new List<Notification>();
            //        }
            //        onSuccess(notifications);
            //    }
            //    catch (Exception)
            //    {
            //        onSuccess(new List<Notification>());
            //    }
            //});
        }

        private void SaveLocalNotifications()
        {
            var worker = new BackgroundWorker();

            var localService = PrismApplicationBase.Current.Container.Resolve<INotificationLocalRepository>();

            worker.DoWork += (sender, args) => localService.SaveNotifications(AppData.Device.UserLoggedOnToDevice.Notifications);
            worker.RunWorkerAsync();
        }

        protected void BeginWsCall()
        {
            service = new NotificationService(new NotificationRepository());
        }
    }
}
