using micro_c_app.Models;
using micro_c_app.Models.Reference;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

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
                SetProperty(ref selectedItem, value);
            }
        
        }



        protected void SortPlans(PlanReference plan)
        {
            switch (plan.Type)
            {
                case PlanReference.PlanType.Apple_Plans_ADH_13:  // ADH Apple
                    Items.Add(new PriceReference($"/Plans/Apple/13 inch MBP MBA/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH 13", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_ADH_15_and_16:
                    Items.Add(new PriceReference($"/Plans/Apple/15 and 16 inch MBP/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH 15 and 16", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_ADH_iMac:
                    Items.Add(new PriceReference($"/Plans/Apple/iMac/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH iMac", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_ADH_iPad:
                    Items.Add(new PriceReference($"/Plans/Apple/iPad/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH iPad", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_ADH_Mac_Mini:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Mini/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH Mac Mini", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_ADH_Mac_Pro:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Pro/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH Mac Pro", "ADH")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Laptop_ADH:  // ADH Laptops and desktops
                    Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Desktop_ADH:
                    Items.Add(new PriceReference($"/Plans/Desktop/{plan.Type.ToString().Replace('_', ' ').Replace("Desktop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Laptop_DOP:  // Laptops and desktops DOP
                    Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Desktop_DOP:
                    Items.Add(new PriceReference($"/Plans/Desktop/{plan.Type.ToString().Replace('_', ' ').Replace("Desktop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Laptop_Extension:  // Laptops and desktops extension
                    Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Desktop_Extension:
                    Items.Add(new PriceReference($"/Plans/Desktop/{plan.Type.ToString().Replace('_', ' ').Replace("Desktop ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Apple_Plans_13:
                    Items.Add(new PriceReference($"/Plans/Apple/13 inch MBP MBA/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans 13", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_15_and_16:
                    Items.Add(new PriceReference($"/Plans/Apple/15 and 16 inch MBP/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans 15 and 16", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_iMac:
                    Items.Add(new PriceReference($"/Plans/Apple/iMac/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans iMac", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_iPad:
                    Items.Add(new PriceReference($"/Plans/Apple/iPad/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans iPad", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_Mac_Mini:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Mini/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans Mac Mini", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Apple_Plans_Mac_Pro:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Pro/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans Mac Pro", "DOP")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Tablet_ADH:
                    Items.Add(new PriceReference($"/Plans/Tablets/{plan.Type.ToString().Replace('_', ' ').Replace("Tablet ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Tablet_DOP:
                    Items.Add(new PriceReference($"/Plans/Tablets/{plan.Type.ToString().Replace('_', ' ').Replace("Tablet ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.Tablet_Extension:
                    Items.Add(new PriceReference($"/Plans/Tablets/{plan.Type.ToString().Replace('_', ' ').Replace("Tablet ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.Carry_In:
                    Items.Add(new PriceReference($"/Plans/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                case PlanReference.PlanType.AppleCare_13_MBA:
                    Items.Add(new PriceReference($"/Plans/Apple/13 inch MBP MBA/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare 13 MBA", "AppleCare MacBook Air")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_13_MBP:
                    Items.Add(new PriceReference($"/Plans/Apple/13 inch MBP MBA/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare 13 MBA", "AppleCare MacBook Pro")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_15:
                    Items.Add(new PriceReference($"/Plans/Apple/15 and 16 inch MBP/{plan.Type.ToString().Replace('_', ' ').Replace(" 15", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Mac_Mini:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Mini/{plan.Type.ToString().Replace('_', ' ').Replace(" Mac Mini", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_iMac:
                    Items.Add(new PriceReference($"/Plans/Apple/iMac/{plan.Type.ToString().Replace('_', ' ').Replace(" iMac", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Mac_Pro:
                    Items.Add(new PriceReference($"/Plans/Apple/Mac Pro/{plan.Type.ToString().Replace('_', ' ').Replace(" Mac Pro", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Apple_TV:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Display:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_iPod_Touch:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_iPad:
                    Items.Add(new PriceReference($"/Plans/Apple/iPad/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare iPad", "AppleCare iPad, iPad Air and iPad Mini")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;  
                case PlanReference.PlanType.AppleCare_iPad_Pro:
                    Items.Add(new PriceReference($"/Plans/Apple/iPad/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_iPhone:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Watch_S3:
                    Items.Add(new PriceReference($"/Plans/Apple/Apple Watch/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare Watch S3", "Series 3")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Watch_S4_S5:
                    Items.Add(new PriceReference($"/Plans/Apple/Apple Watch/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare Watch S4 S5", "Series 4 and 5")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Watch_Stainless:
                    Items.Add(new PriceReference($"/Plans/Apple/Apple Watch/{plan.Type.ToString().Replace('_', ' ').Replace("AppleCare Watch Stainless", "Series 4 and 5 Stainless Steel")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_HomePod:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
                case PlanReference.PlanType.AppleCare_Headphones:
                    Items.Add(new PriceReference($"/Plans/Apple/AppleCare Misc/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;

                default:
                    Items.Add(new PriceReference($"/Plans/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
                    break;
            }
        }
            
            /* Old way of doing ^that
            if (plan.Type.ToString().StartsWith("Apple_Plans_ADH"))   
            {
                //Items.Add(new PriceReference($"/Plans/Apple/ADH/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ADH ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
            else if (plan.Type.ToString().StartsWith("Apple_Plans"))
            {
                Items.Add(new PriceReference($"/Plans/Apple/Non-ADH/{plan.Type.ToString().Replace('_', ' ').Replace("Apple Plans ", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
            else if (plan.Type.ToString().StartsWith("Laptop_ADH"))
            {
                Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
            else if (plan.Type.ToString().StartsWith("Laptop_DOP"))
            {
                Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
            else if (plan.Type.ToString().StartsWith("Laptop_Extension"))
            {
                Items.Add(new PriceReference($"/Plans/Laptops/{plan.Type.ToString().Replace('_', ' ').Replace("Laptop", "")}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
            else
            {
                Items.Add(new PriceReference($"/Plans/{plan.Type.ToString().Replace('_', ' ')}/${plan.MinPrice:N2}-${plan.MaxPrice:N2}", plan.Tiers.Select(t => ($"{t.Duration} years", t.Price)).ToArray()));
            }
        } */

        public ICommand ItemTapped { get; }

        public ReferencePageViewModel()
        {
            ItemTapped = new Command(() =>
            {
                if(SelectedItem == "Back")
                {
                    var pathComps = Path.Split('/');
                    var path = $"{string.Join("/", pathComps.Take(pathComps.Length - 2))}/";
                    SetPath(path);
                    return;
                }
                if (EndLevel)
                {
                    return;
                }
                SetPath(Path + SelectedItem + "/");
            });
            Title = "Search";
            Items = new List<PriceReference>();
            ListItems = new ObservableCollection<string>();

            foreach (var plan in PlanReference.AllPlans)
            {
                SortPlans(plan);
            }
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

            if(Path != "/")
            {
                ListItems.Add("Back");
            }

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

                var pathComps = first.Path.Split('/');
                Title = pathComps.ElementAtOrDefault(pathComps.Length - 2);

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