using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportPage : ContentPage
    {
        public ExportPage()
        {
            InitializeComponent();
        }

        public ExportPage(List<BuildComponent> components, string name, string folder)
        {
            InitializeComponent();
            if(BindingContext is ExportPageViewModel vm)
            {
                vm.Components = components;
                vm.Folder = folder;
                vm.Name = name;
                vm.SendFlare.Execute(null);
            }
        }

        public async static Task Create(List<BuildComponent> items, string folder)
        {
            var name = await Shell.Current.DisplayPromptAsync("Save", "Title");
            if(name == null)
            {
                return;
            }

            await Shell.Current.Navigation.PushModalAsync(
                new ExportPage(items, name, folder)
            );
        }
    }
}