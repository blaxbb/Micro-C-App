using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailsPage : ContentPage
    {
        private Item item;

        public Item Item { get => item; set { item = value; detailView.Item = value; } }
        public ItemDetailsPage()
        {
            InitializeComponent();
            this.SetupActionButton();
        }
    }
}