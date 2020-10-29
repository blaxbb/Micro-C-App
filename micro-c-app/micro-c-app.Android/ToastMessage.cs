using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using micro_c_app.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastMessage))]
namespace micro_c_app.Droid
{
    //https://stackoverflow.com/a/44126899
    public class ToastMessage : IToastMessage
    {
        public void LongAlert(string message)
        {
            ShowToast(message, ToastLength.Short);
        }

        public void ShortAlert(string message)
        {
            ShowToast(message, ToastLength.Short);
        }

        void ShowToast(string text, ToastLength length)
        {
            Handler mainHandler = new Handler(Looper.MainLooper);
            Java.Lang.Runnable runnableToast = new Java.Lang.Runnable(() =>
            {
                Toast.MakeText(Forms.Context, text, length).Show();
            });

            mainHandler.Post(runnableToast);
        }
    }
}