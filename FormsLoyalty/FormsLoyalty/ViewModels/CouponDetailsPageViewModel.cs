using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class CouponDetailsPageViewModel : ViewModelBase
    {
        private PublishedOffer _selectedCoupon;
        public PublishedOffer SelectedCoupon
        {
            get { return _selectedCoupon; }
            set { SetProperty(ref _selectedCoupon, value); }
        }
        private ObservableCollection<LoyItem> _relatedItems = new ObservableCollection<LoyItem>();
        public ObservableCollection<LoyItem> relatedItems
        {
            get { return _relatedItems; }
            set { SetProperty(ref _relatedItems, value); }
        }

        #region Command
        public DelegateCommand<ImageView> ShowPreviewCommand => new DelegateCommand<ImageView>(async (data) =>
        {
            if (string.IsNullOrEmpty(data.Image) || data.Image.ToLower().Contains("noimage".ToLower()))
                return;

            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { { "previewImage", data.Image }, { "images", SelectedCoupon.Images } });
        });
        #endregion

        public CouponDetailsPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        /// <summary>
        /// Add or Remove coupon from QrCode
        /// </summary>
        internal void AddRemoveOffer()
        {
            var model = new OfferModel();
            model.ToggleOffer(SelectedCoupon);
        }

        /// <summary>
        /// Load Coupon's releated Items
        /// </summary>
        private void LoadRelatedItems()
        {
            IsPageEnabled = true;
            try
            {
                var service = new ItemService(new LoyItemRepository());

                Task.Run(async () =>
                {
                    var loyItems = await service.GetItemsByPublishedOfferIdAsync(SelectedCoupon.Id, Int32.MaxValue);

                    relatedItems = new ObservableCollection<LoyItem>(loyItems);

                    

                });
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;

        }

        
        

        /// <summary>
        /// This method is used when offer's item is clicked. It navigates to 
        /// Item Page
        /// </summary>
        /// <param name="loyItem"></param>
        /// <returns></returns>
        internal async Task NavigateToItemPage(LoyItem loyItem)
        {
            if (IsPageEnabled)
                return;

            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", loyItem } });
            IsPageEnabled = false;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            SelectedCoupon = parameters.GetValue<PublishedOffer>("coupon");
            LoadRelatedItems();
        }
    }
}
