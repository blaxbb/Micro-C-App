using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferencePage : ContentPage
    {
        public ReferencePage()
        {
            InitializeComponent();

            if (BindingContext is ReferencePageViewModel vm)
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    System.Diagnostics.Debug.WriteLine("found resource: " + res);
                    if (Regex.IsMatch(res, "micro_c_app\\.Assets\\.(.*?)\\.txt"))
                    {
                        var stream = assembly.GetManifestResourceStream(res);
                        using var reader = new StreamReader(stream);
                        var text = reader.ReadToEnd();
                        vm.Items.Add(new MicroCLib.Models.PriceReference($"/Ref/{res}", ("A", 0f)));
                    }
                }
                vm.SetPath("/");
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if(BindingContext is ReferencePageViewModel vm)
            {
                vm.ItemTapped?.Execute(null);
            }
        }
    }
}