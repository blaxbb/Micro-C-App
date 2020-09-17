using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MicroCBuilder.Views
{
    public sealed partial class SearchResults : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Item> Results { get; }
        public int Count => Results?.Count ?? 0;
        public string Query
        {
            get { return (string)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); HandleQuery(value); }
        }

        // Using a DependencyProperty as the backing store for Query.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register("Query", typeof(string), typeof(SearchResults), new PropertyMetadata("", new PropertyChangedCallback(QueryChanged)));



        public List<Item> Items => BuildComponentCache.Current.FromType(ComponentType);
        public BuildComponent.ComponentType ComponentType
        {
            get { return (BuildComponent.ComponentType)GetValue(ComponentTypeProperty); }
            set { SetValue(ComponentTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComponentType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComponentTypeProperty =
            DependencyProperty.Register("ComponentType", typeof(BuildComponent.ComponentType), typeof(SearchResults), new PropertyMetadata(BuildComponent.ComponentType.CaseFan));



        public ICommand ItemSelected
        {
            get { return (ICommand)GetValue(ItemSelectedProperty); }
            set { SetValue(ItemSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSelectedProperty =
            DependencyProperty.Register("ItemSelected", typeof(ICommand), typeof(SearchResults), new PropertyMetadata(null));

        public delegate void ItemSelectedEventArgs(object sender, Item item);
        public event ItemSelectedEventArgs OnItemSelected;




        public SearchResults()
        {
            this.InitializeComponent();
            DataContext = this;
            Results = new ObservableCollection<Item>();
            var collection = new CollectionViewSource();
            collection.Source = Results;
            dataGrid.ItemsSource = collection.View;
            dataGrid.CanUserSortColumns = true;
        }

        private static void QueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchResults s)
            {
                s.HandleQuery(s.Query);
            }
        }

        private void HandleQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Results.Clear();
                return;
            }

            var matches = FuzzySharp.Process.ExtractTop(query, Items.Select(i => $"{i.Brand} {i.SKU} {i.Name}"), scorer: ScorerCache.Get<TokenDifferenceScorer>(), limit: 100).ToList();

            Results.Clear();
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var item = Items[match.Index];
                Results.Add(item);
            }
            OnPropertyChanged(nameof(Count));
            //list.ItemsSource = Results;
            //dataGrid.ItemsSource = Results;
        }

        private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private Item Clone(Item item)
        {
            var json = JsonSerializer.Serialize(item);
            var ret = JsonSerializer.Deserialize<Item>(json);
            ret.Quantity = 1;
            return ret;
        }

        private void dataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var i = dataGrid.SelectedIndex;
            if(i == -1)
            {
                return;
            }
            var item = Clone(Results?[i]);
            System.Diagnostics.Debug.WriteLine(Results?[i]?.Name);
            ItemSelected?.Execute(item);
            OnItemSelected?.Invoke(this, item);
        }

        private void dataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                dataGrid_DoubleTapped(sender, new DoubleTappedRoutedEventArgs());
            }
        }
    }
}
