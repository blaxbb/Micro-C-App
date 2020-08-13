using micro_c_app.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildComponentViewModel : BaseViewModel
    {
        private BuildComponent component;

        public BuildComponent Component { get => component; set { SetProperty(ref component, value); OnPropertyChanged(nameof(ItemExists)); } }
        public ICommand SubmitButton { get; }
        public ICommand ProductFound { get; }
        public ICommand SearchError { get; }
        public ICommand Remove { get; }
        public bool ItemExists => Component?.Item != null;
        public BuildComponentViewModel()
        {
            Title = "Details";

            ProductFound = new Command<Item>((item) =>
            {
                var isNew = Component.Item == null;
                Component.Item = item;
                if (isNew)
                {
                    MessagingCenter.Send(this, "new");
                }
                else
                {
                    MessagingCenter.Send(this, "selected");
                }
            });

            SearchError = new Command<string>(async (message) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                });
            });

            Remove = new Command(() =>
            {
                component.Item = null;
                MessagingCenter.Send(this, "removed");
            });
        }
    }
}