using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Addresses
{
    public class AddressRepository : IAddressRepository
    {
        private object locker = new object();


        public AddressRepository()
        {
            DBHelper.OpenDBConnection();
        }


        public List<Address> GetAddresses(string ID)
        {

            OfferData n = new OfferData();
            List<Address> result = new List<Address>();
            lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                AddressFactory factory = new AddressFactory();

                var Addreses = DBHelper.DBConnection.Table<AddressessData>().Where(w => w.MemberAccountId == ID).ToList();
                if (Addreses.Count() > 0)
                {
                    foreach (var temp in Addreses)
                        result.Add(factory.BuildEntity(temp));

                }



                return result;
            }
        }



        public void SaveAddress(Address Address, string ContactID)
        {
            lock (locker)
            {

                AddressessData Addressdata = new AddressessData();

                Addressdata.AddressOne = Address.Address1;
                Addressdata.AddressTwo = Address.Address2;
                Addressdata.City = Address.City;
                Addressdata.Country = Address.Country;
                Addressdata.State = Address.StateProvinceRegion;
                Addressdata.MemberAccountId = ContactID;
                Addressdata.Latitude = Address.Latitude;
                Addressdata.Longitude = Address.Longitude;



                DBHelper.DBConnection.Insert(Addressdata);


            }
        }

        public void SaveAddresses(List <Address> Addresses, string ContactID)
        {
            lock (locker)
            {
                foreach (var Address in Addresses)
                {
                    AddressessData Addressdata = new AddressessData();

                    Addressdata.AddressOne = Address.Address1;
                    Addressdata.AddressTwo = Address.Address2;
                    Addressdata.City = Address.City;
                    Addressdata.Country = Address.Country;
                    Addressdata.State = Address.StateProvinceRegion;
                    Addressdata.MemberAccountId = ContactID;
                   
                    Addressdata.Latitude = Address.Latitude;
                    Addressdata.Longitude = Address.Longitude;


                    DBHelper.DBConnection.Insert(Addressdata);
                }
               


            }
        }
    }
}