using micro_c_lib.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCBuilder
{
    public class BuildComponentCache
    {
        private const string FILENAME = "componentcache.json";
        public static BuildComponentCache? Current;
        public Dictionary<string, List<Item>> Cache = new Dictionary<string, List<Item>>();
        public int TotalItems => Cache.Sum(kvp => kvp.Value.Count);

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
            //Cache.Clear();
            var types = Enum.GetValues(typeof(BuildComponent.ComponentType)).Cast<BuildComponent.ComponentType>().ToList();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var category = BuildComponent.CategoryFilterForType(type);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var items = await Search.LoadAll(null, Settings.StoreID(), category, Search.OrderByMode.pricelow);
                    if (Cache.ContainsKey(category))
                    {
                        Debug.WriteLine($"REFRESHING {category}");
                        var existing = Cache[category];
                        List<Item> toRemove = new List<Item>();
                        foreach (var item in items.Items)
                        {
                            var newItem = items.Items.FirstOrDefault(i => i.ID == item.ID);
                            if (newItem == null)
                            {
                                toRemove.Add(item);
                            }
                            else
                            {
                                item.Price = newItem.Price;
                                item.OriginalPrice = newItem.OriginalPrice;
                                item.SKU = newItem.SKU;
                                item.Name = newItem.Name;
                                item.PictureUrls = newItem.PictureUrls;
                                item.Brand = newItem.Brand;
                                item.URL = newItem.URL;
                                item.Stock = newItem.Stock;
                            }
                        }

                        toRemove.ForEach(i => existing.Remove(i));
                        Debug.WriteLine($"REMOVED {toRemove.Count} from {category}");
                    }
                    else
                    {
                        Cache[category] = items.Items;
                    }
                }
                var percent = (int)Math.Round(((float)(i + 1) / types.Count) * 100);
                progress.Report(percent);
            }

            await SaveCache();
        }

        private async Task SaveCache()
        {
            var json = System.Text.Json.JsonSerializer.Serialize(Cache, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(FILENAME, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
            Settings.LastUpdated(DateTimeOffset.Now);
        }

        public async Task DeepPopulateCache(IProgress<int> progress)
        {
            Stopwatch sw = new Stopwatch();
            TimeSpan totalElapsed = TimeSpan.Zero;
            int totalParsed = 0;
            int index = -1;
            var count = Cache.Sum(kvp => kvp.Value.Count);

            foreach(var kvp in Cache)
            {
                var category = kvp.Key;
                var items = kvp.Value;
                var cnt = 0;
                sw.Restart();
                foreach(var item in items)
                {
                    index++;

                    if(item.Specs == null || item.Specs.Count == 0)
                    {
                        var deepItem = await Item.FromUrl(item.URL, Settings.StoreID());
                        item.Specs = deepItem.Specs;
                        item.Location = deepItem.Location;
                        //await Task.Delay(714);
                        cnt++;
                        totalParsed++;
                        if (count > 0)
                        {
                            //var percent = (int)Math.Round(((float)(index + 1) / count) * 100);
                            progress?.Report(index);
                        }
                    }
                }
                sw.Stop();
                totalElapsed += sw.Elapsed;
                Debug.WriteLine($"{cnt} items needed deep populate.  Processed in {sw.Elapsed} seconds");
            }

            Debug.WriteLine("--------");
            Debug.WriteLine($"{Cache.Count} categories processed {totalParsed} in {totalElapsed}");
            await SaveCache();
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
