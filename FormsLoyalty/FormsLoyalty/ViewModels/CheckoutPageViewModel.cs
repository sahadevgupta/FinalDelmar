using FormsLoyalty.Controls.Stepper;
using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.PopUpView;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using FormsLoyalty.Views.CheckoutStepperView;
using Infrastructure.Data.SQLite.Addresses;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using Prism;
using Prism.Ioc;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class CheckoutPageViewModel : ViewModelBase
    {


        #region Stepper Block

        private Grid _subGrid;
        public Grid SubGrid
        {
            get { return _subGrid; }
            set { SetProperty(ref _subGrid, value); }
        }

        private Grid _mainGrid;
        public Grid MainGrid
        {
            get { return _mainGrid; }
            set { SetProperty(ref _mainGrid, value); }
        }

        private ObservableCollection<StepBarModel> _steps;
        public ObservableCollection<StepBarModel> Steps
        {
            get { return _steps; }
            set { SetProperty(ref _steps, value); }
        }
       
        public bool IsNotLastPage { get; set; }

        #endregion


        #region Address view Block

        #region Shipping Address
        private ObservableCollection<Address> _addresses = new ObservableCollection<Address>();
        public ObservableCollection<Address> addresses
        {
            get { return _addresses; }
            set { SetProperty(ref _addresses, value); }
        }

        private Address _selectedAddress;
        public Address selectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                SetProperty(ref _selectedAddress, value);

                if (value != null && value.LineNO != 0)
                {
                    SetSelectedAddress();
                }
                else
                {
                    shippingAddressName = string.Empty;
                    shippingAddress = new Address();
                    SelectedCity = null;
                    SelectedArea = null;
                    contact = new MemberContact();
                }

            }
        }

       

        private Address _shippingAddress;
        public Address shippingAddress
        {
            get { return _shippingAddress; }
            set { SetProperty(ref _shippingAddress, value); }
        }

        private string _shippingAddressName;
        public string shippingAddressName
        {
            get { return _shippingAddressName; }
            set 
            { 
                SetProperty(ref _shippingAddressName, value); 
            }
        }


       
        private ObservableCollection<AreaModel> _areas = new ObservableCollection<AreaModel>();
        public ObservableCollection<AreaModel> Areas
        {
            get { return _areas; }
            set { SetProperty(ref _areas, value); }
        }

        private AreaModel _selectedArea;
        public AreaModel SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                SetProperty(ref _selectedArea, value);
                if (value != null)
                    shippingAddress.Area = value.Area;
            }
        }


        private ObservableCollection<CitiesModel> _cities = new ObservableCollection<CitiesModel>();
        public ObservableCollection<CitiesModel> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        private CitiesModel _selectedCity;
        public CitiesModel SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                SetProperty(ref _selectedCity, value);
               
                if (value != null)
                {
                    LoadArea();
                    shippingAddress.City = value.City;
                    shippingAddress.Country = value.Country;
                }
            }
        }




        #endregion


        #region Billing Address

        private List<string> _billAddresses = new List<string>();
        public List<string> billAddresses
        {
            get { return _billAddresses; }
            set { SetProperty(ref _billAddresses, value); }
        }

        private string _selectedBillAddress;
        public string selectedBillAddress
        {
            get { return _selectedBillAddress; }
            set
            {
                SetProperty(ref _selectedBillAddress, value);

                if (value != "New" && !string.IsNullOrEmpty(value))
                {
                    billingAddressName = contact.Name;
                    billingAddress = SetBillingAddress();
                }
                else
                {
                    billingAddressName = string.Empty;
                    billingAddress = new Address();
                }

            }
        }

        private Address _billingAddress;
        public Address billingAddress
        {
            get { return _billingAddress; }
            set { SetProperty(ref _billingAddress, value); }
        }

        private string _billingAddressName;
        public string billingAddressName
        {
            get { return _billingAddressName; }
            set { SetProperty(ref _billingAddressName, value); }
        }

        #endregion



        #region Payment

        private string _CardNumber;
        public string CardNumber
        {
            get { return _CardNumber; }
            set { SetProperty(ref _CardNumber, value); }
        }

        private string _CardExpirationDate;
        public string CardExpirationDate
        {
            get { return _CardExpirationDate; }
            set { SetProperty(ref _CardExpirationDate, value); }
        }

        private string _CardCvv;
        public string CardCvv
        {
            get { return _CardCvv; }
            set { SetProperty(ref _CardCvv, value); }
        }

        private string _cardDescription;
        public string CardDesc
        {
            get { return _cardDescription; }
            set { SetProperty(ref _cardDescription, value); }
        }

        #endregion

        private MemberContact _contact;
        public MemberContact contact
        {
            get { return _contact; }
            set { SetProperty(ref _contact, value); }
        }

            
        private bool _isVisaOnDelivery;
        public bool IsVisaOnDelivery
        {
            get { return _isVisaOnDelivery; }
            set { SetProperty(ref _isVisaOnDelivery, value); }
        }


        private bool _isSameAddress;
        internal bool isCreditCard;
        private ShippingMedhod shippingMedhod;

        public bool isSameAddress
        {
            get { return _isSameAddress; }
            set
            {
                SetProperty(ref _isSameAddress, value);
                if (value)
                {
                    billingAddress = shippingAddress;
                }
            }
        }

        #region Validation

        private bool _isNoErrorVisible;
        public bool IsNumberErrorVisible
        {
            get { return _isNoErrorVisible; }
            set { SetProperty(ref _isNoErrorVisible, value); }
        }

        private bool _isNameErrorVisible;
        public bool IsNameErrorVisible
        {
            get { return _isNameErrorVisible; }
            set { SetProperty(ref _isNameErrorVisible, value); }
        }
        private bool _isStreetErrorVisible;
        public bool IsStreetErrorVisible
        {
            get { return _isStreetErrorVisible; }
            set { SetProperty(ref _isStreetErrorVisible, value); }
        }

        private bool _isFloorErrorVisible;
        public bool IsFloorErrorVisible
        {
            get { return _isFloorErrorVisible; }
            set { SetProperty(ref _isFloorErrorVisible, value); }
        }

        private bool _isApartmentErrorVisible;
        public bool IsApartmentErrorVisible
        {
            get { return _isApartmentErrorVisible; }
            set { SetProperty(ref _isApartmentErrorVisible, value); }
        }

        #endregion

        #endregion


        #region Review & Submit Block

        private ObservableCollection<OneListItem> _basketItems;
        public ObservableCollection<OneListItem> basketItems
        {
            get { return _basketItems; }
            set { SetProperty(ref _basketItems, value); }
        }

        private string _email;
        public string email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }


        private string _shippingAddressDesc;
        public string ShippingAddressDesc
        {
            get { return _shippingAddressDesc; }
            set { SetProperty(ref _shippingAddressDesc, value); }
        }

        private string _billingingAddressDesc;
        public string BillingingAddressDesc
        {
            get { return _billingingAddressDesc; }
            set { SetProperty(ref _billingingAddressDesc, value); }
        }

        private Order basketOrder;


        
        

        private DelmarCoupons _selectedCoupon;
        public DelmarCoupons SelectedCoupon
        {
            get { return _selectedCoupon; }
            set
            {
                SetProperty(ref _selectedCoupon, value);
                if (value != null)
                {

                    basketOrder.CouponNo = SelectedCoupon.CouponID;
                    totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount - (decimal)value.CouponValue);
                }
                else
                    totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount);

                
            }
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
        #endregion


       
        BasketModel basketModel;
        MemberContactModel memberContactModel;

       
        public bool IsNewAddresAdded { get; private set; }

        public CheckoutPageViewModel(INavigationService navigationService): base(navigationService)
        {
          
            Steps = new ObservableCollection<StepBarModel>()
            {
                new StepBarModel()
                { 
                    StepName=AppResources.txtAddressDetails,
                    Status=StepBarStatus.InProgress,
                    IsNotLast=true,
                    
                    MainContent=new AddressDetailsView(this),IsCurrentContent=true,
                    
                },
                new StepBarModel()
                { 
                    StepName=AppResources.ActionbarCheckout,
                    Status=StepBarStatus.Pending,
                    
                    IsNotLast=false,MainContent=new ReviewSubmitView(this),
                    IsCurrentContent=false
                },
            };
            StepListCount = Steps.Count;
            
            basketModel = new BasketModel();
            memberContactModel = new MemberContactModel();

           


            LoadData();
           

            

        }

        

        void LoadData()
        {
            Task.Run(() =>
            {
                LoadAddressView();
                LoadBasket();
                CalculateBasket();
            });
        }

       

      


        #region Stepper Module

        public void AddContentForSelectedStep()
        {
            if (Steps.FirstOrDefault(x => x.IsCurrentContent) != null)
            {
                ContentView content = Steps.FirstOrDefault(x => x.IsCurrentContent).MainContent;
                bool isNotLast = Steps.FirstOrDefault(x => x.IsCurrentContent).IsNotLast;
                if (content != null)
                {
                    if (SubGrid.Children.Count >= 1)
                    {
                        for (int i = 0; i < SubGrid.Children.Count; i++)
                        {
                            SubGrid.Children.RemoveAt(i);
                        }
                    }

                    SubGrid.Children.Add(content, 0, 0);
                }
                IsNotLastPage = isNotLast;
            }
        }


        public override void OnStepTapped(StepBarModel stepBarModel)
        {
            base.OnStepTapped(stepBarModel);
            NavigateToBackStep();
        }
        public void NavigateToBackStep()
        {
            
                int index = Steps.IndexOf(Steps.LastOrDefault(x => x.IsCurrentContent));
                if (index > 0)
                {
                    StepBarModel step = Steps.ElementAt(index);
                    step.Status = StepBarStatus.Pending;
                    step.IsCurrentContent = false;
                    Steps[index] = step;
                    if ((index - 1) > 0)
                    {
                        StepBarModel stepnext = Steps.ElementAt(index - 1);
                        if (stepnext.Status == StepBarStatus.Completed)
                        {
                            stepnext.Status = StepBarStatus.InProgress;
                        }
                        stepnext.IsCurrentContent = true;
                        Steps[index - 1] = stepnext;
                    }
                    else
                    {
                        StepBarModel stepnext = Steps.ElementAt(0);

                        stepnext.Status = StepBarStatus.InProgress;

                        stepnext.IsCurrentContent = true;
                        Steps[index - 1] = stepnext;
                    }

                    StepBarComponent stepbar = new StepBarComponent(this);
                    int indexforstepper = MainGrid.Children.IndexOf(MainGrid.Children.LastOrDefault(x => x.GetType() == typeof(StepBarComponent)));
                    MainGrid.Children.RemoveAt(indexforstepper);
                    MainGrid.Children.Add(stepbar, 0, 0);
                }
                AddContentForSelectedStep();
            
        }


        public void NavigateToNextStep()
        {
            if (ValidateFields())
            {
                

                if (isCreditCard && string.IsNullOrEmpty(CardNumber))
                  CardDesc = $"{AppResources.ResourceManager.GetString("NotificationViewExpires", AppResources.Culture)} {CardExpirationDate}";

                ShippingAddressDesc = shippingAddress.FormatAddress;
                BillingingAddressDesc = billingAddress?.FormatAddress;

                AddNewAddressToServer();

                int index = Steps.IndexOf(Steps.LastOrDefault(x => x.IsCurrentContent));
                    if (index < Steps.Count)
                    {
                        StepBarModel step = Steps.ElementAt(index);
                        step.Status = StepBarStatus.Completed;
                        step.IsCurrentContent = false;
                        Steps[index] = step;
                        if ((index + 1) < Steps.Count)
                        {
                            StepBarModel stepnext = Steps.ElementAt(index + 1);
                            if (stepnext.Status == StepBarStatus.Pending)
                            {
                                stepnext.Status = StepBarStatus.InProgress;
                            }
                            stepnext.IsCurrentContent = true;
                            Steps[index + 1] = stepnext;
                        }

                        StepBarComponent stepbar = new StepBarComponent(this);
                        int indexforstepper = MainGrid.Children.IndexOf(MainGrid.Children.FirstOrDefault(x => x.GetType() == typeof(StepBarComponent)));
                    MainGrid.Children.RemoveAt(indexforstepper);
                        
                    MainGrid.Children.Add(stepbar, 0, 0);
                    }
                    AddContentForSelectedStep();
                }
            
        }

        private void AddNewAddressToServer()
        {
            if (selectedAddress.LineNO == 0 && !IsNewAddresAdded)
            {
                var addressRepo = PrismApplicationBase.Current.Container.Resolve<IAddressRepository>();

                addressRepo.SaveAddress(shippingAddress, AppData.Device.UserLoggedOnToDevice.Id);
                Task.Run(async () =>
                {
                    try
                    {
                        var lineNo = await memberContactModel.AddNewAddressAsync(AppData.Device.CardId, shippingAddress);
                        if (lineNo > 0)
                        {
                            shippingAddress.LineNO = lineNo;
                            AppData.Device.UserLoggedOnToDevice.Addresses.Add(shippingAddress);
                            IsNewAddresAdded = true;
                        }

                    }
                    catch (Exception)
                    {


                    }
                });
            }
        }

        private bool ValidateFields()
        {
            bool IsSucess = true;

            if (string.IsNullOrEmpty(shippingAddressName))
            {
                IsNameErrorVisible = true;
                IsSucess = false;
            }

            if (SelectedCity == null || string.IsNullOrEmpty(SelectedCity?.City))
            {
                IsSucess = false;
            }
            if (SelectedArea == null || string.IsNullOrEmpty(SelectedArea?.Area))
            {
                IsSucess = false;
            }
            if (string.IsNullOrEmpty(shippingAddress.Number))
            {
                IsNumberErrorVisible = true;
                IsSucess = false;
            }
            if (string.IsNullOrEmpty(shippingAddress.Street))
            {
                IsStreetErrorVisible = true;
                IsSucess = false;
            }
            if (string.IsNullOrEmpty(shippingAddress.FloorNo))
            {
                IsFloorErrorVisible = true;
                IsSucess = false;
            }
            if (string.IsNullOrEmpty(shippingAddress.ApartmentNo))
            {
                IsApartmentErrorVisible = true;
                IsSucess = false;
            }
            if (string.IsNullOrEmpty(shippingAddressName) || string.IsNullOrEmpty(SelectedCity?.City) || string.IsNullOrEmpty(SelectedArea?.Area))
            {
                DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewAllRequiredFieldsMustBeFilled", AppResources.Culture));
                IsSucess = false;
            }

            if (isCreditCard)
            {
                if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardExpirationDate) || string.IsNullOrEmpty(CardCvv))
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewMustEnterCreditCardInfo", AppResources.Culture));
                    IsSucess = false;
                }

                if (!isSameAddress)
                {
                    if (string.IsNullOrEmpty(billingAddressName) || string.IsNullOrEmpty(billingAddress.Address1))
                    {
                        DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewAllRequiredFieldsMustBeFilled", AppResources.Culture));
                        IsSucess = false;
                    }
                }
            }

            return IsSucess;
        }


        #endregion


        #region Checkout Address View Data

        private void LoadAddressView()
        {
            Task.Run(async () =>
            {
                Cities = new ObservableCollection<CitiesModel>(await new CommonModel().GetCitiessync());
                SelectedCity = Cities.FirstOrDefault(x => x.City.Equals(selectedAddress.City));
            });

            if (AppData.Device.UserLoggedOnToDevice.Addresses != null)
            {
                foreach (var item in AppData.Device.UserLoggedOnToDevice.Addresses)
                {
                    item.Address1 = item.FormatAddress;
                }

                addresses = new ObservableCollection<Address>(AppData.Device.UserLoggedOnToDevice.Addresses);
            }
            addresses.Add(new Address { Address1 = AppResources.txtEnterNewAddress });

            selectedAddress = addresses.First();
        }


        void LoadArea()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Areas = new ObservableCollection<AreaModel>(await new CommonModel().GetAreasAsync(SelectedCity.City));
                SelectedArea = Areas.FirstOrDefault(x => x.Area.Equals(selectedAddress.Area));
            });
        }

        private void SetSelectedCityArea()
        {
            if (!Cities.Any() || !Areas.Any())
            {
                Task.Run(async () =>
                {
                    Cities = new ObservableCollection<CitiesModel>(await new CommonModel().GetCitiessync());
                    //Areas = new ObservableCollection<AreaModel>(await new CommonModel().GetAreasAsync("cairo"));

                    SelectedCity = Cities.FirstOrDefault(x => x.City.Equals(selectedAddress.City));
                    //SelectedArea = Areas.FirstOrDefault(x => x.Area.Equals(selectedAddress.Area));
                });
            }
             else
            {
                SelectedCity = Cities.FirstOrDefault(x => x.City.Equals(selectedAddress.City));
                //SelectedArea = Areas.FirstOrDefault(x => x.Area.Equals(selectedAddress.Area));
            }
            

           
        }

        private Address SetBillingAddress()
        {
            if (contact.Addresses != null && contact.Addresses.Count > 0)
            {
                var address = new Address
                {
                    Address1 = contact.Addresses[0].Address1,
                    Address2 = contact.Addresses[0].Address2,
                    City = contact.Addresses[0].City,
                    StateProvinceRegion = contact.Addresses[0].StateProvinceRegion,
                    PostCode = contact.Addresses[0].PostCode,
                    Country = contact.Addresses[0].Country


                };
                return address;
            }
            return new Address();
        }

        private void SetSelectedAddress()
        {
            contact = AppData.Device.UserLoggedOnToDevice;
            shippingAddressName = contact.Name;
            shippingAddress = selectedAddress;
            SetSelectedCityArea();
            IsNewAddresAdded = false;
        }

        #endregion



        #region Checkout Review & Submit View Data

        private void LoadBasket()
        {
            basketItems = new ObservableCollection<OneListItem>();
            try
            {
                foreach (var basketItem in AppData.CartItems)
                {
                    //if (string.IsNullOrEmpty(basketItem.UnitOfMeasureId) == false)
                    //{
                    //    basketItem.= string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString() + " " + basketItem.UnitOfMeasureId);
                    //}
                    //else
                    //{
                    //    item.Qty = string.Format(AppResources.ResourceManager.GetString("ApplicationQtyN", AppResources.Culture), basketItem.Quantity.ToString("N0"));
                    //}

                    basketItem.DiscountAmount = basketItem.DiscountAmount * basketItem.Quantity;

                    basketItems.Add(basketItem);
                }

            }
            catch (Exception)
            {

                
            }

        }

        private void CalculateBasket()
        {
            try
            {
                Task.Run(async () =>
                {

                    // IsPageEnabled = true;

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

                    AddAddressAndPaymentDetailToOrder();

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {


                        totalSubtotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalNetAmount + basketOrder.TotalDiscount);
                        totalShipping = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(AppData.Basket.ShippingAmount);
                        totalVAT = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount - basketOrder.TotalNetAmount);
                        totalDiscount = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalDiscount);
                        totalTotal = AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketOrder.TotalAmount);
                    });
                    // IsPageEnabled = false;
                });

            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        private void AddAddressAndPaymentDetailToOrder()
        {
            basketOrder.ShipToAddress = shippingAddress;
            basketOrder.ContactAddress = billingAddress;
            basketOrder.ShippingAgentServiceCode = "ISP";
            basketOrder.ClickAndCollectOrder = false;

            if (isCreditCard && !string.IsNullOrEmpty(CardNumber))
            {
                basketOrder.OrderPayments.Add(new OrderPayment()
                {
                    Amount = AppData.Basket.TotalAmount,
                    CardType = "VISA",
                    CurrencyCode = AppData.Device.UserLoggedOnToDevice.Environment.Currency.Id,
                    AuthorizationCode = CardCvv,
                    CardNumber = CardNumber,
                    LineNumber = 1,
                    TenderType = ((int)LoyTenderType.Card).ToString(),
                });
            }
            else
            {
                var payment = new OrderPayment()
                {
                    Amount = AppData.Basket.TotalAmount,
                    CurrencyCode = AppData.Device.UserLoggedOnToDevice.Environment.Currency.Id,
                    LineNumber = 1,

                };


                if (IsVisaOnDelivery)
                {
                    payment.TenderType = ((int)LoyTenderType.Visa_On_Delivery).ToString();
                }
                else
                {
                    payment.TenderType = ((int)LoyTenderType.Cash).ToString();
                }

                basketOrder.OrderPayments.Add(payment);
            }
        }

        public async Task RemoveCoupon()
        {
            SelectedCoupon = null;
            await ShowCouponsPopUpView();
        }

        public async Task ShowCouponsPopUpView()
        {
            var couponsPopUp = new CouponsViewPopUp();
            couponsPopUp.Disappearing += (s, e) =>
            {
                if (couponsPopUp.coupon != null)
                {
                    SelectedCoupon = couponsPopUp.coupon;
                }
            };
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(couponsPopUp, true);
        }

       
        public async Task PlaceOrder()
        {
            IsPageEnabled = true;
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
            finally
            {
                IsPageEnabled = false;
            }
        }


        #endregion


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

           
        }

        
    }
}
