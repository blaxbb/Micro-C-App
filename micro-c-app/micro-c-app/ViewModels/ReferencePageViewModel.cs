using micro_c_app.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace micro_c_app.ViewModels
{
    public class ReferencePageViewModel : BaseViewModel
    {
        private string selectedItem;
        private bool endLevel;

        public string Path { get; set; }
        public List<PriceReference> Items { get; }

        public bool EndLevel { get => endLevel; set { SetProperty(ref endLevel, value); OnPropertyChanged(nameof(NotEndLevel)); } }
        public bool NotEndLevel => !EndLevel;

        public ObservableCollection<string> ListItems { get; }
        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                if(EndLevel)
                {
                    return;
                }

                SetProperty(ref selectedItem, value);
                if (value != null)
                {
                    SetPath(Path + SelectedItem + "/");
                }
            }
        }

        public ReferencePageViewModel()
        {
            Title = "Search";
            Items = new List<PriceReference>();
            ListItems = new ObservableCollection<string>();

            Items.Add(new PriceReference("/Plans/Replacement/$0.00-$4.99    ",     ("2 year", 0.75f),  ("3 year", 1.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$4.99-$9.99    ",     ("2 year", 0.99f),  ("3 year", 2.49f)));
            Items.Add(new PriceReference("/Plans/Replacement/$10.00-$14.99  ",   ("2 year", 1.49f),  ("3 year", 2.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$15.00-$19.99  ",   ("2 year", 1.99f),  ("3 year", 3.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$20.00-$24.99  ",   ("2 year", 2.49f),  ("3 year", 4.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$25.00-$49.99  ",   ("2 year", 4.99f),  ("3 year", 9.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$50.00-$74.99  ",   ("2 year", 6.99f),  ("3 year", 14.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$75.00-$99.99  ",   ("2 year", 9.99f),  ("3 year", 19.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$100.00-$199.99", ("2 year", 19.99f), ("3 year", 39.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$200.00-$299.99", ("2 year", 29.99f), ("3 year", 59.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$300.00-$399.99", ("2 year", 49.99f), ("3 year", 89.99f)));
            Items.Add(new PriceReference("/Plans/Replacement/$400.00-$500.00", ("2 year", 69.99f), ("3 year", 139.99f)));

            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$0.00-$49.99",     ("2 year", 14.99f),  ("1 year", 5.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$50.00-$99.99",    ("2 year", 39.99f),  ("1 year", 19.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$100.00-$199.99",  ("2 year", 69.99f),  ("1 year", 29.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$200.00-$199.99",  ("2 year", 99.99f),  ("1 year", 49.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$300.00-$199.99",  ("2 year", 139.99f), ("1 year", 59.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$400.00-$199.99",  ("2 year", 179.99f), ("1 year", 79.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$500.00-$199.99",  ("2 year", 199.99f), ("1 year", 99.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$750.00-$199.99",  ("2 year", 299.99f), ("1 year", 199.99f)));
            Items.Add(new PriceReference("/Plans/Small Electronic ADH/$1000.00-$199.99", ("2 year", 399.99f), ("1 year", 299.99f)));

            Items.Add(new PriceReference("/Plans/BYO Replacement/$0.00-49.99",      ("3 year", 9.99f),   ("2 year", 6.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$50.00-99.99",     ("3 year", 29.99f),  ("2 year", 14.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$100.00-199.99",   ("3 year", 49.99f),  ("2 year", 29.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$200.00-299.99",   ("3 year", 69.99f),  ("2 year", 49.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$300.00-399.99",   ("3 year", 99.99f),  ("2 year", 69.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$400.00-499.99",   ("3 year", 129.99f), ("2 year", 89.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$500.00-999.99",   ("3 year", 199.99f), ("2 year", 139.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$1000.00-1499.99", ("3 year", 279.99f), ("2 year", 199.99f)));
            Items.Add(new PriceReference("/Plans/BYO Replacement/$1500.00-3000.00", ("3 year", 429.99f), ("2 year", 299.99f)));

            SetPath("/");
        }

        private void SetPath(string path)
        {
            //
            //this is awful and I apologize for it in advanced
            //
            Path = path;

            var currentItems = Items.Where(i => i.Path.StartsWith(Path));
            var paths = currentItems.GroupBy(i => i.Path.Substring(Path.Length).Split('/').FirstOrDefault()).ToList();
            ListItems.Clear();

            if (currentItems.Any(c => c.Path.Substring(Path.Length).Contains('/')))
            {
                EndLevel = false;
                foreach (var p in paths)
                {
                    ListItems.Add(p.Key);
                }
            }
            else if(currentItems.Count() > 0)
            {
                EndLevel = true;
                StringBuilder b = new StringBuilder();
                var first = currentItems.FirstOrDefault();
                var cats = first.Items.Select(f => f.name);
                var maxCatLength = cats.Max(c => c.Length);
                var maxPathLength = paths.Max(p => p.Key.Length);
                ListItems.Add($"{"".PadRight(maxPathLength + 4)}{string.Join("     ", cats)}");
                foreach(var p in paths)
                {
                    var item = p.FirstOrDefault();
                    ListItems.Add($"{p.Key.PadRight(maxPathLength)}   {string.Join("    ", item.Items.Select(i => i.price.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("en-us")).PadLeft(7)))}");
                }
            }
        }


        public bool BackButton()
        {
            //called from appshell.xaml.cs
            if (Path.Length > 1)
            {
                var pathComponents = Path.Split('/');
                var path = string.Join("/", pathComponents.Take(pathComponents.Length - 2));
                SetPath(path + "/");
                return true;
            }

            return false;
        }
    }
}