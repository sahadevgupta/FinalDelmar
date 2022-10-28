using System;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Utils
{
    public class Utils
    {
        public enum AppType
        {
            POS,
            Inventory,
            Loyalty,
            HospitalityLoyalty
        };

        //public const string DefaultUrl = @"http://197.51.15.83/LSOmniService/ucjson.svc";
        //public const string DefaultUrlLoyalty = @"http://197.51.15.83/LSOmniService/ucjson.svc";


        //public const string DefaultUrl = @"http://197.51.15.83/LSOmniServicetest/UCJson.svc";               //Test Server
        //public const string DefaultUrlLoyalty = @"http://197.51.15.83/LSOmniServicetest/UCJson.svc";

        //public const string DefaultUrl = @"http://41.39.182.163/LSOmniService/UCJson.svc";               //New Server
        //public const string DefaultUrlLoyalty = @"http://41.39.182.163/LSOmniService/UCJson.svc";

        public const string DefaultUrl = @"http://delmar-test.linkedgates.com/UCJson.svc";               //New Test Server
        public const string DefaultUrlLoyalty = @"http://delmar-test.linkedgates.com/UCJson.svc";

        //public static readonly string DefaultUrl = @"https://app.delmar-attalla.com/UCJson.svc";               //Live Server
        //public const string DefaultUrlLoyalty = @"https://app.delmar-attalla.com/UCJson.svc/";

        //public const string DefaultUrl = @"http://142.11.241.157:8080/LSOmniService/ucjson.svc";
        //public const string DefaultUrlLoyalty = @"http://142.11.241.157:8080/LSOmniService/ucjson.svc";



        //public const string DefaultUrl = @"http://mobiledemo.lsretail.com/LSOmniService/appjson.svc";
        //public const string DefaultUrlLoyalty = @"http://mobiledemo.lsretail.com/LSOmniService/ucjson.svc";

        public static void InitWebService(string deviceId, AppType appType, string url = "", string lsKey = "", int timeOut = 200, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(url))
            {
                ConfigStatic.Url = (appType == AppType.POS || appType == AppType.Inventory) ? DefaultUrl : DefaultUrlLoyalty;
            }
            else
            {
                ConfigStatic.Url = url;
            }

            ConfigStatic.LsKey = lsKey;
            ConfigStatic.Timeout = timeOut;
            ConfigStatic.LanguageCode = languageCode;
            ConfigStatic.UniqueDeviceId = deviceId;
        }

        public string PingServer()
        {
            try
            {
                var repository = new UtilityRepository();
                return repository.Ping(ConfigStatic.Timeout);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
