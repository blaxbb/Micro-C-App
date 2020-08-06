using micro_c_app.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class ItemDetailsViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailsViewModel()
        {
            Title = "Details";
        }
    }
}