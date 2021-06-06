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
        public DelmarCoupons coupon;

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
            coupons.Add(new DelmarCoupons { CouponID = "SAMPLE", CouponValue = 50, ExpirationDate = DateTime.Now.Date });
            Coupons = new ObservableCollection<DelmarCoupons>(coupons);
          

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var a = BindingContext;
        }
        private void OnCouponSelected(object sender, EventArgs e)
        {
            coupon = ((e as TappedEventArgs).Parameter as DelmarCoupons);
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}