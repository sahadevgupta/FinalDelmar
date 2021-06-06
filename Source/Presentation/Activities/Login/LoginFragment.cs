
using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Infrastructure.Data.SQLite.Devices;
using Java.Lang;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;
using Presentation.Activities.Account;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Settings;
using Presentation.Models;
using Presentation.Util;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using ColoredButton = Presentation.Views.ColoredButton;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Util;
using Android.Support.V7.App;
using Org.Json;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Presentation.Activities.Login
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", Name = "LoginFragment")]

    public class LoginFragment : LoyaltyFragment, IRefreshableActivity, TextView.IOnEditorActionListener, View.IOnClickListener, ITextWatcher , IFacebookCallback, GoogleApiClient.IOnConnectionFailedListener , GraphRequest.IGraphJSONObjectCallback
    {

        const string TAG = "MLoginActivity";

        const int RC_SIGN_IN = 9001;


        public GoogleApiClient mGoogleApiClient;
        public TextView mStatusTextView;
       public   ProgressDialog mProgressDialog;

        public  LoginButton BtnFBLogin;
        private ICallbackManager mFBCallManager;
        private MyProfileTracker mprofileTracker;
        private string FacebookID = "";
        private string GoogleID = "";
        private string FaceBookEmail = "";
        private string GoogleEmail = "";
        private string FirstName = "";

        private string LastName = "";
        public static bool IsFacebookLogin = false;



        private const int CreateAccountRequestCode = 0;

        private MemberContactModel memberContactModel;
        private DeviceService deviceService;

        private TextInputLayout usernameInputLayout;
        private TextInputLayout passwordInputLayout;
        private EditText username;
        private EditText password;
        private TextView errorMessage;
        private ViewSwitcher viewSwitcher;
        private View progressView;
        private View contentView;


        private bool isInsideApp = false;
        private bool isLoggingIn = false;
        private View view;

        public static LoginFragment NewInstance()
        {
            var loginDetail = new LoginFragment() { Arguments = new Bundle() };
            return loginDetail;
        }



      



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }


#pragma warning disable CS0618 // Type or member is obsolete
            if (!FacebookSdk.IsInitialized)
            {
                FacebookSdk.ApplicationId = "1370694683123728";
                FacebookSdk.ApplicationName = "Delmar-Attalla";

                FacebookSdk.SdkInitialize(Activity.ApplicationContext);

            }


#pragma warning restore CS0618 // Type or member is obsolete



            mprofileTracker = new MyProfileTracker();


            mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChangedAsync;

            mprofileTracker.StartTracking();


            if (mFBCallManager == null)
            {
                mFBCallManager = CallbackManagerFactory.Create();
            }


            view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.Login);


            var signInButton = view. FindViewById<SignInButton>(Resource.Id.Google_sign_in_button);

            signInButton.SetOnClickListener(this);
            BtnFBLogin = view.FindViewById<LoginButton>(Resource.Id.fblogin);

            BtnFBLogin.Text = GetString( Resource.String.FacebookLoginBtn);
         

#pragma warning disable CS0618 // Type or member is obsolete


            //        BtnFBLogin.SetReadPermissions(new List<string> {
            //    "user_friends",
            //    "public_profile"
            //});



            BtnFBLogin.SetReadPermissions("public_profile");
            BtnFBLogin.SetReadPermissions("email");
            BtnFBLogin.SetReadPermissions("public_profile email");


           

            //mFBCallManager = (Activity as LoyaltyFragmentActivity).mFBCallManager;

