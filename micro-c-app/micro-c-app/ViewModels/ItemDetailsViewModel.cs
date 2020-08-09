using micro_c_app.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class ItemDetailsViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public string ActivePicture => Item?.PictureUrls?[PictureIndex];
        int PictureIndex = 0;
        public ICommand BackPicture { get; }
        public ICommand ForwardPicture { get; }

        public ItemDetailsViewModel()
        {
            Title = "Details";
            //Item = new Item()
            //{
            //    Name = "Item",
            //    OriginalPrice = 1000.00f,
            //    Price = 900.50f,
            //    Location = "Location 000",
            //    SKU = "123456",
            //    Stock = "6 in stock",
            //    Plans = new List<Plan>() {
            //        new Plan()
            //        {
            //            Name = "PLAN 1",
            //            Price = 100f
            //        },
            //    }
            //};

            BackPicture = new Command(() =>
            {
                if(PictureIndex > 0)
                {
                    PictureIndex--;
                    OnPropertyChanged(nameof(ActivePicture));
                }
                else if(Item != null && Item?.PictureUrls?.Count > 1){
                    PictureIndex = Item.PictureUrls.Count - 1;
                    OnPropertyChanged(nameof(ActivePicture));
                }
            });

            ForwardPicture = new Command(() =>
            {
                if(PictureIndex < Item?.PictureUrls?.Count - 1)
                {
                    PictureIndex++;
                    OnPropertyChanged(nameof(ActivePicture));
                }
                else if(Item != null && Item?.PictureUrls?.Count > 1)
                {
                    PictureIndex = 0;
                    OnPropertyChanged(nameof(ActivePicture));
                }
                
            });
        }
    }
}