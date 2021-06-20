using DataFlareClient;
using micro_c_app.Views;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ExportPageViewModel : BaseViewModel
    {
        private List<BuildComponent> components;

        public List<BuildComponent> Components { get => components; set => SetProperty(ref components, value); }
        public ICommand SetTitleCommand { get; }
        public ICommand OpenWebCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand SendFlare { get; }

        private string name;
        private Flare exportFlare;
        private bool isError;
        private bool loadingFlare;

        public string Name { get => name; set => SetProperty(ref name, value); }

        public Flare ExportFlare { get => exportFlare; set => SetProperty(ref exportFlare, value); }
        public bool IsError { get => isError; set => SetProperty(ref isError, value); }

        public string? Folder { get; set; }

        public string FolderPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Folder);
        public string Path(string filename) => System.IO.Path.Combine(FolderPath, filename);

        public bool LoadingFlare { get => loadingFlare; set => SetProperty(ref loadingFlare, value); }

        public ExportPageViewModel()
        {
            components = new List<BuildComponent>();

            //SetTitleCommand = new Command<string>(DoSetTitle);
            OpenWebCommand = new Command(async () =>
            {
                await Xamarin.Essentials.Browser.OpenAsync($"https://microc.bbarrett.me/{Folder}s/{ExportFlare.ShortCode}");
                await Shell.Current.Navigation.PopModalAsync();
            });
            ShareCommand = new Command(async () => { await DoSendQuote(components?.Where(c => c.Item != null).Select(c => c.Item)); });
            SaveCommand = new Command(() => { DoSave(); });
            SendFlare = new Command(async () => await DoSendFlare());
        }

        public async Task DoSendFlare()
        {
            LoadingFlare = true;
            var flare = new Flare(JsonConvert.SerializeObject(components))
            {
                Title = Name
            };
            flare.Tag = $"micro-c-{SettingsPage.StoreID()}";
            var success = await flare.Post("https://dataflare.bbarrett.me/api/Flare");
            if (success)
            {
                IsError = false;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to export to DataFlare server.", "Ok");
                IsError = true;
            }
            ExportFlare = flare;
            LoadingFlare = false;
        }

        public static async Task DoSendQuote(IEnumerable<Item> Items)
        {
            try
            {
                var message = new EmailMessage()
                {
                    Subject = $"MicroCenter Quote - {DateTime.Today.ToShortDateString()}",
                    Body = ExportTxtTable(Items),
                };

                await Share.RequestAsync(new ShareTextRequest()
                {
                    Title = message.Subject,
                    Text = message.Body,
                });
                await Shell.Current.Navigation.PopModalAsync();
            }
            catch (Exception e)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", e.ToString(), "Ok");
                });
            }
        }

        public static string ExportTxtTable(IEnumerable<Item> items)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"SKU         {string.Format("{0,-50}", "Name")}  Qty  Unit       Price");
            b.AppendLine();
            foreach (var item in items)
            {
                b.AppendLine($"{item.SKU}    {string.Format("{0,-40}", item.Name.Substring(0, Math.Min(item.Name.Length, 25)))}  {item.Quantity}    ${item.Price:#0.00}    ${item.Price * item.Quantity:#0.00}");
            }

            var Subtotal = items.Sum(i => i.Price * i.Quantity);
            var TaxedTotal = Subtotal * SettingsPage.TaxRateFactor();

            b.AppendLine();
            b.AppendLine(string.Format("{0,78}", $"Sub ${Subtotal:#0.00}"));
            b.AppendLine(string.Format("{0,78}", $"Total ${TaxedTotal:#0.00}"));

            b.AppendLine();

            var salesId = Preferences.Get("sales_id", "SALESID");
            if (!string.IsNullOrWhiteSpace(salesId))
            {
                //b.AppendLine($"Quote created by {salesId} for additional help contact me at {salesId}@microcenter.com");
            }

            return b.ToString();
        }

        private async void DoSave()
        {
            var (result, text) = ValidateFilename(Name);

            if (result)
            {
                if (File.Exists(Path(Name)))
                {
                    var overwrite = await Device.InvokeOnMainThreadAsync<bool>(async () =>
                    {
                        return await Shell.Current.DisplayAlert("Save", $"File {Name} exists, would you like to overwrite?", "Ok", "Cancel");
                    });
                    if (!overwrite)
                    {
                        return;
                    }
                }

                await SaveFile();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", text, "Okay");
            }

            await Shell.Current.Navigation.PopModalAsync();
        }

        private static (bool result, string? text) ValidateFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return (false, "Error: Filename is empty");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(filename, "^[a-zA-Z-_0-9]+$"))
            {
                return (false, "Error: Only characters a-Z 0-9 - _ allowed.");
            }

            return (true, default);
        }

        private async Task SaveFile()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    string text;
                    if (Folder == "quote")
                    {
                        text = System.Text.Json.JsonSerializer.Serialize(components.Where(c => c.Item != null).Select(c => c.Item));
                    }
                    else
                    {
                        text = System.Text.Json.JsonSerializer.Serialize(components.Where(c => c.Item != null));
                    }
                    File.WriteAllText(Path(Name), text);
                    await Shell.Current.DisplayAlert("Success", Path(Name), "Ok");
                });
            }
            catch (Exception e)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", e.ToString(), "Ok");
                });
            }
        }
    }
}
