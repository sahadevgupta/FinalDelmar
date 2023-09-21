using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class CouponsPageViewModel : ViewModelBase
    {

        private ObservableCollection<PublishedOffer> _coupons;
        public ObservableCollection<PublishedOffer> Coupons
        {
            get { return _coupons; }
            set { SetProperty(ref _coupons, value); }
        }

        private ObservableCollection<DelmarCoupons> _delmarCoupons;
        public ObservableCollection<DelmarCoupons> DelmarCoupons
        {
            get { return _delmarCoupons; }
            set { SetProperty(ref _delmarCoupons, value); }
        }

        private int _spanCount = 2;
        public int SpanCount
        {
            get { return _spanCount; }
            set { SetProperty(ref _spanCount, value); }
        }

        public CouponsPageViewModel(INavigationService navigationService): base(navigationService)
        {
            Task.Run(async () =>
            {
                await LoadCoupons();
            });
        }


       

        /// <summary>
        /// Loaad Offer
        /// </summary>
        internal async Task LoadCoupons()
        {

            IsPageEnabled = true;

            if (AppData.Device.UserLoggedOnToDevice == null)
            {
                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
                return;
            }

            var publishedOffers = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code == OfferDiscountType.Coupon);
            Coupons = new ObservableCollection<PublishedOffer>(publishedOffers);

            var coupons = await new CommonModel().GetDelmarCouponAsync(AppData.Device.UserLoggedOnToDevice.Account.Id);
            DelmarCoupons = new ObservableCollection<DelmarCoupons>(coupons);

            IsPageEnabled = false;

        }

        /// <summary>
        /// Get Offers Image
        /// </summary>
        /// <param name="Offfers"></param>
        /// <returns></returns>
        
        internal async Task NavigateToCouponDetail(PublishedOffer coupon)
        {
            if (IsPageEnabled)
                return;

            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(CouponDetailsPage), new NavigationParameters { { "coupon", coupon } });
            IsPageEnabled = false;
        }

        /// <summary>
        /// Add or Remove Coupon from QrCode
        /// </summary>
        /// <param name="publishedOffer"></param>
        internal void AddRemoveCouponFromQrCode(PublishedOffer coupon)
        {
            if (IsPageEnabled)
                return;

            IsPageEnabled = true;
            var model = new OfferModel();
            model.ToggleOffer(coupon);
            IsPageEnabled = false;
        }
    }
}
