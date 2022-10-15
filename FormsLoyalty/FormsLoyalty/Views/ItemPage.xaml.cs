using FormsLoyalty.PopUpView;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class ItemPage : ContentPage
    {
        ItemPageViewModel _viewModel;
        public ItemPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as ItemPageViewModel;
        }

        
        
        private async void OnRelatedItemTapped(object sender, System.EventArgs e)
        {
            var view = (StackLayout)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

           await _viewModel.ExecuteChangeSelectedItem((e as TappedEventArgs).Parameter as LoyItem);

            view.Opacity = 1;
        }



        private async void variant_Tapped(object sender, EventArgs e)
        {
            var vairantDailog = new VariantPopUp(Quantity: _viewModel.Quantity)
            {
                BindingContext = _viewModel.Item
            };

            vairantDailog.Disappearing += (data, e1) =>
            {
                if (vairantDailog.IsSaved)
                {
                    _viewModel.changeVariant(vairantDailog.variantRegistration);

                    _viewModel.Quantity = vairantDailog.Qty;
                }
            };

            await Navigation.PushPopupAsync(vairantDailog);
        }

        private async void wishlist_Tapped(object sender, EventArgs e)
        {
            var grid = (Grid)sender;
            grid.Opacity = 0;
            await grid.FadeTo(1, 250);

            await _viewModel.AddRemoveWishList();

            grid.Opacity = 1;
        }

        private void moreOption_Clicked(object sender, EventArgs e)
        {
           
            var topMargin = 20;
            var moreOptions = new MoreOptionPopUp(new System.Collections.Generic.List<MoreOptionModel> { new MoreOptionModel { OptionName="Share" }  }, topMargin);
            moreOptions.MoreOptionClicked += MoreOptions_MoreOptionClicked;
            PopupNavigation.Instance.PushAsync(moreOptions);
        }

        private async void MoreOptions_MoreOptionClicked(object sender, EventArgs e)
        {
           await _viewModel.ShareItem();
        }
    }
}
