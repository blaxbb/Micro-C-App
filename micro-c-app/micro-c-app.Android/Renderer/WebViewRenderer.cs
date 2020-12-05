using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using micro_c_app.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(WebViewRenderer))]
namespace micro_c_app.Droid.Renderer
{
    public class WebViewRenderer : Xamarin.Forms.Platform.Android.WebViewRenderer
    {
        public WebViewRenderer(Context context) : base(context)
        {

        }

        protected override Android.Webkit.WebView CreateNativeControl()
        {
            var ret = base.CreateNativeControl();
            ret.SetBackgroundColor(Android.Graphics.Color.Transparent);
            return ret;
        }
    }
}