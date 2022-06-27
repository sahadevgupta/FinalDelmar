﻿using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using Infrastructure.Data.SQLite.Offers;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Offers;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Prism;
using Prism.Ioc;
using System.Threading.Tasks;
using Xamarin.Forms;
using FormsLoyalty.Repos;

namespace FormsLoyalty.Models
{
    public class OfferModel : BaseModel
    {
        private SharedService service;
        private OfferLocalService localService;

        public async Task GetOffersByCardId(string cardId)
        {
            

            BeginWsCall();

            try
            {
                var offers = await service.GetPublishedOffersByCardIdAsync(cardId);
                if (offers is object && PrismApplicationBase.Current?.Container is object)
                {
                    var offerRepo = PrismApplicationBase.Current.Container.Resolve<IGenericDatabaseRepo<PublishedOffer>>();
                    foreach (var offer in offers)
                    {
                       var queryTable =  await offerRepo.GetItemsAsync().ConfigureAwait(false);
                        if (!queryTable.Any(x => x.Id == offer.Id))
                        {
                            await offerRepo.Insert(offer);
                        }
                         
                    }
                   
                }
                AppData.PublishedOffers = offers;

                if(AppData.Device?.UserLoggedOnToDevice !=null)
                     AppData.Device.UserLoggedOnToDevice.PublishedOffers = offers;

                //SendBroadcast(Utils.BroadcastUtils.CouponsUpdated);
                //SendBroadcast(Utils.BroadcastUtils.OffersUpdated);

                SaveLocalOffers(offers);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

        }

        public async Task<List<PublishedOffer>> GetPublishedOffersByItemId(string itemId, string cardId)
        {
            List<PublishedOffer> offers = null;


            BeginWsCall();

            try
            {
                offers = await service.GetPublishedOffersByItemIdAsync(itemId, cardId);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }


            return offers;
        }

        public void ToggleOffer(PublishedOffer offer)
        {
            offer.Selected = !offer.Selected;

           // SendBroadcast(Utils.BroadcastUtils.CouponsUpdated);
           // SendBroadcast(Utils.BroadcastUtils.OffersUpdated);

            if (offer.Selected)
            {
                DependencyService.Get<INotify>().ShowToast(AppResources.ResourceManager.GetString("OfferViewAddedToQRCode",AppResources.Culture));
            }
        }

        private void SaveLocalOffers(List<PublishedOffer> offers)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                localService.SavePublishedOffers(offers);
            };

            worker.RunWorkerAsync();
        }

        protected void BeginWsCall()
        {
            service = new SharedService(new SharedRepository());
            localService = new OfferLocalService(new OfferRepository());
        }
    }
}
