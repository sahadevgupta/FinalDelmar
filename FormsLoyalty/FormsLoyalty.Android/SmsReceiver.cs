using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.Phone;
using Android.Gms.Common.Apis;
using Android.Widget;
using FormsLoyalty.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormsLoyalty.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { SmsRetriever.SmsRetrievedAction })]

    internal class SmsReceiver : BroadcastReceiver
    {
        private static readonly string[] OtpMessageBodyKeywordSet = { "DevEnvExe Generated OTP" };

        // message formate -> <#> 54444 - DevEnvExe Generated OTP fNVBveGi1EN
        //Change value of GetAppHashKey -> fNVBveGi1EN
        //fNVBveGi1EN  form GetAppHashKey

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("SMS Read >>>   ------ OnReceive");
            try
            {
                if (intent.Action != SmsRetriever.SmsRetrievedAction) return;
                Android.OS.Bundle bundle = intent.Extras;
                if (bundle == null) return;
                Statuses status = (Statuses)bundle.Get(SmsRetriever.ExtraStatus);
                switch (status.StatusCode)
                {
                    case CommonStatusCodes.Success:
                        Console.WriteLine("SMS Read >>>   ------ CommonStatusCodes.Success");
                        string message = (string)bundle.Get(SmsRetriever.ExtraSmsMessage);
                        bool foundKeyword = OtpMessageBodyKeywordSet.Any(k => message.Contains(k));
                        if (!foundKeyword)
                        {
                            Console.WriteLine("SMS Read >>>   ------ CommonStatusCodes.Success");
                            return;
                        }
                        string code = ExtractNumber(message);
                        CommonHelper.Notify(Events.SmsRecieved, code);
                        break;

                    case CommonStatusCodes.Timeout:
                        Console.WriteLine("SMS Read >>>   ------ CommonStatusCodes.Timeout");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SMS Read >>>   ------ Exception: " + e.ToString());
                Toast.MakeText(context, "System.Exception => e:" + e.ToString(), ToastLength.Long).Show();
            }
        }

        private static string ExtractNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("SMS Read >>>   ------ ExtractNumber: Is NullOrEmpty");
                return "";
            }
            Console.WriteLine("SMS Read >>>   ------ ExtractNumber: " + text);
            string number = Regex.Match(text, @"\d+").Value;
            return number;
        }
    }
}