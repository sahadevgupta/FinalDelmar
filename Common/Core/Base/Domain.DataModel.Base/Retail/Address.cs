using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LSRetail.Omni.Domain.DataModel.Base.Retail
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Base/2017")]
    public enum AddressType
    {
        [EnumMember]
        Residential = 0,
        [EnumMember]
        Commercial = 1,
        [EnumMember]
        Store = 2,
        [EnumMember]
        Shipping = 3,
        [EnumMember]
        Billing = 4,
        [EnumMember]
        Work = 5,
    }

    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Base/2017")]
    public class Address : IDisposable , INotifyPropertyChanged
    {
        public Address(string id)
        {
            Id = id;
            Type = AddressType.Residential;
            Address1 = string.Empty;
            Address2 = string.Empty;
            City = string.Empty;
            PostCode = string.Empty;
            StateProvinceRegion = string.Empty;
            Country = string.Empty;
        }

        public Address() : this(string.Empty)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
        public string Id { get; set; }
        [DataMember]
        public AddressType Type { get; set; }
        /// <summary>
        /// Address line 1, street address, po box, company name, c/o
        /// </summary>
        [DataMember]
        public string Address1 { get; set; }
        /// <summary>
        /// Address line 2, Appartment, suite, unit, floor etc
        /// </summary>
        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public int LineNO { get; set; }

        [DataMember]
        public string HouseNo { get; set; }

        private string _city;
        [DataMember]
        public string City
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(nameof(City)); }
        }

        private string _country;
        [DataMember]
        public string Country
        {
            get { return _country; }
            set { _country = value; OnPropertyChanged(nameof(Country)); }
        }


        [DataMember]
        public string PostCode { get; set; }
        /// <summary>
        /// State / Province / Region
        /// </summary>
        [DataMember]
        public string StateProvinceRegion { get; set; }  //
        /// <summary>
        /// Country Code, see Countries/Regions in NAV for Codes
        /// </summary>
        
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public string CellPhoneNumber { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }


        [DataMember]
        public string HouseApartmentNo { get; set; }

        private string _area;
        [DataMember]
        public string Area
        {
            get { return _area; }
            set { _area = value; OnPropertyChanged(); }
        }


        private string _number;
        [DataMember]
        public string Number
        {
            get { return _number; }
            set { _number = value; OnPropertyChanged(); }
        }

        private string _street;
        [DataMember]
        public string Street
        {
            get { return _street; }
            set { _street = value; OnPropertyChanged(); }
        }

        private string _ApartmentNo;
        [DataMember]
        public string ApartmentNo
        {
            get { return _ApartmentNo; }
            set { _ApartmentNo = value; OnPropertyChanged(); }
        }

        private string _FloorNo;
        [DataMember]
        public string FloorNo
        {
            get { return _FloorNo; }
            set { _FloorNo = value; OnPropertyChanged(); }
        }
        
        public string FormatAddress
        {
            get
            {
                string address = $"{Number}, {Street}, {FloorNo}, {ApartmentNo}";

                //if (string.IsNullOrEmpty(Address2) == false)
                //    address += Environment.NewLine + Address2;

                address += Environment.NewLine + City;

                //if (string.IsNullOrEmpty(StateProvinceRegion) == false)
                //    address += ", " + StateProvinceRegion;

                //if (string.IsNullOrEmpty(PostCode) == false)
                //    address += ", " + PostCode; 
                
                if (string.IsNullOrEmpty(Country) == false)
                    address += Environment.NewLine + Country;

                return address;
            }

        }

        public override string ToString()
        {
            string s = string.Format("Id: {0} Type: {1} Address1: {2} Address2: {3} City: {4}  PostCode: {5}  StateProvinceRegion: {6}  Country: {7} ",
                Id, Type.ToString(), Address1, Address2, City, PostCode, StateProvinceRegion, Country);
            return s;
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

/* UK
 * Your table would have the following fields
ID .........Region Type .......Region Name  
1 ..........Country ...........UK ......... 
2 ..........Region ............England ... 
3 ..........County ............Berkshire .. 
4 ..........Town ..............Slough ... 
5 ..........Postal Area .......SL1 ....... 
*/
