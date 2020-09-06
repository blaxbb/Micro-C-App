using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : ContentPage
    {
        public event EventHandler<SelectionChangedEventArgs> ItemTapped;
        public bool AutoPop { get; set; }
        public SearchResultsPage()
        {
            InitializeComponent();
            ItemsList.SelectionChanged += ItemsList_ItemTapped;
        }

        private void ItemsList_ItemTapped(object sender, SelectionChangedEventArgs e)
        {
            ItemTapped?.Invoke(sender, e);
            if (AutoPop)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.Navigation.PopAsync();
                });
            }
        }
    }
}