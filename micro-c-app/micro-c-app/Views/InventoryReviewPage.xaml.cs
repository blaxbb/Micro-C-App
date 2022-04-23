using micro_c_app.ViewModels;
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
    public partial class InventoryReviewPage : ContentPage
    {
        public InventoryReviewPage()
        {
            InitializeComponent();
        }

        private void RemoveEntryClicked(object sender, EventArgs e)
        {
            if(sender is Button b)
            {
                var ctx = b.BindingContext;
                var grid = FindParent<Grid>(b);
                if(grid == null)
                {
                    return;
                }
                var grid2 = FindParent<Grid>(grid);
                if(grid2 == null)
                {
                    return;
                }

                var parentCtx = grid2.BindingContext;
                if(ctx is string entry && parentCtx is KeyValuePair<string, List<string>> loc)
                {
                    Console.WriteLine($"{loc.Key} + {entry}");
                    if(BindingContext is InventoryReviewViewModel vm)
                    {
                        vm.RemoveEntry(loc.Key, entry);
                    }
                }
            }
            Console.WriteLine(sender);
        }

        private void RemoveLocationClicked(object sender, EventArgs e)
        {
            if (sender is Button b)
            {
                var ctx = b.BindingContext;
                if (ctx is KeyValuePair<string, List<string>> loc)
                {
                    Console.WriteLine($"{loc.Key}");
                    if (BindingContext is InventoryReviewViewModel vm)
                    {
                        vm.RemoveLocation(loc.Key);
                    }
                }
            }
        }

        T FindParent<T>(Element view)
        {
            if(view.Parent == null)
            {
                return default;
            }

            if(view.Parent is T t)
            {
                return t;
            }

            return FindParent<T>(view.Parent);
        }
    }
}