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
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Infrastructure.Data.SQLite.Addresses
{
    public interface IAddressRepository
    {
        List<Address> GetAddresses(String ID);
        void SaveAddress(Address Address, String ContactID);
        void SaveAddresses(List<Address> Addresses, string ContactID);

    }
}