using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Base.Loyalty;
using LSRetail.Omni.Domain.Services.Loyalty.Search;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Shared;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class GeneralSearchModel : BaseModel
    {
        private SearchService service;
        private SharedService sharedService;
        


        public async Task<SearchRs> Search(string searchKey, SearchType searchType)
        {
            SearchRs searchRs = null;

           

            BeginWsCall();

            var userId = string.Empty;

            if (EnabledItems.ForceLogin || AppData.Device.UserLoggedOnToDevice != null)
            {
                userId = AppData.Device.UserLoggedOnToDevice.Id;
            }

            try
            {
                searchRs = await service.SearchAsync(userId, searchKey, 15, searchType);

                
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }

           

            return searchRs;
        }

        public async Task<string> AppSettings(ConfigKey key)
        {
            string result = string.Empty;


            BeginWsCall();

            try
            {
                result = await sharedService.AppSettingsAsync(key, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await HandleUIExceptionAsync(ex);
            }


            return result;
        }

        protected void BeginWsCall()
        {
            service = new SearchService(new SearchRepository());
            sharedService = new SharedService(new SharedRepository());
        }
    }
}
