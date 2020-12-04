using micro_c_app.Models.Reference;
using micro_c_app.ViewModels.Reference;
using micro_c_app.Views;
using micro_c_app.Views.Reference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace micro_c_app.ViewModels
{
    public class ReferenceIndexPageViewModel : BaseViewModel
    {
        private List<IReferenceItem> nodes;
        public List<IReferenceItem> Nodes { get => nodes; set => SetProperty(ref nodes, value); }
        public ICommand SelectedCommand { get; }

        public ReferenceIndexPageViewModel()
        {
            Nodes = new List<IReferenceItem>();

            SelectedCommand = new Command<IReferenceItem>(async(node) => await ReferenceIndexPage.NavigateTo(node));
        }
    }
}
