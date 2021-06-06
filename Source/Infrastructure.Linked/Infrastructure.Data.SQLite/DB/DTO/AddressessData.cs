using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Infrastructure.Data.SQLite.DB.DTO
{
    public class AddressessData
    {


        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } //each DTO class used in our SQLite class must have this ID. Never used by us, only internally

        public string AddressID { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }


        public string MemberAccountId { get; set; }
    }
}