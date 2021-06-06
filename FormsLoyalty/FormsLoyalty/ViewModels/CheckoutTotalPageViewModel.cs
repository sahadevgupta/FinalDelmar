using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Xamarin.Forms;
using FormsLoyalty.PopUpView;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;

namespace FormsLoyalty.ViewModels
{
    public class CheckoutTotalPageViewModel : ViewModelBase
    {
        private ShippingMedhod _shippingMedhod;
        public ShippingMedhod shippingMedhod
        {
            get { return _shippingMedhod; }
            set { SetProperty(ref _shippingMedhod, value); }
        }

        private Address _shippingAddress;
        public Address shippingAddress
        {
            get { return _shippingAddress; }
            set { SetProperty(ref _shippingAddress, value); }
        }

        private ObservableCollection<OneListItem> _basketItems;
        public ObservableCollection<OneListItem> basketItems
        {
            get { return _basketItems; }
            set { SetProperty(ref _basketItems, value); }
        }

        #region Amount

        private string _totalSubtotal;
        public string totalSubtotal
        {
            get { return _totalSubtotal; }
            set { SetProperty(ref _totalSubtotal, value); }
        }

        private string _totalShipping;
        public string totalShipping
        {
            get { return _totalShipping; }
            set { SetProperty(ref _totalShipping, value); }
        }

        private string _totalVAT;
        public string totalVAT
        {
            get { return _totalVAT; }
            set { SetProperty(ref _totalVAT, value); }
        }

        private string _totalDiscount;
        public string totalDiscount
        {
            get { return _totalDiscount; }
            set { SetProperty(ref _totalDiscount, value); }
        }

        private string _totalTotal;
        public string totalTotal
        {
            get { return _totalTotal; }
            set { SetProperty(ref _totalTotal, value); }
        }

        #endregion


        private string _email;
        public string email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        LoyPaymentType paymentType;
        private Address billingAddress;
        private Order basketOrder;


        private string _CardExpiration;
        public string CardExpiration
        {
            get { return _CardExpiration; }
            set { SetProperty(ref _CardExpiration, value); }
        }

        private string _shippingName;
        public string ShippingAddressName
        {
            get { return _shippingName; }
            set { SetProperty(ref _shippingName, value); }
        }

        private string _billingDesc;
        public string billingDesc
        {
            get { return _billingDesc; }
            set { SetProperty(ref _billingDesc, value); }
        }

        private string _billingAddressName;
        public string billingAddressName
        {
            get { return _billingAddressName; }
            set { SetProperty(ref _billingAddressName, value); }
        }

        private string _shippingDesc;
        public string ShippingDesc
        {
            get { return _shippingDesc; }
            set { SetProperty(ref _shippingDesc, value); }
        }

        private string _cardNo;
        public string cardno
        {
            get { return _cardNo; }
            set { SetProperty(ref _cardNo, value); }
        }

        private string _cardDesc;
        public string cardDesc
        {
            get { return _cardDesc; }
            set { SetProperty(ref _cardDesc, value); }
        }

