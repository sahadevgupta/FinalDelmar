using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormsLoyalty.Services
{
    public interface IScanSendManager
    {
        Task<bool> CreateScanSend(List<ScanSend> scanSends);
    }
}