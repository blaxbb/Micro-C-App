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
        public List<Item> Result {
            get
            {
                if(BindingContext is ImportPageViewModel vm)
                {
                    return vm.Result;
                }
                return null;
            }
        }

        public delegate void ImportResultsHandler(object sender, List<Item> result);
        public event ImportResultsHandler OnImportResults;

        public ImportPage()
        {
            InitializeComponent();
        }

        public ImportPage(string folder)
        {
            InitializeComponent();
            if(BindingContext is ImportPageViewModel vm)
            {
                vm.Folder = folder;
                vm.ListLocalFiles.Execute(null);
            }
        }

        public static async Task<ImportPage> Create(string folder)
        {
            var page = new ImportPage(folder);
            await Shell.Current.Navigation.PushModalAsync(page);
            return page;
        }

        protected override void OnDisappearing()
        {
            if(Result != null)
            {
                OnImportResults?.Invoke(this, Result);
            }
            base.OnDisappearing();
        }
    }
}