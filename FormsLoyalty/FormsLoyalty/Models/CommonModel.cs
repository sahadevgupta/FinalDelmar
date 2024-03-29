﻿using FormsLoyalty.Utils;
using Infrastructure.Data.SQLite.Devices;
using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Domain.Services.Loyalty.VeifyPhone;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.VerifyPhone;
using Prism;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class CommonModel : BaseModel
    {
        private CommonModelRepository _repository;
        private CommonModelService _service;
        private DeviceRepository deviceRepo;
        public CommonModel()
        {
            _service = new CommonModelService();
            _repository = new CommonModelRepository();
            deviceRepo = new DeviceRepository();
        }

        #region Phone Verification
        public async Task<string> GenerateOTPAsync(string number)
        {
            return await Task.Run(() => GenerateOTP(number)) ;
           
        }

        private string GenerateOTP(string number)
        {
            return _service.GenerateOTP(_repository, number);
        }

        public async Task<MemberContact> VerifyPhoneAsync(string number)
        {
           
            try
            {
                var contact = await VerifyPhone(number);
                if (contact == null)
                {
                    
                    return null;
                }
                AppData.Device.UserLoggedOnToDevice = contact;
                AppData.Device.UserLoggedOnToDevice.Cards = contact.Cards;
                AppData.Device.SecurityToken = contact.LoggedOnToDevice?.SecurityToken;

                if(contact.Cards.Any())
                    AppData.Device.CardId = contact.Cards[0].Id;

                AppData.Basket = contact.GetBasket(AppData.Device.CardId);

                SaveLocalMemberContact(contact);

                deviceRepo.SaveDevice(AppData.Device);

                if (string.IsNullOrEmpty(contact.LoggedOnToDevice?.Manufacturer) || string.IsNullOrEmpty(contact.LoggedOnToDevice?.Platform) || string.IsNullOrEmpty(contact.LoggedOnToDevice?.OsVersion) || string.IsNullOrEmpty(contact.LoggedOnToDevice?.Model))
                {
                    await new FormsLoyalty.Models.MemberContactModel().RegisterDevice();
                }
                return contact;
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex, showToastOnNetworkError: false, displayAlert: false);
                return new MemberContact();
            }
          
        }
       
        public void SaveLocalMemberContact(MemberContact contact)
        {
            var contactRepo = PrismApplicationBase.Current.Container.Resolve<IMemberContactLocalRepository>();

            contactRepo.SaveMemberContact(contact);
        }
        private async  Task<MemberContact> VerifyPhone(string number)
        {
               return await Task.Run(() =>
                {
                    return _service.VerifyPhone(_repository, number);
                });
        }

        #endregion


        #region Delmar Coupons
        public async Task<List<DelmarCoupons>> GetDelmarCouponAsync(string accountNo)
        {
            return await Task.Run(() => FetchDelmarCoupon(accountNo));

        }

        private List<DelmarCoupons> FetchDelmarCoupon(string accountNo)
        {
            return _service.GetDelmarCoupons(_repository, accountNo);
        }
        #endregion

        #region API Call Address Areas and Cities
        public async Task<List<AreaModel>> GetAreasAsync(string city)
        {
            return await Task.Run(() => GetAreas(city));

        }
        public async Task<List<CitiesModel>> GetCitiessync()
        {
            return await Task.Run(() => GetCities());

        }
        private List<CitiesModel> GetCities()
        {
            return _service.GetCities(_repository);
        }
        private List<AreaModel> GetAreas(string city)
        {
            return _service.GetAreas(_repository,city);
        }
        #endregion

        #region Magazine
        public async Task<List<LSRetail.Omni.Domain.DataModel.Loyalty.Magazine.MagazineModel>> GetMagazineAsync()
        {
            return await Task.Run(() => GetMagazine());

        }

        private List<LSRetail.Omni.Domain.DataModel.Loyalty.Magazine.MagazineModel> GetMagazine()
        {
            return _service.GetMagazines(_repository);
        }
        #endregion


        #region Check_Social_Media_Display_Status

        public async Task<bool> GetSocialMediaDisplayStatusAsync()
        {
            return await Task.Run(() => GetSocialMediaDisplayStatus());

        }

        private bool GetSocialMediaDisplayStatus()
        {
            return _service.GetSocialMediaDisplayStatus(_repository);
        }

        #endregion

    }
}
