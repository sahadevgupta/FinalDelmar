using AVKit;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.AppCenter.Crashes;
using PanCardView.iOS;
using Plugin.FacebookClient;
using Plugin.FirebasePushNotification;
using Plugin.GoogleClient;
using Prism;
using Prism.Ioc;
using System;
using System.Drawing;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = System.Drawing.Color;

namespace FormsLoyalty.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        bool IsAnimationOver = true;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

           
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::Xamarin.Forms.Forms.Init();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            var control = new VideoController();                          //Video Splash Screen 
            Window.RootViewController = control;
            Window.MakeKeyAndVisible();

            Xamarin.FormsMaps.Init();
            global::Xamarin.Forms.FormsMaterial.Init();
           FacebookClientManager.Initialize(app, options);
           GoogleClientManager.Initialize();
            ImageCircleRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            XF.Material.iOS.Material.Init();
            CardsViewRenderer.Preserve();
            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                               UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                               new NSSet());

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            // SetStatusBarColor();
            // GlobalExceptionHelper.RegisterForGlobalExceptionHandling();
            FirebasePushNotificationManager.Initialize(options);
            MessagingCenter.Subscribe<object, object>(this, "ShowMainScreen", (sender, args) =>
            {
                if (IsAnimationOver)
                {

                    System.Diagnostics.Debug.WriteLine("IsAnimation over ");
                    LoadApplication(new App(new iOSInitializer()));
                    base.FinishedLaunching(app, options);
                    IsAnimationOver = false;
                }
            });
            return true;
            //LoadApplication(new App(new iOSInitializer()));




            // return base.FinishedLaunching(app, options); ;
        }

        private void SetStatusBarColor()
        {
            UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            if (statusBar != null && statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
            {
                // change to your desired color 
                statusBar.BackgroundColor = Xamarin.Forms.Color.FromHex("#b72228").ToUIColor();
            }
        }

        #region Push Notification
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }
        #endregion

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            FacebookClientManager.OnActivated();
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
             GoogleClientManager.OnOpenUrl(app, url, options);
            return FacebookClientManager.OpenUrl(app, url, options);
        }
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return FacebookClientManager.OpenUrl(application, url, sourceApplication, annotation);
        }

        


    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }

    public static class GlobalExceptionHelper
    {
        public static void RegisterForGlobalExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var newExc = new System.Exception("CurrentDomainOnUnhandledException",
                args.ExceptionObject as System.Exception);
                Crashes.TrackError(newExc);
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                var newExc = new ApplicationException(
                    "TaskSchedulerOnUnobservedTaskException",
                    args.Exception);
                Crashes.TrackError(newExc);
            };
        }
    }


   

    
}
