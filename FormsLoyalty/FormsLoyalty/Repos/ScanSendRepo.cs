using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public class ScanSendRepo : BaseRepository, IScanSendRepo
    {
        public object ScanSendCreate(ScanSend request)
        {
            string methodName = "ScanSendCreate";
            var jObject = new { ScanSend = request };
            return base.PostData<bool>(jObject, methodName);
        }

        public async Task<string> GetTermsCondition()
        {

            return await Task.Run(() => GetTermsAndConditionsAsync());

        }

        private string GetTermsAndConditionsAsync()
        {
            string methodName = "TermsAndConditions";

            return base.PostData<string>(null, methodName);
        }
    }
}
