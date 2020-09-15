using micro_c_lib.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCBuilder
{
    public class BuildComponentCache
    {
        private const string FILENAME = "componentcache.json";
        public static BuildComponentCache Current;
        public Dictionary<string, List<Item>> Cache = new Dictionary<string, List<Item>>();

        public BuildComponentCache()
        {
            Current = this;
        }

        public async Task<bool> LoadCache()
        {
            if (File.Exists($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}/{FILENAME}"))
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(FILENAME);
                if (file != null)
                {
                    var text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    Cache = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<Item>>>(text);
                    return true;
                }
            }

            return false;
        }

        public async Task PopulateCache(IProgress<int> progress)
        {
            var types = Enum.GetValues(typeof(BuildComponent.ComponentType)).Cast<BuildComponent.ComponentType>().ToList();
            for(int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var category = BuildComponent.CategoryFilterForType(type);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var items = await Search.LoadAll(null, "141", category, Search.OrderByMode.pricelow);
                    Cache[category] = items.Items;
                }
                var percent = (int)Math.Round(((float)(i+1) / types.Count) * 100);
                progress.Report(percent);
            }
            var json = System.Text.Json.JsonSerializer.Serialize(Cache, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(FILENAME, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);

        }

        public List<Item> FromType(BuildComponent.ComponentType type)
        {
            var category = BuildComponent.CategoryFilterForType(type);
            if (Cache.ContainsKey(category))
            {
                return Cache[category];
            }

            return new List<Item>();
        }
    }
}
