using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CouponsViewPopUp : PopupPage
    {
        internal DelmarCoupons coupon;

        private ObservableCollection<DelmarCoupons> _coupons;

        public ObservableCollection<DelmarCoupons> Coupons
        {
            get { return _coupons; }
            set { _coupons = value; }
        }


        public CouponsViewPopUp()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () =>
            {
                await LoadCoupons();
                BindingContext = this;
            });
            
        }
        internal async Task LoadCoupons()
        {
            var coupons = await new CommonModel().GetDelmarCouponAsync(AppData.Device.UserLoggedOnToDevice.Account.Id);
            Coupons = new ObservableCollection<DelmarCoupons>();
            foreach (var delmarCoupon in coupons)
            {
                if (!delmarCoupon.Blocked && delmarCoupon.RedeemedAmount== 0 && string.IsNullOrEmpty(delmarCoupon.CustomerOrderNo))
                {
                    Coupons.Add(delmarCoupon);
                }
            }

            if (coupons.Any())
            {
                noitemlbl.IsVisible = false;
            }
            else
                noitemlbl.IsVisible = true;



        }
        
        private async void OnCouponSelected(object sender, EventArgs e)
        {
            var selectedCoupom = ((e as TappedEventArgs).Parameter as DelmarCoupons);

            var dataDate = selectedCoupom.ExpirationDate.ToString("dd/MM/yyyy");
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy");

            var response = dataDate.Equals(currentDate);
            if (response)
            {
               await DisplayAlert("Alert!!", "Cannot add expired coupon", "OK");
                return;
            }
            else
            {
                coupon = selectedCoupom;
            }

           await PopupNavigation.Instance.PopAsync(true);
        }
    }
}