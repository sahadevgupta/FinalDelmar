using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public interface IScanSendRepo
    {
        object ScanSendCreate(ScanSend request);
        Task<string> GetTermsCondition();
    }
}