        private DelmarCoupons _selectedCoupon;
        public DelmarCoupons SelectedCoupon
        {
            get { return _selectedCoupon; }
            set 
            { 
                SetProperty(ref _selectedCoupon, value);
                if (value != null)
                {
                    totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount - value.CouponValue);
                }
                else
                    totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount);
            }
        }

        public string CVV { get; private set; }


        public DelegateCommand PlaceOrderCommand { get; set; }
        public DelegateCommand ViewOffersCommand { get; set; }
        public DelegateCommand RemoveCouponCommand { get; set; }

        BasketModel basketModel;
        MemberContactModel memberContactModel;
        public CheckoutTotalPageViewModel(INavigationService navigationService):base(navigationService)
        {
            PlaceOrderCommand = new DelegateCommand(async() => await PlaceOrder());
            ViewOffersCommand = new DelegateCommand(async() => await ShowCouponsPopUpView());
            RemoveCouponCommand = new DelegateCommand(async () => await RemoveCoupon());

            basketModel = new BasketModel();
            memberContactModel = new MemberContactModel();
        }

        private async Task RemoveCoupon()
        {
            SelectedCoupon = null;
            await ShowCouponsPopUpView();
        }

        private async Task ShowCouponsPopUpView()
        {
             var couponsPopUp = new CouponsViewPopUp();
            couponsPopUp.Disappearing += (s, e) =>
            {
                if (couponsPopUp.coupon !=null)
                {
                    SelectedCoupon = couponsPopUp.coupon;
                }
            };
             await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(couponsPopUp, true);
        }

        private async Task PlaceOrder()
        {
            IsPageEnabled = true;
            if (shippingMedhod == ShippingMedhod.ClickCollect)
            {
               await ClickAndCollectOrder();
            }
            else if (shippingMedhod == ShippingMedhod.HomeDelivery)
            {
               await HomeDeliveryOrder();
            }

            IsPageEnabled = false;
        }
        private async Task ClickAndCollectOrder()
        {
            var clickCollectModel = new ClickCollectModel();
            var success = await clickCollectModel.ClickCollectOrderCreate(basketOrder);

            if (success)
            {
                if (success)
                {
                    
                    await memberContactModel.MemberContactGetPointBalance(AppData.Device.CardId);
                    AppData.Basket.Items.Clear();
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
                }
            }
        }
        private async Task HomeDeliveryOrder()
        {
            try
            {
                var success = await basketModel.SendOrder(basketOrder);

                if (success)
                {
                    AppData.Basket.Items = new List<OneListItem>();
                    await memberContactModel.MemberContactGetPointBalance(AppData.Device.CardId);
                    
                    await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
                }
            }
            catch (Exception)
            {

                
            }
            
            
        }

        public void LoadData()
        {
            InitializeBasket();
            CalculateBasket();
            if (shippingMedhod == ShippingMedhod.HomeDelivery)
            {
                ShippingDesc = shippingAddress.FormatAddress;
                     
                if (paymentType == LoyPaymentType.CreditCard)
                {

                    cardDesc = $"{AppResources.ResourceManager.GetString("NotificationViewExpires", AppResources.Culture)} {CardExpiration}";
                    billingDesc = billingAddress.FormatAddress;
                }
            }
            else if (shippingMedhod == ShippingMedhod.ClickCollect)
            {

                email = AppData.Device.UserLoggedOnToDevice.Email;
            }

            

        }

        private  void CalculateBasket()
        {
            try
            {

                Task.Run(async() =>
                {

                    IsPageEnabled = true;

            var oneListModel = new OneListModel();
            basketOrder = await oneListModel.OneListCalculate(AppData.Basket);
                    if (basketOrder == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Error!!", "Unable to connect server", "OK");
                        IsPageEnabled = false;
                        return;
                    }
                basketOrder.Id = Guid.NewGuid().ToString();
                basketOrder.Email = AppData.Device.UserLoggedOnToDevice.Email;
                basketOrder.ContactName = AppData.Device.UserLoggedOnToDevice.Name;
                basketOrder.PhoneNumber = AppData.Device.UserLoggedOnToDevice.Phone;

                if (shippingMedhod == ShippingMedhod.ClickCollect)
                {
                    basketOrder.ClickAndCollectOrder = true;
                }
                else
                {
                    basketOrder.ShipToAddress = shippingAddress;
                    basketOrder.ContactAddress = billingAddress;
                    basketOrder.ShippingAgentServiceCode = "ISP";
                    basketOrder.ClickAndCollectOrder = false;

                    if (paymentType == LoyPaymentType.CreditCard)
                    {
                        basketOrder.OrderPayments.Add(new OrderPayment()
                        {
                            Amount = AppData.Basket.TotalAmount,
                            CardType = "VISA",
                            CurrencyCode = AppData.Device.UserLoggedOnToDevice.Environment.Currency.Id,
                            AuthorizationCode = CVV,
                            CardNumber = cardno,
                            LineNumber = 1,
                            TenderType = ((int)LoyTenderType.Card).ToString(),
                        });
                    }
                    else if (paymentType == LoyPaymentType.PayOnDelivery)
                    {
                        basketOrder.OrderPayments.Add(new OrderPayment()
                        {
                            Amount = AppData.Basket.TotalAmount,
                            CurrencyCode = AppData.Device.UserLoggedOnToDevice.Environment.Currency.Id,
                            LineNumber = 1,
                            TenderType = ((int)LoyTenderType.Cash).ToString(),
                        });
                    }
                }

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        totalSubtotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalNetAmount + basketOrder.TotalDiscount);
                        totalShipping = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.ShippingAmount);
                        totalVAT = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount - basketOrder.TotalNetAmount);
                        totalDiscount = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalDiscount);
                        totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount);
                    });
                    IsPageEnabled = false;
                });
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        void InitializeBasket()
        {
            basketItems = new ObservableCollection<OneListItem>(AppData.Basket.Items);
            LoadImages();
        }

        private void LoadImages()
        {
            try
            {
                Task.Run(async() =>
                {
                    foreach (var item in basketItems)
                    {
                       var imgView = await ImageHelper.GetImageById(item.Image.Id, new ImageSize(396, 396));
                        item.Image.Image = imgView.Image;
                    }
                });
            }
            catch (Exception)
            {

                
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            shippingMedhod = parameters.GetValue<ShippingMedhod>("shipMethod");

            switch (shippingMedhod)
            {
                case ShippingMedhod.ClickCollect:
                    LoadData();
                    break;
                case ShippingMedhod.HomeDelivery:

                    shippingAddress = parameters.GetValue<Address>("ShipAddress");
                    ShippingAddressName = parameters.GetValue<string>("shipName");
                    cardno = parameters.GetValue<string>("cardno");
                    if (cardno == null)
                    {
                        paymentType = LoyPaymentType.PayOnDelivery;
                    }
                    else
                    {
                      billingAddress =  parameters.GetValue<Address>("BillAddress");
                      billingAddressName =  parameters.GetValue<string>("billName");
                      CardExpiration =  parameters.GetValue<string>("cardexp");
                        CVV = parameters.GetValue<string>("cvv");
                        paymentType = LoyPaymentType.CreditCard;
                    }

                    LoadData();
                    break;
                default:
                    break;
            }
        }
    }

    
}
