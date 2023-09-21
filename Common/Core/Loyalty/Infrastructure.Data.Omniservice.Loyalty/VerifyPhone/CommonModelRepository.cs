using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.Services.Loyalty.VeifyPhone;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.VerifyPhone
{
    public class CommonModelRepository : BaseRepository, ICommonModelRepository
    {
        public string GenerateOTP(string phoneNumber)
        {
            string methodName = "GenerateOtp";
            var jObject = new { phone = phoneNumber };
            string token = base.PostData<string>(jObject, methodName);


            return token;
        }

        public List<AreaModel> GetAreasList(string city)
        {
            string methodName = "AreasList";
            var jObject = new { City = city };
            List<AreaModel> areas = base.PostData<List<AreaModel>>(jObject, methodName);
            return areas;
        }

        public List<CitiesModel> GetCitiesList()
        {
            string methodName = "CitiesList";
            var jObject = new { };
            List<CitiesModel> cities = base.PostData<List<CitiesModel>>(jObject, methodName);
            return cities;
        }

        public List<DelmarCoupons> GetDelmarCoupons(string accountNo)
        {
            string methodName = "DelmarCouponsList";
            var jObject = new { AccountNo = accountNo };
            List<DelmarCoupons> coupons = base.PostData<List<DelmarCoupons>>(jObject, methodName);


            return coupons;
        }

        public List<MagazineModel> GetMagazines()
        {
            string methodName = "MagazinesList";
            var jObject = new { };
            List<MagazineModel> magazines = base.PostData<List<MagazineModel>>(jObject, methodName);


            return magazines;
        }

        public MemberContact VerifyPhoneNumber(string number)
        {
            string methodName = "VerifyPhone";
            var jObject = new { phone = number };
            MemberContact member = base.PostData<MemberContact>(jObject, methodName);


            return member;
        }

        public bool GetSocialMediaDisplayStatus()
        {
            string methodName = "getSocialMediaStatus";
            var jObject = new { };
            bool status = base.PostData<bool>(jObject, methodName);

            return status;
        }
    }
}
