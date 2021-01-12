using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.RemoteConfig;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace micro_c_app.iOS
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
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags(new string[] { "CollectionView_Experimental", "Shapes_Experimental" });
            global::Xamarin.Forms.Forms.Init();

            UNUserNotificationCenter.Current.Delegate = new iOSNotificationReceiver();

            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            var notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager?.Initialize();


            LoadApplication(new App());

            GoogleVisionBarCodeScanner.iOS.Initializer.Init();
            // Temporary work around for bug on Firebase Library
	        // https://github.com/xamarin/GoogleApisForiOSComponents/issues/368
	        Firebase.Core.App.Configure();
	        RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings();


            return base.FinishedLaunching(app, options);
        }
    }
}
