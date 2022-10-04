using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsLoyalty.Helpers
{
    public static class MemberConatcts
    {
       public static async Task RefreshMemberContactAsync()
        {
            AppData.IsFirstTimeMemberRefresh = false;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        if (AppData.Device?.UserLoggedOnToDevice != null)
                        {
                            var memberContactModel = new MemberContactModel();
                            await memberContactModel.UserGetByCardId(AppData.Device.CardId).ConfigureAwait(false);
                            AppData.IsFirstTimeMemberRefresh = true;
                            
                        }
                    }).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);

                }
            }

        }

        public static async Task LoadAdvertisementsFromServer()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var service = new SharedService(new SharedRepository());
                    var ads = await service.AdvertisementsGetByIdAsync("LOY", string.Empty).ConfigureAwait(false);

                    AppData.Advertisements = ads.ToList();

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

            }
        }

        public static async Task LoadOfferFromServer()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await new OfferModel().GetOffersByCardId(string.Empty).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

            }
        }

        public static async Task LoadCategories(int retryCounter = 3)
        {

            await Task.Run(async () =>
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        try
                        {

                            var cat = await new ItemModel().GetItemCategories().ConfigureAwait(false);
                            if (cat?.Count > 0)
                            {
                                retryCounter = 0;
                            }
                            

                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            if (retryCounter != 0)
                            {
                                await LoadCategories(--retryCounter);

                            }
                            
                               

                        }
                    }
                }).ConfigureAwait(false);
          
        }

        public static async Task LoadBestSellerItems(int retryCounter = 3)
        {
            if (AppData.BestSellers == null || AppData.BestSellers?.Count == 0)
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {

                            var bestSellerItems = await new ItemModel().GetBestSellerItems(10, true).ConfigureAwait(false);
                            
                            if (bestSellerItems.Any())
                            {
                                
                                retryCounter = 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                            if (retryCounter != 0)
                            {
                                await LoadBestSellerItems(--retryCounter);
                            }
                            
                        }
                    }).ConfigureAwait(false);
                }
            }
            

        }
    }
}
