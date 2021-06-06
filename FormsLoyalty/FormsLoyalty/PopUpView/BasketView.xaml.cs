using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Prism.Mvvm;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasketView : PopupPage
    {
        public bool isPlacOrder;

        public event EventHandler CheckoutClicked;
        public BasketView()
        {
            InitializeComponent();

            var moveaAnimation = new MoveAnimation
            {
                PositionIn = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Left,
                PositionOut = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Left,
                DurationIn = 400,
                DurationOut = 300,
                EasingIn = Easing.SinIn,
                EasingOut = Easing.SinOut,
                HasBackgroundAnimation = true
            };

            if (Settings.RTL)
            {
                moveaAnimation.PositionIn = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Left;
                moveaAnimation.PositionOut = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Left;
            }
            else
            {
                moveaAnimation.PositionIn = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Right;
                moveaAnimation.PositionOut = Rg.Plugins.Popup.Enums.MoveAnimationOptions.Right;
            }
            this.Animation = moveaAnimation;

            // Width (in pixels)
            var width = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width;

            // Width (in xamarin.forms units)
            var xamarinWidth = width / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;

              mainView.WidthRequest = xamarinWidth / 1.3; 

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //HomeMasterDetailPage masterDetailPage = this.Parent as HomeMasterDetailPage;
            //var nav = (masterDetailPage.Detail as NavigationPage);
            //nav.CurrentPage.Opacity = 0.5;
            
            LoadBasketItems();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //HomeMasterDetailPage masterDetailPage = this.Parent as HomeMasterDetailPage;
            //var nav = (masterDetailPage.Detail as NavigationPage);
            //nav.CurrentPage.Opacity = 1;
        }

      

        private void LoadBasketItems()
        {
           
                var basketItems = new List<Basket>();

                var items = new ObservableCollection<OneListItem>(AppData.Basket.Items);

                foreach (var basketItem in items)
                {
                    var item = new Basket();
                    item.Id = basketItem.Id;
                    item.ItemDescription = basketItem.ItemDescription;
                    item.VariantDescription = basketItem.VariantDescription;
                    item.Amount = basketItem.Amount;
                    item.ItemId = basketItem.ItemId;
                    item.Quantity = basketItem.Quantity;
                    if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId) == false)
                    {
                        item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId);
                    }
                    else
                    {
                        item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString("N0"));
                    }

                    basketItems.Add(item);
                }


            if (items.Any())
            {
                if (AppData.Basket.State == BasketState.Dirty)
                {
                    totalPrice.Text = "~" + AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.TotalAmount);
                }
                else
                {
                    totalPrice.Text = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.TotalAmount);
                }
               
            }

            basketlist.ItemsSource = basketItems;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await ClosePopUp();
            CheckoutClicked?.Invoke(this, e);
        }

        private async Task ClosePopUp()
        {
            await Navigation.PopAllPopupAsync();
        }

        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            button.Opacity = 0;
            await button.FadeTo(1, 250);

            var response = await DisplayAlert("Alert!!", AppResources.ResourceManager.GetString("BasketViewClearBasketPrompt", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationYes", AppResources.Culture), AppResources.ResourceManager.GetString("ApplicationNo", AppResources.Culture));
            if (response)
            {
               var result = await new BasketModel().ClearBasket();
                if (result)
                {
                    LoadBasketItems();
                    totalPrice.Text = string.Empty;
                }
            }

            button.Opacity = 0;

        }


        Basket selectedBasketItem;
        private async void BasketItem_Tapped(object sender, EventArgs e)
        {
            try
            {
                var view = (Frame)sender;
                view.Opacity = 0;
                await view.FadeTo(1, 250);

                selectedBasketItem = (e as TappedEventArgs).Parameter as Basket;
                var loyitem = await new ItemModel().GetItemById(selectedBasketItem.ItemId);
                if (loyitem != null)
                {
                    var variantDailog = new VariantPopUp(selectedBasketItem.Quantity, onDelete)
                    {
                        BindingContext = loyitem
                    };
                    variantDailog.Disappearing += async (data, e1) =>
                    {
                        if (variantDailog.IsSaved)
                        {
                            var response = await new BasketModel().EditItem(selectedBasketItem.Id, variantDailog.Qty, variantDailog.variantRegistration);
                            if(response)
                             LoadBasketItems();

                        }
                    };
                    await Navigation.PushPopupAsync(variantDailog);
                }
                view.Opacity = 1;
            }
            catch (Exception)
            {

               
            }
        }

       

        private  void onDelete()
        {
            var existinItemIndex = basketlist.ItemsSource.Cast<Basket>().IndexOf(x => x.Id == selectedBasketItem.Id);
            Device.BeginInvokeOnMainThread(async () =>
            {
                await new BasketModel().DeleteItem(selectedBasketItem);
                LoadBasketItems();
            });
           
        }

    }

    public class Basket: OneListItem
    {

        private string _qty;

        public string Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        private string _total;

        public string total
        {
            get { return _total; }
            set { _total = value; }
        }


    }
}