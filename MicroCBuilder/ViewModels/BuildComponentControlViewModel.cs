using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MicroCBuilder.ViewModels
{
    public class BuildComponentControlViewModel : BaseViewModel
    {
        private BuildComponent.ComponentType componentType;
        private Item selectedItem;
        public Item SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }
        public BuildComponent.ComponentType ComponentType { get => componentType; set => SetProperty(ref componentType, value); }

        public BuildComponentControlViewModel()
        {
            
        }
    }
}
