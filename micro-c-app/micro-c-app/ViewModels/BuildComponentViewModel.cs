using micro_c_app.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildComponentViewModel : BaseViewModel
    {
        private BuildComponent component;

        public BuildComponent Component { get => component; set => SetProperty(ref component, value); }
        public ICommand SubmitButton { get; }
        public BuildComponentViewModel()
        {
            Title = "Details";
            SubmitButton = new Command(() =>
            {
                MessagingCenter.Send(this, "selected");
            });
        }
    }
}