using Foundation;
using micro_c_app.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace micro_c_app.iOS
{
    public class BaseUrl : IBaseUrl
    {
        public string Get => NSBundle.MainBundle.BundlePath;
    }
}