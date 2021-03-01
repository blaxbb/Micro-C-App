using DataFlareClient;
using micro_c_app.Views;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ExportPageViewModel : BaseViewModel
    {
        private List<BuildComponent> components;

        public List<BuildComponent> Components { get => components; set => SetProperty(ref components, value); }
        public ICommand SetTitleCommand { get; }

        private string title;
        private Flare exportFlare;

        public string Title { get => title; set => SetProperty(ref title, value); }

        public Flare ExportFlare { get => exportFlare; set => SetProperty(ref exportFlare, value); }

        public ExportPageViewModel()
        {
            components = new List<BuildComponent>();

            SetTitleCommand = new Command<string>(DoSetTitle);
        }

        private async void DoSetTitle(string s)
        {
            var title = s == null ? "" : s;

            var flare = new Flare(JsonConvert.SerializeObject(components))
            {
                Title = title
            };
            flare.Tag = $"micro-c-{SettingsPage.StoreID()}";
            var success = await flare.Post("https://dataflare.bbarrett.me/api/Flare");
            if (success)
            {
                await Shell.Current.DisplayAlert("Import using code", $"{flare.ShortCode}", "Ok");
                ExportFlare = flare;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to export to DataFlare server.", "Ok");
            }

        }
    }
}
