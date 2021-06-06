using FormsLoyalty.PopUpView;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using Rg.Plugins.Popup.Services;
using System;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadShoppingList();
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
            var mainView = ((View)((View)sender).Parent.Parent).Height;
            var SelectedView = (View)sender;
            var topMargin = SelectedView.Height + ((View)SelectedView.Parent).Height + mainView;
            var moreOptions = new MoreOptionPopUp(new System.Collections.Generic.List<MoreOptionModel> 
            { 
                new MoreOptionModel { OptionName = AppResources.ResourceManager.GetString("ApplicationAddToBasket",AppResources.Culture) },
                new MoreOptionModel { OptionName = AppResources.ResourceManager.GetString("ShoppingListDetailViewDeleteItemFromList",AppResources.Culture) },
            }, topMargin);
            moreOptions.MoreOptionClicked += MoreOptions_MoreOptionClicked;
            PopupNavigation.Instance.PushAsync(moreOptions);
        }

        private async void MoreOptions_MoreOptionClicked(object sender, EventArgs e)
        {
            var data =((View)sender).BindingContext;
            //await _viewModel.ShareItem();
        }
    }
}
