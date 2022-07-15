﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using static Android.Media.MediaPlayer;

namespace FormsLoyalty.Droid
{
    #region Video Splash
    [Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        MainLauncher = true,
        Theme = "@style/SplashTheme",
    NoHistory = true)]
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
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //var apiCallService = new Runnable(async () =>
            //{
            //    await App.CallPCLMethod().ConfigureAwait(false);

            //});
            //apiCallService.Run();

            App.CallPCLMethod();



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

            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.AutoReset = false; // the key is here so it repeats
            timer.Elapsed += timer_elapsed;
            timer.Start();


            // videoHolder.SetVideoPath(videoPath);
            mVideoView.SetOnCompletionListener(this);
            mVideoView.Start();
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            var mainActivityIntent = new Intent(this, typeof(MainActivity));
            mainActivityIntent.AddFlags(ActivityFlags.NoAnimation); //Add this line

            StartActivity(mainActivityIntent);
        }

        public void OnCompletion(MediaPlayer mp)
        {
            if (IsFinishing)
                return;


            // Finish();
        }
    }
    #endregion
    //[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    //public class SplashActivity: AppCompatActivity
    //{
    //    static readonly string TAG = "X:" + typeof(SplashActivity).Name;

    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState);
    //        Rg.Plugins.Popup.Popup.Init(this);
    //        FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
    //        Xamarin.Essentials.Platform.Init(this,savedInstanceState);
    //        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
    //        App.CallPCLMethod();
    //    }

    //    // Launches the startup task
    //    protected override void OnResume()
    //    {
    //        base.OnResume();
    //        //Task startupWork = new Task(() => { SimulateStartup(); });
    //        //startupWork.Start();
    //        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
    //    }

    //    // Prevent the back button from canceling the startup process
    //    public override void OnBackPressed() { }

    //    // Simulates background work that happens behind the splash screen
    //     void SimulateStartup()
    //    {


    //        Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
    //        //var apiCallService = new Runnable(async () =>
    //        //{
    //        //    await App.CallPCLMethod().ConfigureAwait(false);

    //        //});
    //        //apiCallService.Run();

    //        //Rg.Plugins.Popup.Popup.Init(this);
    //        //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
    //        //Xamarin.Essentials.Platform.Init(Application);
    //        ////global::Xamarin.Forms.Forms.Init(Xamarin.Essentials.Platform.CurrentActivit);
    //        //App.CallPCLMethod();



    //        //await Task.Delay(3000); // Simulate a bit of startup work.
    //        Log.Debug(TAG, "Startup work is finished - starting MainActivity.");

    //    }
    //}
}