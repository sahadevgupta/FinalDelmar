﻿using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Base;
using LSRetail.Omni.Domain.DataModel.Base.Setup.SpecialCase;

namespace LSRetail.Omni.Domain.DataModel.Base.Setup
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Base/2017")]
    public enum TerminalTypes
    {
        [EnumMember]
        Unknown = -1,
        [EnumMember]
        Retail = 1,
        [EnumMember]
        Hospitality = 3
    }

    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Base/2017"), KnownType(typeof(UnknownTerminal))]
    [System.Xml.Serialization.XmlInclude(typeof(UnknownTerminal))]
	public class Terminal : Entity, IAggregateRoot
    {
        public Terminal()
        {
            Store = new Store();
            Staff = new Staff();
            Features = new FeatureFlags();
        }

        public Terminal(string id) : base(id)
        {
            Store = new UnknownStore(string.Empty);
            Staff = new UnknownStaff(string.Empty);
            Features = new FeatureFlags();
            Description = string.Empty;
            InventoryMainMenuId = string.Empty;
            LicenseKey = string.Empty;
            UniqueId = string.Empty;
            MainMenu = string.Empty;
            StoreInventory = true;
        }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int DeviceType { get; set; }
        [DataMember]
        public int TerminalType { get; set; }
        [DataMember]
        public Store Store { get; set; }
        [DataMember]
        public Staff Staff { get; set; }
        [DataMember]
        public string MainMenu { get; set; }
        [DataMember]
        public string InventoryMainMenuId { get; set; }
        [DataMember]
        public string LicenseKey { get; set; }
        [DataMember]
        public int NoOfRecords { get; set; }
        [DataMember]
        public int ItemFilterMethod { get; set; }
        [DataMember]
        public bool StoreInventory { get; set; }
        [DataMember]
        public string UniqueId { get; set; }
        [DataMember]
        public string DefaultHospType { get; set; }
        [DataMember]
        public string HospTypeFilter { get; set; }
        [DataMember]
        public FeatureFlags Features { get; set; }
    }
}

