using System;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Setup
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class Device : Entity, IDisposable, IAggregateRoot
    {
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        [DataMember]
        public string DeviceFriendlyName { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public string OsVersion { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Model { get; set; }
        
        private string _cardId;
        [DataMember]
        public string CardId
        {
            get { return _cardId; }
            set { _cardId = value; }
        }

        //public string CardId { get; set; }
        [DataMember]
        public string SecurityToken { get; set; }
        [IgnoreDataMember]
        public MemberContact UserLoggedOnToDevice;
        [DataMember]
        public int Status;
        [DataMember]
        public string BlockedBy;
        [DataMember]
        public string BlockedReason;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime BlockedDate;
    }
}
