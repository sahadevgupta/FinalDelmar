using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members.SpecialCase;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System.Diagnostics;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Members
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017"), KnownType(typeof(UnknownMemberContact))]
    public class MemberContact : Entity, IDisposable,INotifyPropertyChanged
    {
        #region Member variables

        private List<Notification> notifications;

        #endregion

        #region Properties

        private string _UserName;
        [DataMember(IsRequired = true)]
        public string UserName
        {
            get { return _UserName; }
            set 
            { 
                _UserName = value;

                OnPropertyChanged("UserName");
            }
        }

       
            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        

        [DataMember]
        public string Password { get; set; }

        private string _Email;

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember(IsRequired = true)]
        public string Email
        {
            get { return _Email; }
            set 
            {
                _Email = value;
                OnPropertyChanged("Email");
            }
        }

        [DataMember]
        public string Initials { get; set; }

        [DataMember(IsRequired = true)]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember(IsRequired = true)]
        public string LastName { get; set; }

        [DataMember]
        public List<LSRetail.Omni.Domain.DataModel.Base.Retail.Address> Addresses { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string StoreId { get; set; }
        [DataMember]
        public string MobilePhone { get; set; }
        [DataMember]
        public Gender Gender { get; set; }
        [DataMember]
        public MaritalStatus MaritalStatus { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime BirthDay { get; set; }

        //Due to circular reference
       
        

        private Device _LoggedOnToDevice;
        [DataMember]
        [JsonProperty("LoggedOnToDevice")]
        public Device LoggedOnToDevice
        {
            get { return _LoggedOnToDevice; }
            set { _LoggedOnToDevice = value; }
        }



        [DataMember(IsRequired = false)]
        public string FacebookID { get; set; }

        
        [DataMember(IsRequired = false)]
        public string GoogleID { get; set; }


        [DataMember]
        public string Name
        {
            get
            {
                string name = FirstName;
                if (string.IsNullOrEmpty(MiddleName) == false)
                {
                    name += " " + MiddleName;
                }
                name += " " + LastName;
                return name;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                string[] names = value.Split(' ');
                if (names.Count() == 1)
                {
                    FirstName = value;
                    MiddleName = string.Empty;
                    LastName = string.Empty;
                }
                else if (names.Count() == 2)
                {
                    FirstName = names[0];
                    MiddleName = string.Empty;
                    LastName = names[1];
                }
                else if (names.Count() > 2)
                {
                    FirstName = names[0];
                    MiddleName = string.Empty;
                    LastName = names[names.Count() - 1];
                    for (int i = 1; i < (names.Count() - 1); i++)
                    {
                        MiddleName += names[i];
                    }
                }

                OnPropertyChanged("Name");
            }
        }

        [DataMember]
        public List<Notification> Notifications
        {
            get { return notifications; }
            set
            {
                if (value == null)
                    notifications = new List<Notification>();
                else
                    notifications = value.OrderBy(x => x.Status)
                                     .ThenByDescending(x => x.ExpiryDate)
                                     .ThenByDescending(x => x.Description)
                                     .ToList();
            }
        }

        [DataMember]
        public List<PublishedOffer> PublishedOffers { get; set; }
        [DataMember]
        public List<Profile> Profiles { get; set; }
        [DataMember]
        public List<SalesEntry> SalesEntries { get; set; }
        [DataMember]
        public List<OneList> OneLists { get; set; }

        [DataMember]
        public OmniEnvironment Environment { get; set; }
        [DataMember]
        public string AlternateId { get; set; }
        [DataMember]
        public List<Card> Cards { get; set; }
        [DataMember]
        public Account Account { get; set; }

        [IgnoreDataMember]
        public List<SalesEntry> TransactionOrderedByDate
        {
            get { return SalesEntries?.OrderByDescending(x => x.DocumentRegTime).ToList(); }//
        }

        #endregion

        #region Constructors

        public MemberContact()
            : this(null)
        {
            UserName = string.Empty;
        }

        public MemberContact(string id)
            : base(id)
        {
            UserName = string.Empty;
            Password = string.Empty;
            Initials = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;

            LoggedOnToDevice = null;
            BirthDay = new DateTime(1900, 1, 1);
            Environment = new OmniEnvironment();
            Addresses = new List<LSRetail.Omni.Domain.DataModel.Base.Retail.Address>();
            Account = null;
            notifications = new List<Notification>();
            PublishedOffers = new List<PublishedOffer>();
            Profiles = new List<Profile>();
            SalesEntries = new List<SalesEntry>();
            Cards = new List<Card>();
            OneLists = new List<OneList>();
        }

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

        #endregion

        public Card GetCard(string id)
        {
            return Cards.Find(c => c.Id == id);
        }

        public OneList GetWishList(string cardId)
        {
            return GetWishList(cardId, null);
        }

        public OneList GetWishList(string cardId, OneList list)
        {
            if (OneLists.Count > 0 && string.IsNullOrEmpty(cardId) == false)
            {
                if (list == null)
                    list = OneLists.Find(t => t.ListType == ListType.Wish && t.CardId == cardId);
                else
                    list = OneLists.Find(t => t.ListType == ListType.Wish && t.CardId == cardId && t.Id == list.Id);
            }

            if (list == null)
            {
                list = new OneList()
                {
                    CardId = LoggedOnToDevice.CardId,
                    ListType = ListType.Wish,
                    Items = new List<OneListItem>()
                };
            }
            return list;
        }

        public OneList CreateOneListWithDescription(string description)
        {
            OneList list = null;

            list = new OneList()
            {
                CardId = LoggedOnToDevice.CardId,
                ListType = ListType.Wish,
                Description = description,
                Items = new List<OneListItem>()
            };

            return list;
        }

        public OneList GetBasket(string cardId)
        {
            OneList list = null;
            if (OneLists.Count > 0 && string.IsNullOrEmpty(cardId) == false)
            {
                list = OneLists.Find(t => t.ListType == ListType.Basket && t.CardId == cardId);
            }
            if (list == null)
            {
                list = new OneList()
                {
                    CardId = LoggedOnToDevice.CardId,
                    ListType = ListType.Basket,
                    Items = new List<OneListItem>()
                };
            }

            Debug.WriteLine("***************BasketItem***************");

            foreach (var x in list.Items)
            Debug.WriteLine("***************BasketItem***************" + x.Detail);


            return list;
        }

        public void AddList(string cardId, OneList list, ListType type)
        {            
            OneList mylist = OneLists.Find(t => t.ListType == type && t.CardId == cardId && t.Id == list.Id);
            if (mylist != null)
            {
                var index = OneLists.IndexOf(OneLists.FirstOrDefault(x => x.Id == mylist.Id));
                if(index > -1)
                {
                    OneLists.RemoveAt(index);
                }

                
            }
            OneLists.Add(list);

        }

        public static bool NameContainsFirstName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        public static bool NameContainsLastName(string name)
        {
            string[] names = name.Trim(' ').Split(' ');
            return names.Count() >= 2;
        }

        public string FormatPoints(DecimalUtils.DecimalFormat decimalFormat)
        {
            string points = "0";
            if (Account != null)
            {
                points = Account.PointBalance.ToString("N0", DecimalUtils.GetCultureInfo(decimalFormat));
            }
            return points;
        }

        //note> only clones the basic members, references to custom items remain the sam
        public MemberContact ShallowCopy()
        {
            MemberContact temp = (MemberContact)MemberwiseClone();
            return temp;
        }
    }

    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public enum Gender
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Male = 1,
        [EnumMember]
        Female = 2,
    }

    [DataContract(Namespace = "http://lsretail.com/LSOmniService/POS/2014/10")]
    public enum MaritalStatus
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Single = 1,
        [EnumMember]
        Married = 2,
        [EnumMember]
        Divorced = 3,
        [EnumMember]
        Widowed = 4,
    }
}
