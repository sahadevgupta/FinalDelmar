using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;

namespace FormsLoyalty.Repos
{
    public interface IScanSendRepo
    {
        object ScanSendCreate(ScanSend scansend);
        string GetTermsCondition();
    }
}