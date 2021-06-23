using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCBuilder.ViewModels
{
    public class LandingPageViewModel : BaseViewModel
    {
        public Command<string> NewBuildCommand { get; }
        public ObservableCollection<BuildInfo> BuildTemplates { get; }

        public delegate void CreateBuildEventHandler(object sender, BuildInfo name);
        public event CreateBuildEventHandler OnCreateBuild;

        public LandingPageViewModel()
        {
            BuildTemplates = new ObservableCollection<BuildInfo>();
            NewBuildCommand = new Command<string>((name) => OnCreateBuild?.Invoke(this, GetInfo(name)));
        }

        private BuildInfo GetInfo(string name)
        {
            return BuildTemplates.FirstOrDefault(b => b.Name == name);
        }
    }
    public class BuildInfo
    {
        public string Name { get; set; }
        public List<BuildComponent> Components { get; set; }
        public float Total => Components.Sum(c => c?.Item.Price ?? 0 * c?.Item.Quantity ?? 0);
    }
}
