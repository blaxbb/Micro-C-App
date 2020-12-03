using Foundation;
using micro_c_app.iOS.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(micro_c_app.iOS.Renderer.WebViewRenderer))]
namespace micro_c_app.iOS.Renderer
{
    public class WebViewRenderer : Xamarin.Forms.Platform.iOS.WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            this.Opaque = false;
            base.OnElementChanged(e);
        }
    }
}