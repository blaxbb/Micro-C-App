using DataFlareClient;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MicroCBuilder.ViewModels
{
    public class LandingPageViewModel : BaseViewModel
    {
        public Command<string> NewBuildCommand { get; }
        public ObservableCollection<BuildInfo> BuildTemplates { get; }

        public delegate void CreateBuildEventHandler(object sender, BuildInfo name);
        public event CreateBuildEventHandler OnCreateBuild;

        public ObservableCollection<FlareInfo> Flares { get; set; }
        public Command UpdateNetworkFlares { get; }

        public LandingPageViewModel()
        {
            BuildTemplates = new ObservableCollection<BuildInfo>();
            NewBuildCommand = new Command<string>((name) => OnCreateBuild?.Invoke(this, GetInfo(name)));

            Flares = new ObservableCollection<FlareInfo>();

            UpdateNetworkFlares = new Command(async (o) =>
            {
                var flares = (await Flare.GetTag("https://dataflare.bbarrett.me/api/Flare", $"micro-c-{Settings.StoreID()}"));

                var toAdd = flares.Where(f => Flares.All(existing => existing.Flare.ShortCode != f.ShortCode)).OrderBy(f => f.Created).ToList();
                foreach(var f in toAdd)
                {
                    Flares.Insert(0, new FlareInfo(f));
                }

                var toRemove = Flares.Where(f => flares.All(updated => updated.ShortCode != f.Flare.ShortCode)).ToList();
                foreach(var f in toRemove)
                {
                    var index = Flares.ToList().FindIndex(check => check.Flare.ShortCode == f.Flare.ShortCode);
                    if(index >= 0 && index < Flares.Count)
                    {
                        Flares.RemoveAt(index);
                    }
                }

                if (toAdd.Count > 0 || toRemove.Count > 0)
                {
                    OnPropertyChanged(nameof(Flares));
                }
            });
            UpdateNetworkFlares.Execute(null);

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

    public class FlareInfo
    {
        public Flare Flare { get; set; }
        public List<BuildComponent> Components => (List<BuildComponent>)Flare.Value(typeof(List<BuildComponent>));
        public float Total => Components.Sum(c => c?.Item.Price ?? 0 * c?.Item.Quantity ?? 0);

        public FlareInfo(Flare flare)
        {
            Flare = flare;
        }
    }
}
