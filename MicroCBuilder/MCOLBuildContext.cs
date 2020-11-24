using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MicroCBuilder
{
    public class MCOLBuildContext : INotifyPropertyChanged
    {
        public string BuildURL { get => buildURL; set => SetProperty(ref buildURL, value); }
        public string TinyBuildURL { get => tinyBuildURL; set => SetProperty(ref tinyBuildURL, value); }

        private HttpClient client;
        private Dictionary<string, List<string>> CategorySelectors = new Dictionary<string, List<string>>();
        private Dictionary<BuildComponent, string> ComponentIdMap = new Dictionary<BuildComponent, string>();

        private string buildURL;
        private string tinyBuildURL;

        public MCOLBuildContext()
        {
            client = new HttpClient();
        }

        ~MCOLBuildContext()
        {
            client?.Dispose();
        }

        public async Task AddComponent(BuildComponent component)
        {
            if (component.Item == null || component.Item.ID == null)
            {
                return;
            }

            var selectorID = BuildComponent.MCOLSelectorIDForType(component.Type);
            //bool duplicateSelector = hitCategories.Contains(selectorID);
            bool duplicateSelector = CategorySelectors.ContainsKey(selectorID);
            var url = $"https://www.microcenter.com/site/content/custom-pc-builder.aspx?toselectorId={selectorID}&configuratorId=1&productId={component.Item.ID}&productName={component.Item.Name}&newItem={(duplicateSelector ? "true" : "false")}&qty=5";

            var result = await client.GetAsync(url);
            var body = await result.Content.ReadAsStringAsync();

            //Get url
            var match = Regex.Match(body, "value=\"(.*?)\" name=\"shareURL\" id=\"shareURL\">");
            if (match.Success)
            {
                if (BuildURL != match.Groups[1].Value)
                {
                    BuildURL = match.Groups[1].Value;
                    TinyBuildURL = await GetTinyUrl(BuildURL);
                }
                Debug.WriteLine(BuildURL);
            }

            //get section with component id
            match = Regex.Match(body, $"id=\"selector_{selectorID}\"(?:.*?)(?:div(?:.*?)selectorWrapper|scroll-container)", RegexOptions.Singleline);
            if (match.Success)
            {
                var componentText = match.Groups[0].Value;
                //collect all of the items in that section
                var matches = Regex.Matches(componentText, "id=\"selector_([^\"]{4,})\"");
                List<string> selectors = new List<string>();
                if (matches.Count > 0)
                {
                    selectors = matches.OfType<Match>().Select(m => m.Groups[1].Value).ToList();
                }
                selectors.Add("");

                string newID = "";
                if (CategorySelectors.ContainsKey(selectorID))
                {
                    newID = selectors.Except(CategorySelectors[selectorID]).FirstOrDefault();
                }

                ComponentIdMap[component] = newID;

                if (component.Item.Quantity > 1)
                {
                    await SetQuantity(selectorID, newID, component.Item.Quantity);
                }
                Debug.WriteLine(newID);
                Debug.WriteLine(string.Join(",", selectors.ToArray()));
                CategorySelectors[selectorID] = selectors;
            }
        }

        public async Task RemoveComponent(BuildComponent component)
        {
            var selectorID = BuildComponent.MCOLSelectorIDForType(component.Type);
            if (!CategorySelectors.ContainsKey(selectorID))
            {
                return;
            }

            if (ComponentIdMap.ContainsKey(component))
            {
                var id = ComponentIdMap[component];
                await Remove(selectorID, id);
                ComponentIdMap.Remove(component);
            }
        }

        private async Task Remove(string category, string selector)
        {
            var url = $"https://www.microcenter.com/site/content/custom-pc-builder.aspx?toselectorId={category}&qty=0&selectorQtyId={category}&selectorQtyitemKey={selector}";
            var result = await client.GetAsync(url);

        }

        private async Task SetQuantity(string category, string id, int quantity)
        {
            var url = $"https://www.microcenter.com/site/content/custom-pc-builder.aspx?toselectorId={category}&qty={quantity}&selectorQtyId={category}&selectorQtyitemKey={id}";
            var result = await client.GetAsync(url);
        }

        private async Task<string> GetTinyUrl(string url)
        {
            var result = await client.GetAsync($"https://tinyurl.com/api-create.php?url={url}");
            return await result.Content.ReadAsStringAsync();
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
