using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportPage : ContentPage
    {
        public delegate void ImportResultsHandler(ImportPage sender);
        public event ImportResultsHandler? OnImportResults;

        public ImportPage()
        {
            InitializeComponent();
        }

        public static async Task<ImportPage> Create<T>(string folder)
        {
            var page = new ImportPage();
            page.BindingContext = new ImportPageViewModel<T>()
            {
                Folder = folder,
            };

            await Shell.Current.Navigation.PushModalAsync(page);
            return page;
        }

        protected override void OnDisappearing()
        {
            OnImportResults?.Invoke(this);
            base.OnDisappearing();
        }
    }
}