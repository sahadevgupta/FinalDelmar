using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSRetail.Omni.Domain.Services.Loyalty.VeifyPhone
{
    public class CommonModelService 
    {
        public string GenerateOTP(ICommonModelRepository repository,  string phoneNumber)
        {
           return repository.GenerateOTP(phoneNumber);
        }

        public MemberContact VerifyPhone(ICommonModelRepository repository, string phoneNumber)
        {
            return repository.VerifyPhoneNumber(phoneNumber);
        }
        public List<MagazineModel> GetMagazines(ICommonModelRepository repository)
        {
            return repository.GetMagazines();
        }
        public List<DelmarCoupons> GetDelmarCoupons(ICommonModelRepository repository, string accountNo)
        {
            return repository.GetDelmarCoupons(accountNo);
        }

        public List<AreaModel> GetAreas(ICommonModelRepository repository, string city)
        {
            return repository.GetAreasList( city);
        }
        public List<CitiesModel> GetCities(ICommonModelRepository repository)
        {
            return repository.GetCitiesList();
        }
        public bool GetSocialMediaDisplayStatus(ICommonModelRepository repository)
        {
            return repository.GetSocialMediaDisplayStatus();
        }
    }
}
