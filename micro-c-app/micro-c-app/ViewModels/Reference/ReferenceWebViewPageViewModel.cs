using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace micro_c_app.ViewModels.Reference
{
    public class ReferenceWebViewPageViewModel : BaseViewModel
    {
        private string text;
        public string Text { get => text; set => SetProperty(ref text, value); }
    }
}