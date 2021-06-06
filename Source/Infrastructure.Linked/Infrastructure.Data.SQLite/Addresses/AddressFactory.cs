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
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Addresses
{
    public class AddressFactory
    {
        public Address BuildEntity(AddressessData AddressData)
        {
            if (AddressData == null)
                return new Address();

            Address entity = new Address(AddressData.AddressID);

            entity.Address1 = AddressData.AddressID;
            entity.Address2 = AddressData.AddressTwo;
            entity.City = AddressData.City;
            entity.Country = AddressData.Country;
            entity.PostCode = AddressData.PostCode;
            entity.StateProvinceRegion = AddressData.State;
            entity.Latitude = AddressData.Latitude;
            entity.Longitude = AddressData.Longitude;



            return entity;
        }


    }
}