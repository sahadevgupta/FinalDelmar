using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.Models;

namespace FormsLoyalty.ViewModels
{
    public class NotificationsPageViewModel : ViewModelBase
    {
        private ObservableCollection<NotificationGroup> _notifications;
        public ObservableCollection<NotificationGroup> notifications
        {
            get { return _notifications; }
            set { SetProperty(ref _notifications, value); }
        }

        private int _count =2;
        public int count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        public ICommand ListMenuCommand => new Command<MaterialMenuResult>(async (s) => await ListMenuSelected(s));

      

        public NotificationsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            
           
        }

       /// <summary>
       /// This method is used to update notification status or delete it.
       /// </summary>
       /// <param name="result"></param>
       /// <returns></returns>
        private async Task ListMenuSelected(MaterialMenuResult result)
        {
            if (result.Index == -1)
            {
                return;
            }

            IsPageEnabled = true;
            try
            {
                var notification = result.Parameter as Notification;
                if (result.Index == 0)
                {

                    if (notification.Status == NotificationStatus.New)
                    {
                        await UpdateNotificationStatus(notification, NotificationStatus.Read);

                       
                    }
                    else
                        await UpdateNotificationStatus(notification, NotificationStatus.New);
                }
                else
                    await UpdateNotificationStatus(notification, NotificationStatus.Closed);

                
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        /// <summary>
        /// This method used to update notification by considering the following params
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task UpdateNotificationStatus(Notification notification, NotificationStatus status)
        {
            IsPageEnabled = true;
            try
            {
                var model = new NotificationModel();
                var success = await model.UpdateNotification(notification, status);

                if (success)
                {
                    LoadNotifications();
                }
            }
            catch (Exception)
            {
                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }


        /// <summary>
        /// This method navigates to detail page
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        internal async Task GoToDetailPage(Notification notification)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(NotificationDetailPage), new NavigationParameters { { "notification", notification } });
            IsPageEnabled = false;
        }

        /// <summary>
        /// This method is used to load notifications
        /// </summary>
        internal void LoadNotifications()
        {
            IsPageEnabled = true;
            try
            {

           
            
            var temp = new ObservableCollection<NotificationGroup>();

            var unread = AppData.Device.UserLoggedOnToDevice.Notifications.Where(x => x.Status == NotificationStatus.New).ToList();
            var read = AppData.Device.UserLoggedOnToDevice.Notifications.Where(x => x.Status == NotificationStatus.Read).ToList();

            if (unread.Any())
            {
                temp.Add(new NotificationGroup(
                AppResources.ResourceManager.GetString("NotificationViewUnread", AppResources.Culture),
                new List<Notification>(LoadWithImage(unread))

            ));
            }

            if (read.Any())
            {
                temp.Add(new NotificationGroup(
                AppResources.ResourceManager.GetString("NotificationViewRead", AppResources.Culture),
                new List<Notification>(LoadWithImage(read))

            ));
            }
            notifications = new ObservableCollection<NotificationGroup>(temp);
            }
            catch (Exception)
            {


            }
            finally
            {
                IsPageEnabled = false;
            }
           
        }

        /// <summary>
        /// This method is used to load notification image
        /// </summary>
        /// <param name="notification"></param>
        /// <returns>Notifcation with image</returns>
        private IEnumerable<Notification> LoadWithImage(List<Notification> notifications)
        {
            try
            {
                Task.Run(async () =>
                {
                foreach (var notification in notifications)
                {
                    if (notification.Images.Count > 0)
                    {
                        
                            var imageView = await ImageHelper.GetImageById(notification.Images[0].Id, new ImageSize(396, 396));
                            notification.Images[0].Image = imageView.Image;
                       
                    }
                    else
                            notification.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                    }
                });
            }
            catch (Exception)
            {

                
            }
            return notifications;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
           
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var navigationMode = parameters.GetNavigationMode();
            switch (navigationMode)
            {
                case NavigationMode.Back:
                    LoadNotifications();
                    break;
                case NavigationMode.New:
                    break;
                case NavigationMode.Forward:
                    break;
                case NavigationMode.Refresh:
                    break;
                default:
                    break;
            }

        }

    }

    /// <summary>
    /// This class is created to display notication in group based on their Notification Status
    /// </summary>
    public class NotificationGroup : List<Notification>
    {
        public string Name { get; private set; }

        public NotificationGroup(string name, List<Notification> notification) : base(notification)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
