using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using Xamarin.Essentials;

namespace FormsLoyalty.Helpers
{
    public static class  Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        private const string ShowCardView = "setting_key7";
        private const string RTLDirection = "LanguageSetting";
        private const string NotificationToken = "notificationToken";
        private const string SYNC_FCM_KEY = "SYNC_FCMTTOKEN_KEY";
        private static readonly bool ViewDefaultValue = true;
        private static readonly bool DefaultValue = false;

        public static bool ShowCard
        {
            get
            {
                return AppSettings.GetValueOrDefault(ShowCardView, ViewDefaultValue);
            }
            set
            {
                AppSettings.AddOrUpdateValue(ShowCardView, value);
            }
        }

     

        public static bool RTL
        {
            get 
            { 
                return AppSettings.GetValueOrDefault(RTLDirection, DefaultValue); 
            }
            set 
            {
                AppSettings.AddOrUpdateValue(RTLDirection, value); 
            }
        }

        public static string TermsConditions
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(TermsConditions), null);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(TermsConditions), value);
            }
        }

        public static string FCM_Token
        {
            get
            {
                return AppSettings.GetValueOrDefault(NotificationToken, string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue(NotificationToken, value);
            }
        }

        public static bool KEY_SYNC_FCMTOKEN
        {
            get
            {
                return AppSettings.GetValueOrDefault(SYNC_FCM_KEY, DefaultValue);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SYNC_FCM_KEY, value);
            }
        }

    }
}
