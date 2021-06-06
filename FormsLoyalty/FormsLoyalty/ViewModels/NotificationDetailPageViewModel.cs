using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class NotificationDetailPageViewModel : ViewModelBase
    {
        private Notification _selectedNotification;
        public Notification SelectedNotification
        {
            get { return _selectedNotification; }
            set { SetProperty(ref _selectedNotification, value); }
        }

        private string _expirationDate;
        public string expirationDate
        {
            get { return _expirationDate; }
            set { SetProperty(ref _expirationDate, value); }
        }


        #region Command
        public DelegateCommand<ImageView> ShowPreviewCommand => new DelegateCommand<ImageView>(async (data) =>
        {
            if (string.IsNullOrEmpty(data.Image) || data.Image.ToLower().Contains("noimage".ToLower()))
                return;

            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { { "previewImage", data.Image }, { "images", SelectedNotification.Images } });
        });
        #endregion

        public NotificationDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        /// <summary>
        /// Get Notification Image
        /// </summary>
        /// <param name="image"></param>
       internal async void LoadImage(ImageView image)
        {
            try
            {
                var imageview = await ImageHelper.GetImageById(image.Id, new ImageSize(396, 396));
                image.Image = imageview.Image;
            }
            catch (Exception)
            {

                
            }
           
        }

        /// <summary>
        /// Update Notification as Read When the notification is opened
        /// </summary>
        private void UpdateNotification()
        {
            var notificationModel = new NotificationModel();
            Task.Run(async() =>
            {
                await notificationModel.UpdateNotification(SelectedNotification, NotificationStatus.Read);
            });
            
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);



            SelectedNotification = parameters.GetValue<Notification>("notification");
            if (SelectedNotification.ExpiryDate.HasValue)
                expirationDate = string.Format(AppResources.ResourceManager.GetString("DetailViewExpires",AppResources.Culture), SelectedNotification.ExpiryDate.Value.ToShortTimeString(), SelectedNotification.ExpiryDate.Value.ToShortDateString());
            else
                expirationDate = AppResources.ResourceManager.GetString("DetailViewNeverExpires",AppResources.Culture);

            UpdateNotification();
        }

       
    }
}
