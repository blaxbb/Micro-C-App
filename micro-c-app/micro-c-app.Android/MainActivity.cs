﻿
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Google.Android.Material.Internal;
using Xamarin.Essentials;

namespace micro_c_app.Droid
{
    [Activity(Label = "Micro C App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.SetFlags("Shapes_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            BarcodeScanner.Mobile.Methods.Droid.RendererInitializer.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override async void OnNewIntent(Intent intent)
        {
            if (intent.HasExtra("inventory"))
            {
                System.Console.WriteLine($"INVENTORY - {intent.GetStringExtra("inventory")}");
                var manager = (NotificationManager)Application.Context.GetSystemService(NotificationService);

                manager.CancelAll();


                await Xamarin.Forms.Shell.Current.Navigation.PushAsync(new Views.InventoryLandingPage());

            }

            base.OnNewIntent(intent);
        }
    }
}