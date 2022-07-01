using FormsLoyalty.PopUpView;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class WishListPage : ContentPage
    {
        WishListPageViewModel _viewModel;
        public WishListPage()
        {
            InitializeComponent();

            _viewModel = BindingContext as WishListPageViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadShoppingList();
        }


        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var frame = (Frame)sender;
            frame.Opacity = 0;
            await frame.FadeTo(1, 250);

           await _viewModel.NavigateToItemPage((e as TappedEventArgs).Parameter as OneListItem);
            frame.Opacity = 0;
        }

        private void moreButton_Clicked(object sender, System.EventArgs e)
        {
            var view = (ImageButton)sender;
            view.BackgroundColor = Color.LightPink;
            var index =  _viewModel.WishList.IndexOf(view.CommandParameter as OneListItem);


            var mainView = ((View)((View)sender).Parent.Parent);
            //var SelectedView = (View)sender;
            var topMargin = index > 0 ? index * mainView.Height : mainView.Height;
            var popup = new MoreOptionsView(new System.Collections.Generic.List<MoreOptionModel> 
            { 
                new MoreOptionModel { OptionName = AppResources.ResourceManager.GetString("ApplicationAddToBasket",AppResources.Culture) },
                new MoreOptionModel { OptionName = AppResources.ResourceManager.GetString("ShoppingListDetailViewDeleteItemFromList",AppResources.Culture) },
            }, topMargin + 10);

            popup.MoreOptionClicked += async(s, e1) =>
            {
                var selectedoption = (e1 as TappedEventArgs).Parameter as MoreOptionModel;
                if (selectedoption.OptionName.Equals(AppResources.ResourceManager.GetString("ApplicationAddToBasket", AppResources.Culture)))
                {
                    await _viewModel.AddItemToBasket(view.CommandParameter as OneListItem);
                }
                else
                {
                    await _viewModel.DeleteShoppingListItem(view.CommandParameter as OneListItem);
                }
            };

            popup.Disappearing += (s, e1) =>
            {
                view.BackgroundColor = Color.Transparent;
            };
            
            PopupNavigation.Instance.PushAsync(popup);
        }

       

      

        }
    }
