using FormsLoyalty.Helpers;
using FormsLoyalty.PopUpView;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Extensions;
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
                    viewtoolbar.IconImageSource = new FontImageSource()
                    {
                        FontFamily = "FontAwesome",
                        Glyph = FormsLoyalty.Resources.FontAwesomeIcons.List,
                        Size = Xamarin.Forms.Device.GetNamedSize(NamedSize.Large, viewtoolbar)
                    };
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = new FontImageSource()
                    {
                        FontFamily = "FontAwesome",
                        Glyph = FormsLoyalty.Resources.FontAwesomeIcons.Th,
                        Size = Xamarin.Forms.Device.GetNamedSize(NamedSize.Large, viewtoolbar)
                    };
                    offerlist.ItemTemplate = (DataTemplate)Resources["ListView"];
                }
            }
            catch (System.Exception)
            {

                if (Settings.ShowCard)
                {
                    viewtoolbar.IconImageSource = new FontImageSource()
                    {
                        FontFamily = "FontAwesome",
                        Glyph = FormsLoyalty.Resources.FontAwesomeIcons.List,
                        Size = Xamarin.Forms.Device.GetNamedSize(NamedSize.Large, viewtoolbar)
                    };
                    _viewModel.count = 2;
                    offerlist.ItemTemplate = (DataTemplate)Resources["CardView"];
                }
                else
                {
                    _viewModel.count = 1;
                    viewtoolbar.IconImageSource = new FontImageSource()
                    {
                        FontFamily = "FontAwesome",
                        Glyph = FormsLoyalty.Resources.FontAwesomeIcons.Th,
                        Size = Xamarin.Forms.Device.GetNamedSize(NamedSize.Large, viewtoolbar)
                    };
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
            var moreBtn = (MaterialMenuButton)sender;
            var SelelctedNotification = moreBtn.CommandParameter as Notification;
            var choices = new System.Collections.Generic.List<MoreOptionModel>();
            switch (SelelctedNotification.Status)
            {
                case NotificationStatus.New:
                    choices = new List<MoreOptionModel>
                    {
                        new MoreOptionModel
                        {
                             OptionName =  AppResources.ResourceManager.GetString("MenuViewMarkAsReadTitle",AppResources.Culture)
                        },
                        new MoreOptionModel
                        {
                            OptionName = AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)

                        }
                    };

                    
                    break;
                case NotificationStatus.Read:

                    choices = new List<MoreOptionModel>
                    {
                        new MoreOptionModel
                        {
                             OptionName =  AppResources.ResourceManager.GetString("MenuViewMarkAsUnreadTitle",AppResources.Culture)
                        },
                        new MoreOptionModel
                        {
                            OptionName = AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)

                        }
                    };

                    break;
                case NotificationStatus.Closed:
                    break;
                default:
                    break;
            }
            moreBtn.Choices = choices;


        }


        private void moreButton_Clicked(object sender, System.EventArgs e)
        {
            var view = (ImageButton)sender;
            var SelelctedNotification = view.CommandParameter as Notification;
            view.BackgroundColor = Color.LightPink;
            var index = _viewModel.notifications.IndexOf(view.BindingContext as NotificationGroup);
            var choices = new System.Collections.Generic.List<MoreOptionModel>();


            var mainView = ((View)((View)sender).Parent.Parent);
            var topMargin = index > 0 ? index * mainView.Height : mainView.Height;

            switch (SelelctedNotification.Status)
            {
                case NotificationStatus.New:
                    choices = new List<MoreOptionModel>
                    {
                        new MoreOptionModel
                        {
                             OptionName =  AppResources.ResourceManager.GetString("MenuViewMarkAsReadTitle",AppResources.Culture)
                        },
                        new MoreOptionModel
                        {
                            OptionName = AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)

                        }
                    };


                    break;
                case NotificationStatus.Read:

                    choices = new List<MoreOptionModel>
                    {
                        new MoreOptionModel
                        {
                             OptionName =  AppResources.ResourceManager.GetString("MenuViewMarkAsUnreadTitle",AppResources.Culture)
                        },
                        new MoreOptionModel
                        {
                            OptionName = AppResources.ResourceManager.GetString("MenuViewDeleteTitle",AppResources.Culture)

                        }
                    };

                    break;
                case NotificationStatus.Closed:
                    break;
                default:
                    break;
            }

            var popup = new MoreOptionsView(choices, topMargin + 10);

            popup.MoreOptionClicked += async (s, e1) =>
            {
                var selectedoption = (e1 as TappedEventArgs).Parameter as MoreOptionModel;
                await _viewModel.ListMenuSelected(selectedoption, SelelctedNotification);
            };

            popup.Disappearing += (s, e1) =>
            {
                view.BackgroundColor = Color.Transparent;
            };

            PopupNavigation.Instance.PushAsync(popup);
        }

    }
}
