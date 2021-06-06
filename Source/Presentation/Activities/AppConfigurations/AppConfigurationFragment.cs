using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Telecom;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Util;

namespace Presentation.Activities.AppConfigurations
{
    public class AppConfigurationFragment : LoyaltyFragment, IBroadcastObserver, IRefreshableActivity, IItemClickListener, SwipeRefreshLayout.IOnRefreshListener, PopupMenu.IOnMenuItemClickListener
    {
        RadioButton ArLanBtn;
        RadioButton EnLanBtn;
        Presentation.Views.ColoredButton ApplyBtn;

        RadioGroup LanguageRadioGroup;
        ISharedPreferences pref;
        String LangCode = "";
        public void BroadcastReceived(string action)
        {
            throw new NotImplementedException();
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            throw new NotImplementedException();
        }

       

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);


            
            base.OnCreate(savedInstanceState);

            // Create your application here

            


            HasOptionsMenu = true;

            var isBigScreen = Resources.GetBoolean( Resource.Boolean.BigScreen);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.AppConfigurations);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppConfigurationToolbar);
            ApplyBtn = view.FindViewById<Presentation.Views.ColoredButton>(Resource.Id.AppConfigSaveButton);
            EnLanBtn = view.FindViewById<RadioButton>(Resource.Id.EnRadioBtn);
            ArLanBtn = view.FindViewById<RadioButton>(Resource.Id.ArRadioBtn);
            LanguageRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.LanguageRadioGroup);
            pref = PreferenceManager.GetDefaultSharedPreferences(this.Context);

            LangCode = pref.GetString("LangCode", "en");


            if (LangCode == "en")

            {
                EnLanBtn.Checked = true;
                ArLanBtn.Checked = false;
            }
            else if (LangCode == "ar")
            {
                EnLanBtn.Checked = false;
                ArLanBtn.Checked = true;
            }

            ApplyBtn.Click += ChangeLanguageBtnAction;


            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);



            if (isBigScreen)
            {
            }
            else
            {
            }

            return view;



   //         return base.OnCreateView(inflater, container, savedInstanceState);
        }


        private void ChangeLanguageBtnAction(object sender,EventArgs e)
        {
            if (ArLanBtn.Checked)
            {
                LangCode = "ar";
            }
            else if (EnLanBtn.Checked)
            {
                LangCode = "en";

            }
            Toast.MakeText(Context, LangCode, ToastLength.Short);
            var editor = pref.Edit();
            editor.PutString("LangCode", LangCode);
            editor.Apply();
            SetLanguage();

            var intent = new Intent();
            intent.SetClass(Activity, typeof(HomeActivity));

            Toast.MakeText(this.Activity , Resource.String.ApplySettings, ToastLength.Short ).Show();

            StartActivity(intent);

        }

        public void SetLanguage()
        {

            Android.Content.Res.Configuration conf = this.Resources.Configuration;
            DisplayMetrics displayMetrix = this.Resources.DisplayMetrics;
            conf.SetLocale(new Locale(LangCode));
#pragma warning disable CS0618 // Type or member is obsolete
            this.Resources.UpdateConfiguration(conf, displayMetrix);
#pragma warning restore CS0618 // Type or member is obsolete
            this.Activity.Recreate();
            
         
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void OnRefresh()
        {
            throw new NotImplementedException();
        }

        public void ShowIndicator(bool show)
        {
            throw new NotImplementedException();
        }
    }
}