using micro_c_app.Models;
using System;
using System.Linq;
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
        public ICommand ProductFound { get; }
        public ICommand SearchError { get; }
        public BuildComponentViewModel()
        {
            Title = "Details";

            ProductFound = new Command<Item>(async (item) =>
            {
                Component.Item = item;
                MessagingCenter.Send(this, "selected");
            });

            SearchError = new Command<string>(async (message) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                });
            });
        }
    }
}