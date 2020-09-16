using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using MicroCBuilder.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
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
    public sealed partial class BuildComponentControl : UserControl, INotifyPropertyChanged
    {
        List<Item> Suggestions = new List<Item>();
        public BuildComponent Component
        {
            get { return (BuildComponent)GetValue(ComponentProperty); }
            set { SetValue(ComponentProperty, value); }
        }

        public static readonly DependencyProperty ComponentProperty =
            DependencyProperty.Register("Component", typeof(BuildComponent), typeof(BuildComponentControl), new PropertyMetadata(new BuildComponent()));

        public List<Item> Items => BuildComponentCache.Current.FromType(Component.Type);

        public ICommand QueryChanged
        {
            get { return (ICommand)GetValue(QueryChangedProperty); }
            set { SetValue(QueryChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Query.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryChangedProperty =
            DependencyProperty.Register("QueryChanged", typeof(ICommand), typeof(BuildComponentControl), new PropertyMetadata(null));

        public delegate void QueryUpdatedEventHandler(BuildComponentControl sender, string query);
        public event QueryUpdatedEventHandler QueryUpdated;

        public BuildComponentControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            var children = VisualTreeHelper.GetChildrenCount(textBox);
        }

        private void textBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(QueryChanged != null)
            {
                QueryChanged.Execute(sender.Text);
            }
            QueryUpdated?.Invoke(this, sender.Text);
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var matches = FuzzySharp.Process.ExtractTop(sender.Text, Items.Select(i => $"{i.Brand} {i.SKU} {i.Name}"), scorer: ScorerCache.Get<TokenDifferenceScorer>(), limit: 10).ToList();
                Suggestions = new List<Item>();

                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var item = Items[match.Index];
                    Suggestions.Add(item);
                }

                textBox.ItemsSource = Suggestions;
            }
        }

        private void textBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is Item item)
            {
                Component.Item = Clone(item);
            }
            else
            {
                if (Suggestions != null && Suggestions.Count > 0)
                {
                    Component.Item = Clone(Suggestions.First());
                }
            }
            textBox.Text = "";
            Suggestions.Clear();
        }

        private Item Clone(Item item)
        {
            var json = JsonSerializer.Serialize(item);
            var ret = JsonSerializer.Deserialize<Item>(json);
            ret.Quantity = 1;
            return ret;
        }

        private void textBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //if(args.SelectedItem is Item item)
            //{
            //    textBox.Text = item.Name;
            //}
        }

        public void SetTextBox(string text)
        {
            textBox.Text = text;
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
    }
}
