using System.Collections.Generic;

using Prism.Ioc;
using Infrastructure.Data.SQLite.MemberContacts;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;

using Prism.Services;
using Prism.Unity;
using Prism;
using Xamarin.Essentials;
using Infrastructure.Data.SQLite.Devices;
using System;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Microsoft.AppCenter.Crashes;

namespace FormsLoyalty.Utils
{
    public enum SocialMediaConnection
    {
        None,
        Google,
        Facebook,
    }

    public enum ShippingMedhod
    {
        ClickCollect = 0,
        HomeDelivery = 1
    }

    class AppData
    {
        private static List<Store> stores = new List<Store>();

        public static IDeviceLocalRepository _deviceLocalRepository { get; set; }
        public static IMemberContactLocalRepository _memberContactRepo { get; set; }

        private AppData()
        {
            
        }


        public static string ShowAsCard = "ShowAsCard";
        


        public static Device Device
        {
            get
            {
                if (device == null)
                {
                    try
                    {
                        _deviceLocalRepository = new DeviceRepository();
                        device = _deviceLocalRepository.GetDevice();

                        if (!(device is UnknownDevice) && AppData.IsLoggedIn)
                        {
                            _memberContactRepo = new MemberContactRepository();
                            var contact = _memberContactRepo.GetLocalMemberContact();

                            if (contact != null)
                            {
                                contact.LoggedOnToDevice = device;

                                device.CardId = contact.LoggedOnToDevice.CardId;
                                device.SecurityToken = contact.LoggedOnToDevice.SecurityToken;
                                device.UserLoggedOnToDevice = contact;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                        
                    }
                    
                }

                return device;
            }
            set { device = value; }
        }

        private static List<ItemCategory> itemCategories;
        public static List<ItemCategory> ItemCategories
        {
            get { return itemCategories; }
            set { itemCategories = value; }
        }

        private static List<OneListItem> _cartItems;
        public static List<OneListItem> CartItems
        {
            get { return _cartItems; }
            set { _cartItems = value; }
        }
        private static List<LoyItem> _bestSellers;
        public static List<LoyItem> BestSellers
        {
            get { return _bestSellers; }
            set { _bestSellers = value; }
        }
        private static List<LoyItem> _mostViewed;
        public static List<LoyItem> MostViewed
        {
            get { return _mostViewed; }
            set { _mostViewed = value; }
        }

        private static List<PublishedOffer> _publishedOffers;
        public static List<PublishedOffer> PublishedOffers
        {
            get { return _publishedOffers; }
            set { _publishedOffers = value; }
        }

        public static List<Store> Stores
        {
            get { return stores; }
            set { stores = value; }
        }

        public static OneList Basket
        {
            get
            {
                if (Device == null || Device.UserLoggedOnToDevice == null)
                    return null;

                return Device.UserLoggedOnToDevice.GetBasket(Device.CardId);
            }
            set
            {
                if (Device == null)
                    return;

                if(Device.UserLoggedOnToDevice is object)
                   Device.UserLoggedOnToDevice.AddList(Device.CardId, value, ListType.Basket);
            }
        }

        public static List<Advertisement> Advertisements { get; set; }
        public static bool IsDualScreen = false;
        public static string magazineURL = "https://magento.linkedgates.com/";


        public static bool IsLoggedIn
        {
            get => Preferences.Get(nameof(IsLoggedIn), false);
            set => Preferences.Set(nameof(IsLoggedIn), value);
        }

        public static bool IsViewStock = false;
        public static bool IsLanguageChanged = false;
        public static bool IsFirstTimeMemberRefresh = false;

        public static bool GetSocialMediaStatusResult;

        public static string Msg { get; set; }
        public static string MyPoints { get; set; }
        public static Device device;
        

        public static SocialMediaConnection SocialMediaConnection = SocialMediaConnection.None;
        public static List<ShippingMedhod> ShippingMethods = new List<ShippingMedhod>(){ShippingMedhod.ClickCollect, ShippingMedhod.HomeDelivery}; 



    }
}