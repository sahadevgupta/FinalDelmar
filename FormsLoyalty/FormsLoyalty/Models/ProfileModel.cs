using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Profiles;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Members;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class ProfileModel : BaseModel
    {
        
        private ProfileService profileService;

       

        public async Task<List<Profile>> ProfilesGetAll()
        {
            List<Profile> profiles = null;

         

            BeginWsCall();

            try
            {
                profiles = await profileService.GetAvailableProfilesAsync();
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }


            return profiles;
        }

        public async Task<List<Profile>> ProfilesGetByCardId(string cardId)
        {
            List<Profile> allProfiles = null;


            BeginWsCall();

            try
            {
                var profiles = await profileService.ProfilesGetByCardIdAsync(cardId);

                allProfiles = await profileService.GetAvailableProfilesAsync();

                foreach (var memberProfile in profiles)
                {
                    foreach (var profile in allProfiles)
                    {
                        if (memberProfile.Id == profile.Id)
                        {
                            profile.ContactValue = memberProfile.ContactValue;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            return allProfiles;
        }

        protected  void BeginWsCall()
        {
            profileService = new ProfileService(new ProfileRepository());
        }
    }
}
