using FormsLoyalty.Helpers;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using System.Collections.Generic;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using Device = Xamarin.Forms.Device;
namespace FormsLoyalty.Views
{
    public partial class NotificationsPage : ContentPage
    {
        NotificationsPageViewModel _viewModel;
        public NotificationsPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as NotificationsPageViewModel;
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadNotifications();
            ChangeToolbarIcon();

        }

        private void ChangeToolbarIcon()
        {
            try
            {
                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    offerlist.ItemTemplate = (DataTemplate)Resources["ListView"];
                }
            }
            catch (System.Exception)
            {

                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = "ic_view_list_white_24dp";
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = "ic_view_module_white_24dp";
                    offerlist.ItemTemplate = (DataTemplate)Resources["ListView"];
                }
            }
           
        }

        private async void Notification_Tapped(object sender, System.EventArgs e)
        {
            var grid = (Grid)sender;
             grid.Opacity = 0;
            await grid.FadeTo(1, 250);

           await _viewModel.GoToDetailPage((e as TappedEventArgs).Parameter as Notification);
            grid.Opacity = 1;
        }


        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Settings.ShowCard = !Settings.ShowCard;
            ChangeToolbarIcon();

        }

        private void MaterialMenuButton_Clicked(object sender, System.EventArgs e)
        {
            var moreBtn = ((MaterialMenuButton)sender);
            var SelelctedNotification = moreBtn.CommandParameter as Notification;

            switch (SelelctedNotification.Status)
            {
                case NotificationStatus.New:
                    moreBtn.Choices = new List<string>
                    {
                        AppResources.ResourceManager.GetString("MenuViewMarkAsReadTitle",AppResources.Culture),
                        AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)
                    };

                    
                    break;
                case NotificationStatus.Read:
                    moreBtn.Choices = new List<string>
                    {
                        AppResources.ResourceManager.GetString("MenuViewMarkAsUnreadTitle",AppResources.Culture),
                        AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)
                    };
                    break;
                case NotificationStatus.Closed:
                    break;
                default:
                    break;
            }

           
        }
    }
}
