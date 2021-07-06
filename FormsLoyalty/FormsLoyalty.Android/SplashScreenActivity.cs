using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using static Android.Media.MediaPlayer;

namespace FormsLoyalty.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTask,
        MainLauncher = true,
        Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class SplashScreenActivity : Activity, IOnCompletionListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            base.SetTheme(Resource.Style.MainTheme);
            SetContentView(Resource.Layout.splash);
            RelativeLayout rootView = (RelativeLayout)FindViewById(Resource.Id.rootLayout);
            rootView.SetBackgroundColor(Color.Black);
            Display display = WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);


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
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }



    }
}