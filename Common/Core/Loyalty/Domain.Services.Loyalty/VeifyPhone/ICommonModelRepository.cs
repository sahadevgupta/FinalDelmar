using LSRetail.Omni.Domain.DataModel.Loyalty.Address;
using LSRetail.Omni.Domain.DataModel.Loyalty.Coupons;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSRetail.Omni.Domain.Services.Loyalty.VeifyPhone
{
    public interface ICommonModelRepository
    {
        MemberContact VerifyPhoneNumber(string phoneNumber); 
        string GenerateOTP(string phoneNumber);
        List<DelmarCoupons> GetDelmarCoupons( string accountNo);
        List<MagazineModel> GetMagazines();
        List<AreaModel> GetAreasList(string city);
        List<CitiesModel> GetCitiesList();

        bool GetSocialMediaDisplayStatus();
    }
}