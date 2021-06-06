using System;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Java.Util;
using Fragment = Android.Support.V4.App.Fragment;

namespace Presentation.Activities.Base
{
    public class LoyaltyFragment : Fragment
    {
        protected bool active;
        private ISharedPreferences pref;
        private String LangCode = "";
        protected View Inflate(LayoutInflater inflater, int resourceId, ViewGroup root = null, bool tryAgain = true)
        {
            try
            {
                return inflater.Inflate(resourceId, root);
            }
            //catch (OutOfMemoryException oome)
            catch (Exception)
            {
                if (tryAgain)
                {
                    GC.Collect();
                    return Inflate(inflater, resourceId, root, false);
                }
                throw;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            SetLanguage();
            active = true;
        }

        public override void OnStop()
        {
            active = false;

            base.OnStop();
            SetLanguage();
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


        }
        public override void OnResume()
        {
            base.OnResume();
            SetLanguage();

        }
        public void SetLanguage()
        {
            try {

                pref = PreferenceManager.GetDefaultSharedPreferences(this.Context);

                LangCode = pref.GetString("LangCode", "en");
                Android.Content.Res.Configuration conf = this.Resources.Configuration;
                DisplayMetrics displayMetrix = this.Resources.DisplayMetrics;
                conf.SetLocale(new Locale(LangCode));
#pragma warning disable CS0618 // Type or member is obsolete
                this.Resources.UpdateConfiguration(conf, displayMetrix);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            catch (Exception)
            {

            }
           



        }

    }
}