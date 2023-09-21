using System;
using System.Collections.Generic;
using System.Text;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Coupons
{
    public class DelmarCoupons
    {
        public string AccountNo { get; set; }
        public bool Blocked { get; set; }
        public string CouponID { get; set; }
        public double CouponValue { get; set; }
        public DateTime CreationDate { get; set; }
        public string CustomerOrderNo { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string MobileNo { get; set; }
        public int NoOfCoupons { get; set; }
        public double PointsRedeemed { get; set; }
        public bool Printed { get; set; }
        public double RedeemedAmount { get; set; }
        public string RedeemedInStoreNo { get; set; }
        public string RedeemedWithReceiptNo { get; set; }
        public int ReplicationCounter { get; set; }
        public bool SendSMS { get; set; }
        public bool Sent { get; set; }
        public string SentBy { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime SentTime { get; set; }
        public bool SentToMember { get; set; }
        public string timestamp { get; set; }
    }
}
