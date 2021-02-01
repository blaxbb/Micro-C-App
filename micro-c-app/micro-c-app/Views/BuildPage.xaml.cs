using micro_c_app.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildPage : ContentPage
    {
        public BuildPage()
        {
            InitializeComponent();
            this.SetupActionButton();
            if(BindingContext is BuildPageViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(BuildPageViewModel.Components))
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(1);
                    var items = listView.ItemsSource;
                    listView.ItemsSource = null;
                    listView.ItemsSource = items;
                });
            }
        }
    }
}