#pragma warning restore CS0618 // Type or member is obsolete
            (Activity as LoyaltyFragmentActivity).mFBCallManager = CallbackManagerFactory.Create();

            BtnFBLogin.RegisterCallback(mFBCallManager, (Activity as LoyaltyFragmentActivity));
          //  BtnFBLogin.Click += BtnFBLoginCallOnClick;

            
            LoginManager.Instance.LogOut();


            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                .RequestProfile()
                                
                                .RequestEmail()

                                .Build();
            if ((Activity as LoyaltyFragmentActivity) .mGoogleApiClient == null)
            {
                (Activity as LoyaltyFragmentActivity).mGoogleApiClient = new GoogleApiClient.Builder(this.Context)
                 .EnableAutoManage(this.Activity /* FragmentActivity */, this /* OnConnectionFailedListener */)
                 .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                 .AddScope(new Scope(Scopes.Profile))

                 .Build();

            }

            mGoogleApiClient = (Activity as LoyaltyFragmentActivity).mGoogleApiClient;


            var toolbar = view.FindViewById<Toolbar>(Resource.Id.LoginScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            HasOptionsMenu = true;

            memberContactModel = new MemberContactModel(Activity);

            deviceService = new DeviceService(new DeviceRepository());

            usernameInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.LoginScreenUsernameInputLayout);
            passwordInputLayout = view.FindViewById<TextInputLayout>(Resource.Id.LoginScreenPasswordInputLayout);

            username = view.FindViewById<EditText>(Resource.Id.LoginScreenUsername);
            password = view.FindViewById<EditText>(Resource.Id.LoginScreenPassword);
            errorMessage = view.FindViewById<TextView>(Resource.Id.LoginViewErrorText);

            if (Arguments != null && Arguments.ContainsKey(BundleConstants.ErrorMessage))
            {
                errorMessage.Text = Arguments.GetString(BundleConstants.ErrorMessage);
                errorMessage.Visibility = ViewStates.Visible;
            }

            username.SetOnEditorActionListener(this);
            password.SetOnEditorActionListener(this);

            var loginButton = view.FindViewById<ColoredButton>(Resource.Id.LoginViewLoginButton);
            loginButton.SetOnClickListener(this);

            var createAccountButton = view.FindViewById<TextView>(Resource.Id.LoginViewCreateAccountButton);
            createAccountButton.SetOnClickListener(this);

            var forgotPassword = view.FindViewById<TextView>(Resource.Id.LoginViewForgotPassword);
            forgotPassword.SetOnClickListener(this);

            viewSwitcher = view.FindViewById<ViewSwitcher>(Resource.Id.LoginViewSwitcher);
            progressView = view.FindViewById<View>(Resource.Id.LoginViewLoadingSpinner);
            contentView = view.FindViewById<View>(Resource.Id.LoginViewContent);

            if (Arguments != null)
                isInsideApp = Arguments.GetBoolean(BundleConstants.IsInsideApp);

            if (isInsideApp)
            {
                ShowError(GetString(Resource.String.LoginViewInvalidSecurityToken));
            }
            else
            {
                SavePushNotification();
            }

            if (savedInstanceState != null)
            {
                username.Text = savedInstanceState.GetString(BundleConstants.Username);
                password.Text = savedInstanceState.GetString(BundleConstants.Password);
                errorMessage.Text = savedInstanceState.GetString(BundleConstants.ErrorMessage);

                if (!string.IsNullOrEmpty(errorMessage.Text))
                {
                    errorMessage.Visibility = ViewStates.Visible;
                }
                else
                {
                    errorMessage.Visibility = ViewStates.Gone;
                }

                isLoggingIn = savedInstanceState.GetBoolean(BundleConstants.IsLoggingIn);
                if (isLoggingIn)
                {
                    Logon();
                }

            }

            return view;
        }


        //public void onClickFacebookButton(View view)
        //{
        //    if (view == fb)
        //    {
        //        BtnFBLogin.PerformClick();
        //        FacebookLogon();

        //    }
        //}

        //private void BtnFBLoginCallOnClick(object sender, EventArgs e)
        //{
        //    //Toast.MakeText(this.Context, "Clicked", ToastLength.Long).Show();

        //    BtnFBLogin.PerformClick();
        //    FacebookLogon();
        //}


        public override void OnStart()
        {
            base.OnStart();

            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                // and the GoogleSignInResult will be available instantly.
                Log.Debug(TAG, "Got cached sign-in");



                var result = opr.Get() as GoogleSignInResult;
                HandleSignInResult(result);
            }
            else
            {
                // If the user has not previously signed in on this device or the sign-in has expired,
                // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                // single sign-on will occur in this branch.
                ShowProgressDialog();
                opr.SetResultCallback(new SignInResultCallback { Activity = this });
            }


        }




        public override void OnResume()
        {
            base.OnResume();
            HideProgressDialog();

            username.AddTextChangedListener(this);
            password.AddTextChangedListener(this);

            if (this.mprofileTracker != null)
            {
                mprofileTracker.StartTracking();
            }

        }
        public override void OnStop()
        {
            base.OnStop();
            if (this.mprofileTracker != null)
            {
                mprofileTracker.StopTracking();
            }

            mGoogleApiClient.Disconnect();
            LoginManager.Instance.LogOut();

        }

        public override void OnPause()
        {
            username.RemoveTextChangedListener(this);
            password.RemoveTextChangedListener(this);
            if (this.mprofileTracker != null)
            {
                mprofileTracker.StopTracking();
            }
            base.OnPause();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString(BundleConstants.Username, username.Text);
            outState.PutString(BundleConstants.Password, password.Text);
            outState.PutString(BundleConstants.ErrorMessage, errorMessage.Text);
            outState.PutBoolean(BundleConstants.IsLoggingIn, isLoggingIn);
        }

        public override void OnDestroyView()
        {
            memberContactModel.Stop();

            base.OnDestroyView();
        }

        private async void SavePushNotification()
        {
            await memberContactModel.PushNotificationSave(PushStatus.Disabled);
        }

        private void GoToMainScreen()
        {
            isLoggingIn = false;

            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity);
            var prefEditor = sharedPreferences.Edit();
            prefEditor.PutString(PreferenceConstants.UserId, AppData.Device.UserLoggedOnToDevice.Id);
            prefEditor.PutString(PreferenceConstants.SecurityToken, AppData.Device.SecurityToken);
            prefEditor.Commit();

            if (EnabledItems.ForceLogin || Activity is HomeActivity)
            {
                if (Activity is HomeActivity)
                {
                    (Activity as LoyaltyFragmentActivity).CheckDrawerStatus();
                    (Activity as HomeActivity).SelectItem(LoyaltyFragmentActivity.ActivityTypes.DefaultItem);


                    //var upIntent = new Intent();
                    //upIntent.SetClass(Activity, typeof(HomeActivity));
                    ////upIntent.AddFlags(ActivityFlags.ClearTop);
                    ////upIntent.AddFlags(ActivityFlags.SingleTop);

                    //upIntent.PutExtra(BundleConstants.ChosenMenuBundleName, LoyaltyFragmentActivity.ActivityTypes.DefaultItem);

                    //StartActivity(upIntent);
                    //Activity.Finish();
                }




            }
            else
            {
                Activity.Finish();
            }
        }

        private async void Logon()
        {
            isLoggingIn = true;

            this.errorMessage.Visibility = ViewStates.Gone;

            View view = Activity.CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }



            if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text))
            {
                if (string.IsNullOrEmpty(username.Text))
                {
                    usernameInputLayout.Error = GetString(Resource.String.LoginViewUserNameEmpty);
                    usernameInputLayout.ErrorEnabled = true;
                }

                if (string.IsNullOrEmpty(password.Text))
                {
                    passwordInputLayout.Error = GetString(Resource.String.LoginViewPasswordEmpty);
                    passwordInputLayout.ErrorEnabled = true;
                }
            }
            else
            {
                var success = await memberContactModel.Login(username.Text, password.Text, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }
        }


        private async void FacebookLogon()
        {
            isLoggingIn = true;
            IsFacebookLogin = true;
            ShowProgressDialog();

            this.errorMessage.Visibility = ViewStates.Gone;

            View view = Activity.CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }


           // FaceBookEmail = "peterelpop.fcih@gmail.com";

            if (string.IsNullOrEmpty(FacebookID) || string.IsNullOrEmpty(FaceBookEmail))
            {
                if (string.IsNullOrEmpty(FacebookID))
                {

                }

                if (string.IsNullOrEmpty(FaceBookEmail))
                {

                }
            }
            else
            {
                var success =  await memberContactModel.FacbookLoginAsync(FacebookID, FaceBookEmail, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }


            IsFacebookLogin = false;
            LoginManager.Instance.LogOut();

            HideProgressDialog();
        }




        private async void GoogleLogon()
        {
            isLoggingIn = true;

            this.errorMessage.Visibility = ViewStates.Gone;

            View view = Activity.CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }


            if (string.IsNullOrEmpty(GoogleID) || string.IsNullOrEmpty(GoogleEmail))
            {

            }
            else
            {
                var success = await memberContactModel.GoogleLogin(GoogleID, GoogleEmail, ShowError);

                if (success)
                {
                    GoToMainScreen();
                }
            }
        }

        private void ShowError(string errorMessage)
        {
            isLoggingIn = false;

            username.ClearFocus();
            password.ClearFocus();


            try
            {
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(InputMethodService.InputMethodService);
                inputManager.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            }
            catch (Exception)
            {
                //View has now finished initializing and keyboard manipulations will not work
            }

            if (errorMessage.Equals(GetString(Resource.String.AccountViewFacebookInvalid)))
            {
                var createIntentintent = new Intent();

      

                ISharedPreferences  pref = PreferenceManager.GetDefaultSharedPreferences(this.Context);

                ISharedPreferencesEditor edit = pref.Edit();
                edit.PutString("FacebookID", FacebookID);
                edit.PutString("FaceBookEmail", FaceBookEmail);
                edit.PutString("FirstName", FirstName);
                edit.PutString("LastName", LastName);

                edit.PutString("GoogleID", "");
                edit.PutString("GoogleEmail", "");
                edit.PutInt("IntentCode", 1);

                edit.Apply();


                createIntentintent.SetClass(Activity, typeof(AccountActivity));
                StartActivityForResult(createIntentintent, CreateAccountRequestCode);
            }
            else if (errorMessage.Equals(GetString(Resource.String.AccountViewGoogleInvalid)))
            {

                var createIntentintent = new Intent();



                ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this.Context);
                ISharedPreferencesEditor edit = pref.Edit();
                edit.PutString("FacebookID", "");
                edit.PutString("FaceBookEmail", "");
                edit.PutString("GoogleID", GoogleID);
                edit.PutString("GoogleEmail", GoogleEmail);
                edit.PutString("FirstName", FirstName);
                edit.PutString("LastName", LastName);

                edit.PutInt("IntentCode", 2);

                edit.Apply();

                createIntentintent.SetClass(Activity, typeof(AccountActivity));
                StartActivityForResult(createIntentintent, CreateAccountRequestCode);


            }
            else {

                this.errorMessage.Visibility = ViewStates.Visible;
                this.errorMessage.Text = errorMessage;
            }

           
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Next)
            {
                password.RequestFocus();
                return true;
            }
            else if (actionId == ImeAction.Done)
            {
                Logon();
                return true;
            }
            return false;
        }


        public override void OnActivityResult(int requestCode, int resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
                GoogleLogon();

            }
            if (requestCode == CreateAccountRequestCode)
            {
                if (resultCode == (int)Result.Ok)
                {
                    deviceService.SaveDevice(AppData.Device);

                    GoToMainScreen();
                    //Toast.MakeText(this, GetString(Resource.String.LogonViewNetworkCreateAccountSuccessful), ToastLength.Long).Show();
                }
            }

        }



        public void AfterTextChanged(IEditable s)
        {
            if (usernameInputLayout.ErrorEnabled && !string.IsNullOrEmpty(username.Text))
            {
                usernameInputLayout.ErrorEnabled = false;
            }

            if (passwordInputLayout.ErrorEnabled && !string.IsNullOrEmpty(password.Text))
            {
                passwordInputLayout.ErrorEnabled = false;
            }
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

     

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.LoginViewLoginButton:
                    Logon();
                    break;

                case Resource.Id.LoginViewForgotPassword:
                    var forgotPasswordIntent = new Intent();
                    forgotPasswordIntent.SetClass(Activity, typeof(ForgotPasswordActivity));
                    StartActivity(forgotPasswordIntent);
                    break;

                case Resource.Id.LoginViewCreateAccountButton:
                    var createIntentintent = new Intent();
                    ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this.Context);

                    ISharedPreferencesEditor edit = pref.Edit();
                    edit.PutString("FacebookID", "");
                    edit.PutString("FaceBookEmail", "");
                    edit.PutString("FirstName", "");
                    edit.PutString("LastName", "");
                    edit.PutString("GoogleID", "");
                    edit.PutString("GoogleEmail", "");

                    edit.PutInt("IntentCode", 0);

                    edit.Apply();
                    createIntentintent.SetClass(Activity, typeof(AccountActivity));
                    StartActivityForResult(createIntentintent, CreateAccountRequestCode);
                    break;

                case Resource.Id.Google_sign_in_button:
                    SignIn();

                    break;


            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (viewSwitcher.CurrentView == contentView)
                    viewSwitcher.ShowNext();
            }
            else
            {
                if (viewSwitcher.CurrentView == progressView)
                    viewSwitcher.ShowPrevious();
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

#if CHANGEWS
            inflater.Inflate(Resource.Menu.ChangeWSMenu, menu);
#endif
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.ChangeWS:
                    var intent = new Intent();

                    intent.SetClass(Activity, typeof(SettingsActivity));
                    StartActivity(intent);

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
            GoogleID = "";
            GoogleEmail = "";
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                if (acct != null)

                {
                    GoogleEmail = acct.Email;
                    GoogleID = acct.Id;
                    FirstName = acct.DisplayName;
                    LastName = acct.FamilyName;
               
                }
              
                //UpdateUI(true);
            }
            else
            {
                // Signed out, show unauthenticated UI.
                //UpdateUI(false);
            }
        }

        void SignIn()
        {
            if (mGoogleApiClient.IsConnected)
            {
                SignOut();
            }

            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        void SignOut()
        {
            Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
        }

        void RevokeAccess()
        {
            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
        }

        public void ShowProgressDialog()
        {
            if (mProgressDialog == null)
            {
                mProgressDialog = new ProgressDialog(this.Activity);
                mProgressDialog.SetMessage(GetString(Resource.String.loading));
                mProgressDialog.Indeterminate = true;
            }

            mProgressDialog.Show();
        }

        public void HideProgressDialog()
        {
            if (mProgressDialog != null && mProgressDialog.IsShowing)
            {
                mProgressDialog.Hide();
            }
        }
        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
            Log.Debug(TAG, "onConnectionFailed:" + result);
        }

        void  mProfileTracker_mOnProfileChangedAsync(object sender, OnProfileChangedEventArgs e)
        {
            this.FacebookID = "";
            this.FaceBookEmail = "";
            this.FirstName = "";
            this.LastName = "";

            if (e.mProfile != null)
            {

                try
                {
                    FirstName = e.mProfile.FirstName;
                    LastName = e.mProfile.LastName;

                    // FaceBookEmail = LoginManager.Instance.GetType().GetProperty("email").ToString();

                    //  FaceBookEmail = e.mProfile.
                    this.FacebookID = e.mProfile.Id;
                    //  string mail1 =  GetEmailAsync(accessToken.Token);
                    //   TxtProfileID.Text = e.mProfile.Id;

                    GetEmail();


                }
                catch (Java.Lang.Exception ) { }

               


            }
            else
            {
                FirstName = "";
                LastName = "";
                //  FaceBookEmail = e.mProfile.
                this.FacebookID = "";
                //   TxtProfileID.Text = e.mProfile.Id;



            }
        }

        public void OnCancel()
        {
        }

        public void OnError(FacebookException error)
        {
        }

        public void OnSuccess(Java.Lang.Object result)
        {


            var id = 1;
            FacebookLogon();

        }

        public void GetEmail()
        {
            
            GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, (this));

            Bundle parameters = new Bundle();
            parameters.PutString("fields", "id,name,age_range,email");
            request.Parameters = parameters;
             request.ExecuteAsync();
           



        }

        public void OnCompleted(JSONObject @object, GraphResponse response)
        {
            if (@object == null)
                return;
            string data = @object.ToString();
            FacebookResult result = JsonConvert.DeserializeObject<FacebookResult>(data);
            if (result != null)

            {
                if (string.IsNullOrEmpty(FaceBookEmail))
                {
                    FaceBookEmail = result.email;
                    //if (!IsFacebookLogin)
                        //     FacebookLogon();
                        FacebookLogon();

                    //else return;
                }
               

            }


        }
    }
    

    class FacebookResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}
