using Infrastructure.Data.SQLite.Addresses;
using Prism;
using Prism.Ioc;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using FormsLoyalty.Utils;
using Xamarin.Forms;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using System.Threading.Tasks;
using FormsLoyalty.Views;
using FormsLoyalty.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FormsLoyalty.Models;
using LSRetail.Omni.Domain.DataModel.Loyalty.Address;

namespace FormsLoyalty.ViewModels
{
    public class CheckoutShippingPageViewModel : ViewModelBase
    {

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

                if (value!=null && value.LineNO != 0 )
                {
                    LoadMemberContact();
                }
                else
                {
                    shippingAddressName = string.Empty;
                    shippingAddress = new Address();
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
            set { SetProperty(ref _shippingAddressName, value); }
        }


        #region Updated Address Section
        private ObservableCollection<AreaModel> _areas;
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


        private ObservableCollection<CitiesModel> _cities;
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
                    Task.Run(async () => Areas = new ObservableCollection<AreaModel>(await new CommonModel().GetAreasAsync("cairo")));

                    shippingAddress.City = value.City;
                    shippingAddress.Country = value.Country;
                }
            }
        }

        #endregion


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
                    billingAddress = LoadAddress();
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

        #endregion

        private MemberContact _contact;
        public MemberContact contact
        {
            get { return _contact; }
            set { SetProperty(ref _contact, value); }
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


        public DelegateCommand NextCommand { get; set; }
        public bool IsBackNavigation { get; private set; }

        public CheckoutShippingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            NextCommand = new DelegateCommand(async()=> await Next());
        }

        private async Task Next()
        {
            

            if (ValidateData())
            {
                if (selectedAddress.LineNO == 0 && !IsBackNavigation)
                {
                    var addressRepo = PrismApplicationBase.Current.Container.Resolve<IAddressRepository>();

                    addressRepo.SaveAddress(shippingAddress, AppData.Device.UserLoggedOnToDevice.Id);


                    

                    Task.Run(async() =>
                    {
                        try
                        {
                           var lineNo = await new MemberContactModel().AddNewAddressAsync(AppData.Device.CardId, shippingAddress);
                            if (lineNo > 0)
                            {
                                shippingAddress.LineNO = lineNo;
                                AppData.Device.UserLoggedOnToDevice.Addresses.Add(shippingAddress);
                            }
                            
                        }
                        catch (Exception)
                        {

                            
                        }
                    });
                }
                if (isSameAddress)
                {
                    billingAddressName = shippingAddressName;
                }

                NavigationParameters param = new NavigationParameters();

                if (isCreditCard)
                {
                    param.Add("BillAddress", billingAddress);
                    param.Add("billName", billingAddressName);
                    param.Add("cardno", CardNumber);
                    param.Add("cardexp", CardExpirationDate);
                    param.Add("cvv", CardCvv);
                }
                param.Add("ShipAddress", shippingAddress);
                param.Add("shipMethod", shippingMedhod);
                param.Add("shipName", shippingAddressName);

                await NavigationService.NavigateAsync(nameof(CheckoutTotalPage), param);

            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(shippingAddressName) || string.IsNullOrEmpty(shippingAddress.Address1)) 
            {
                DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewAllRequiredFieldsMustBeFilled", AppResources.Culture));
                return false;
            }

            if (isCreditCard)
            {
                if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardExpirationDate) || string.IsNullOrEmpty(CardCvv))
                {
                    DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewMustEnterCreditCardInfo", AppResources.Culture));
                    return false;
                }

                if (!isSameAddress)
                {
                    if (string.IsNullOrEmpty(billingAddressName) || string.IsNullOrEmpty(billingAddress.Address1))
                    {
                        DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("CheckoutViewAllRequiredFieldsMustBeFilled", AppResources.Culture));
                        return false;
                    }
                }
            }

            return true;

        }



        private void LoadMemberContact()
        {
            contact = AppData.Device.UserLoggedOnToDevice;
            shippingAddressName = contact.Name;
            shippingAddress = selectedAddress;
        }

        private Address LoadAddress()
        {
            if (contact.Addresses != null && contact.Addresses.Count > 0)
            {
               var  address = new Address
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

        private void LoadData()
        {

            Task.Run(async () =>
            {
               
                Cities = new ObservableCollection<CitiesModel>(await new CommonModel().GetCitiessync());
            });

            if (AppData.Device.UserLoggedOnToDevice.Addresses != null)
            {

                foreach (var item in AppData.Device.UserLoggedOnToDevice.Addresses)
                {
                    string title = $"{AppData.Device.UserLoggedOnToDevice.Name} :  {item.FloorNo} , {item.ApartmentNo} , {item.Street}\n{item.Area}, {item.City}. {item.Country} - {item.PostCode}";

                    //addresses.Add(title);
                }
                addresses = new ObservableCollection<Address>(AppData.Device.UserLoggedOnToDevice.Addresses);
            }
            addresses.Add(new Address { Address1 = "Enter New Address" });

            selectedAddress = addresses.First();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var navigationMode = parameters.GetNavigationMode();
            switch (navigationMode)
            {
                case NavigationMode.Back:
                    IsBackNavigation = true;
                    break;
                case NavigationMode.New:
                   
                    break;
                case NavigationMode.Forward:
                    break;
                case NavigationMode.Refresh:
                    break;
                default:
                    break;
            }
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            shippingMedhod = parameters.GetValue<ShippingMedhod>("shipMethod");
            LoadData();

        }

       
    }
}
