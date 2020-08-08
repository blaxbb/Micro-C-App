using micro_c_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class QuotePageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand IncreaseQuantity { get; }
        public ICommand DecreaseQuantity { get; }
        public ICommand RemoveItem { get; }

        public ICommand SendQuote { get; }

        public float Subtotal => Items.Sum(i => i.Price * i.Quantity);
        public float TaxedTotal => Subtotal * 1.075f;
        public QuotePageViewModel()
        {
            Title = "Quote";
            Items = new ObservableCollection<Item>();
            for(int i = 0; i < 10; i++)
            {
                var item = new Item()
                {
                    Name = "ITEM",
                    SKU = "123456",
                    OriginalPrice = 100,
                    Price = 90,
                    Stock = "5 in stock",
                };
                Items.Add(item);
            }

            Items.CollectionChanged += (sender, args) =>
            {
                UpdateProperties();
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Items.Add(item);
                }
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                await Shell.Current?.DisplayAlert("Error", message, "Ok");
            });

            IncreaseQuantity = new Command<Item>((Item item) =>
            {
                item.Quantity++;
                UpdateProperties();
            });

            DecreaseQuantity = new Command<Item>((Item item) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    UpdateProperties();
                }
            });
            RemoveItem = new Command<Item>((Item item) =>
            {
                Items.Remove(item);
            });

            SendQuote = new Command(DoSendQuote);

        }

        private async void DoSendQuote(object obj)
        {
            try
            {
                var message = new EmailMessage()
                {
                    Subject = $"MicroCenter Quote - {DateTime.Today.ToShortDateString()}",
                    Body = ExportTxtTable(Items),
                };

                await Email.ComposeAsync(message);
            }
            catch(Exception e)
            {
                Shell.Current.DisplayAlert("Error", e.ToString(), "Ok");
            }
        }

        public static string ExportTxtTable(IEnumerable<Item> items)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"SKU\t\t{string.Format("{0,-40}", "Name")}\tQty  Unit      Price");
            b.AppendLine();
            foreach (var item in items)
            {
                b.AppendLine($"{item.SKU}\t{string.Format("{0,-40}", item.Name)}\t{item.Quantity}\t\t${item.Price.ToString("#0.00")}\t\t${(item.Price * item.Quantity).ToString("#0.00")}");
            }

            var Subtotal = items.Sum(i => i.Price * i.Quantity);
            var TaxedTotal = Subtotal * 1.075f;

            b.AppendLine();
            b.AppendLine(string.Format("{0,78}", $"Sub ${Subtotal.ToString("#0.00")}"));
            b.AppendLine(string.Format("{0,78}", $"Total ${TaxedTotal.ToString("#0.00")}"));

            b.AppendLine();

            var salesId = Preferences.Get("sales_id", "SALESID");
            b.AppendLine($"Quote created by {salesId} for additional help contact me at {salesId}@microcenter.com");

            return b.ToString();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
        }
    }
}