﻿using micro_c_app.Views;
using micro_c_lib.Models.Inventory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class BaseInventoryViewModel : BaseViewModel
    {

        public const string BASE_URL = "https://location.bbarrett.me/api/Audit";
        private ComponentType type;
        private bool isLoading;
        private bool hasSelectedCategory;

        public ComponentType Type { get => type; set => SetProperty(ref type, value); }
        public bool IsLoading { get => isLoading; set => SetProperty(ref isLoading, value); }

        public bool HasSelectedCategory { get => hasSelectedCategory; set => SetProperty(ref hasSelectedCategory, value); }
        public ICommand SetFavorite { get; }

        public ICommand CategoryCommand { get; }
        public ICommand ResetCategoryCommand { get; }
        public List<ComponentTypeFavoriteInfo> ComponentTypes { get; set; }

        public BaseInventoryViewModel()
        {
            ResetCategoryCommand = new Command(() =>
            {
                HasSelectedCategory = false;
            });

            CategoryCommand = new Command<ComponentType>(async (type) =>
            {
                Type = type;
                IsLoading = true;
                await Load();
                HasSelectedCategory = true;
                IsLoading = false;
            });

            SetFavorite = new Command<ComponentType>((t) =>
            {
                var favorites = SettingsPage.InventoryFavorites();
                if (favorites.Contains(t))
                {
                    favorites.RemoveAll(f => f == t);
                }
                else
                {
                    favorites.Add(t);
                }
                SettingsPage.InventoryFavorites(favorites);
                InitFavorites();
            });

            InitFavorites();
        }

        protected virtual async Task Load()
        {
            return;
        }

        void InitFavorites()
        {
            var favorites = SettingsPage.InventoryFavorites();
            ComponentTypes = new List<ComponentTypeFavoriteInfo>();
            favorites.ForEach(c => ComponentTypes.Add(new ComponentTypeFavoriteInfo(c, true)));

            Enum.GetValues(typeof(ComponentType))
                .Cast<ComponentType>()
                .Where(c => !favorites.Contains(c))
                .ToList()
                .ForEach(c => ComponentTypes.Add(new ComponentTypeFavoriteInfo(c, false)));

            OnPropertyChanged(nameof(ComponentTypes));
        }

        protected async Task<Dictionary<string, List<InventoryEntry>>?> GetEntries(ComponentType type)
        {
            var store = SettingsPage.StoreID();

            using var client = new HttpClient();

            var entriesResult = await client.GetStringAsync($"{BASE_URL}/Entries/{store}/{(int)type}");
            if (string.IsNullOrWhiteSpace(entriesResult))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<Dictionary<string, List<InventoryEntry>>>(entriesResult);
        }

        public static async Task<T> Get<T>(ComponentType type, string method)
        {
            var store = SettingsPage.StoreID();

            using var client = new HttpClient();

            var result = await client.GetStringAsync($"{BASE_URL}/{method}/{store}/{(int)type}");
            return JsonConvert.DeserializeObject<T>(result);
        }
    }

    public class ComponentTypeFavoriteInfo : BaseViewModel
    {
        private ComponentType type;
        private bool favorite;

        public ComponentType ComponentType { get => type; set => SetProperty(ref type, value); }
        public bool Favorite { get => favorite; set => SetProperty(ref favorite, value); }

        public ComponentTypeFavoriteInfo(ComponentType type, bool favorite)
        {
            ComponentType = type;
            Favorite = favorite;
        }
    }
}
