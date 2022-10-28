using System;

using Infrastructure.Data.SQLite.DB;
using Infrastructure.Data.SQLite.DB.DTO;
using Infrastructure.Data.SQLite.Notifications;
using Infrastructure.Data.SQLite.Offers;
using Infrastructure.Data.SQLite.Transactions;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using Infrastructure.Data.SQLite.Addresses;
using Infrastructure.Data.SQLite.Devices;
using System.Linq;

namespace Infrastructure.Data.SQLite.MemberContacts
{
    public class MemberContactRepository : IMemberContactLocalRepository
    {
        private object locker = new object();
        IAddressRepository Addressrepo;

        public MemberContactRepository()
        {
            DBHelper.OpenDBConnection();
             Addressrepo = new Addresses.AddressRepository();

        }

        public MemberContact GetLocalMemberContact()
        {
            //lock (locker)
            {
                //get device only has one row, no need  to narrow down the search criteria
                var factory = new MemberContactFactory();

                var contactData = DBHelper.DBConnection.Table<MemberContactData>().FirstOrDefault();
                MemberContact contact = null;

                if (contactData != null)
                {
                    var notificationRepository = new NotificationRepository();
                    var offerRepository = new OfferRepository();                    
                    var transactionRepository = new TransactionRepository();



                    contact = factory.BuildEntity(contactData);

                    contact.Addresses = Addressrepo.GetAddresses(contact.Id);

                    contact.LoggedOnToDevice = new DeviceRepository().GetDevice();
                    contact.Notifications = notificationRepository.GetLocalNotifications();
                    contact.PublishedOffers = offerRepository.GetLocalPublishedOffers();                                           
                    contact.SalesEntries = transactionRepository.GetLocalTransactions();
                }

                return contact;
            }
        }

        public void SaveMemberContact(MemberContact memberContact)
        {
            try
            {
                var factory = new MemberContactFactory();

                var contact = factory.BuildEntity(memberContact);
                System.Diagnostics.Debug.WriteLine("Member contact save started");
                DBHelper.DBConnection.DeleteAll<MemberContactData>();

                System.Diagnostics.Debug.WriteLine("Member contact table deleted");

                DBHelper.DBConnection.Insert(contact);
                System.Diagnostics.Debug.WriteLine("Member contact data inserted");

                if (memberContact.Addresses != null)
                {
                    DBHelper.DBConnection.Table<AddressessData>().Delete(w => w.MemberAccountId == contact.ContactId);

                    Addressrepo.SaveAddresses(memberContact.Addresses, memberContact.Id);

                }

                var notificationRepository = new NotificationRepository();

                notificationRepository.SaveNotifications(memberContact.Notifications);

                var offerRepository = new OfferRepository();

                offerRepository.SavePublishedOffers(memberContact.PublishedOffers);

                var transactionRepository = new TransactionRepository();

                transactionRepository.SaveTransactions(memberContact.SalesEntries);

                var deviceRepository = new DeviceRepository();
                deviceRepository.SaveDevice(memberContact.LoggedOnToDevice);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        //public void DeleteMemberContact(string id)
        //{
        //   var data = DBHelper.DBConnection.Table<MemberContactData>().ToList();
        //   var item = data.FirstOrDefault(x => x.ContactId == id);
        //    DBHelper.DBConnection.Delete(item);
        //}

        public void DeleteMemberContact()
        {
            DBHelper.DBConnection.DeleteAll<MemberContactData>();
        }
    }
}
