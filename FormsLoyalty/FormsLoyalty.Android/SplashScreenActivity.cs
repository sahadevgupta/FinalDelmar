using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using static Android.Media.MediaPlayer;

namespace FormsLoyalty.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        MainLauncher = true,
        Theme = "@style/SplashTheme",
    NoHistory = true,
    ScreenOrientation = ScreenOrientation.Portrait,
    LaunchMode = LaunchMode.SingleTop)]
    public class SplashScreenActivity : Activity, IOnCompletionListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
          
            SetContentView(Resource.Layout.splash);
            RelativeLayout rootView = (RelativeLayout)FindViewById(Resource.Id.rootLayout);
            rootView.SetBackgroundColor(Color.Black);
            Display display = WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);


            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var apiCallService = new Runnable(async () =>
            {
                await App.CallPCLMethod().ConfigureAwait(false);

            });
            apiCallService.Run();



            FrameLayout.LayoutParams rootViewParams = (FrameLayout.LayoutParams)rootView.LayoutParameters;
            int videoWidth = 1280;
            int videoHeight = 2920;

            if ((float)videoWidth / (float)videoHeight < (float)size.X / (float)size.Y)
            {
                rootViewParams.Width = size.X;
                rootViewParams.Height = Convert.ToInt32(DeviceDisplay.MainDisplayInfo.Height) + 100;
                rootView.SetX(0);
                rootView.SetY((rootViewParams.Height - size.Y) / 2 * -1);
            }
            else
            {
                rootViewParams.Width = videoWidth * size.Y / videoHeight;
                rootViewParams.Height = size.Y;
                rootView.SetX((rootViewParams.Width - size.X) / 2 * -1);
                rootView.SetY(0);
            }

            rootView.LayoutParameters = rootViewParams;

            VideoView mVideoView = (VideoView)FindViewById(Resource.Id.video_view);

            var uri = Android.Net.Uri.Parse($"android.resource://{PackageName}/{Resource.Raw.Intro}");
            // var videoPath = $"android.resource://{PackageName}/resou";


            mVideoView.SetVideoURI(uri);
            mVideoView.RequestFocus();




            // videoHolder.SetVideoPath(videoPath);
            mVideoView.SetOnCompletionListener(this);
            mVideoView.Start();
        }
        public void OnCompletion(MediaPlayer mp)
        {
            if (IsFinishing)
                return;
           
            var mainActivityIntent = new Intent(this, typeof(MainActivity));
            mainActivityIntent.AddFlags(ActivityFlags.NoAnimation); //Add this line

            StartActivity(mainActivityIntent);
           // Finish();
        }



    }
}