using micro_c_app.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildPage : ContentPage
    {
        BuildPageViewModel viewModel => (BuildPageViewModel)BindingContext;
        //HttpClient client;
        public BuildPage()
        {
            InitializeComponent();
            this.SetupActionButton();
            viewModel.Navigation = Navigation;
        }
    }
}