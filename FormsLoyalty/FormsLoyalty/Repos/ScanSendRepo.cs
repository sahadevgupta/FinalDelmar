using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Repos
{
    public class ScanSendRepo : BaseRepository, IScanSendRepo
    {
        public object ScanSendCreate(ScanSend ScanSend)
        {
            string methodName = "ScanSendCreate";
            var jObject = new { ScanSend };
            return base.PostData<bool>(jObject, methodName);
        }

        public string GetTermsCondition()
        {
            string methodName = "TermsAndConditions";
            
            return base.PostData<string>(null, methodName);
        }
    }
}
