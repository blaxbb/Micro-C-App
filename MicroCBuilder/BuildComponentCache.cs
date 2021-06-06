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
                    foreach(var kvp in Cache)
                    {
                        foreach(var i in kvp.Value)
                        {
                            i.ComponentType = BuildComponent.TypeForCategoryFilter(kvp.Key);
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        public async Task PopulateCache(IProgress<int> progress)
        {
            //Cache.Clear();
            var types = Settings.Categories();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var category = BuildComponent.CategoryFilterForType(type);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var items = await Search.LoadEnhanced(null, Settings.StoreID(), category);
                    if (Cache.ContainsKey(category))
                    {
                        Debug.WriteLine($"REFRESHING {category}");
                        var existing = Cache[category];
                        List<Item> toAdd = new List<Item>();
                        foreach (var item in items.Items)
                        {
                            var existingItem = existing.FirstOrDefault(i => i.ID == item.ID);
                            if (existingItem == null)
                            {
                                toAdd.Add(item);
                            }
                            else
                            {
                                existingItem.Price = item.Price;
                                existingItem.OriginalPrice = item.OriginalPrice;
                                existingItem.SKU = item.SKU;
                                existingItem.Name = item.Name;
                                existingItem.PictureUrls = item.PictureUrls;
                                existingItem.Brand = item.Brand;
                                existingItem.URL = item.URL;
                                existingItem.Stock = item.Stock;
                            }
                        }

                        var toRemove = new List<Item>();
                        foreach(var existingItem in existing)
                        {
                            if(!items.Items.Any(i => i.ID == existingItem.ID))
                            {
                                toRemove.Add(existingItem);
                            }
                        }

                        toRemove.ForEach(i => existing.Remove(i));
                        toAdd.ForEach(i => existing.Add(i));
                        if (toAdd.Count > 0)
                        {
                            Debug.WriteLine($"Added {toAdd.Count} from {category}");
                        }
                        if(toRemove.Count > 0)
                        {
                            Debug.WriteLine($"Removed {toRemove.Count} from {category}");
                        }
                    }
                    else
                    {
                        Cache[category] = items.Items;
                        Debug.WriteLine($"Created {category} with {items.Items.Count}");
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
            var categoryStrings = Settings.Categories().Select(c => BuildComponent.CategoryFilterForType(c)).ToList();
            var count = Cache.Where(kvp => categoryStrings.Contains(kvp.Key)).Sum(kvp => kvp.Value.Count);
            foreach(var kvp in Cache.Where(kvp => categoryStrings.Contains(kvp.Key)))
            {
                var category = kvp.Key;
                var items = kvp.Value;
                var cnt = 0;
                sw.Restart();
                foreach(var item in items)
                {
                    index++;

                    if(item.Specs == null || item.Specs.Count <= 1)
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

                        await SaveCache();
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

        public BuildComponent.ComponentType FindType(string sku)
        {
            foreach(var kvp in Cache)
            {
                var categoryFilter = kvp.Key;
                var items = kvp.Value;
                if(items.Any(i => i.SKU == sku))
                {
                    return BuildComponent.TypeForCategoryFilter(categoryFilter);
                }
            }

            return BuildComponent.ComponentType.None;
        }

        public Item? FindItem(string sku)
        {
            foreach (var kvp in Cache)
            {
                var categoryFilter = kvp.Key;
                var items = kvp.Value;
                var item = items.FirstOrDefault(i => i.SKU == sku);
                if(item != null)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
