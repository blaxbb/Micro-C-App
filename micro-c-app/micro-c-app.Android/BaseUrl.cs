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

[assembly: Dependency(typeof(BaseUrl))]
namespace micro_c_app.Droid
{
    public class BaseUrl : IBaseUrl
    {
        public string Get => "file:///android_asset/";
    }
}