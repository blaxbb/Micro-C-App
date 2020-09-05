using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.ViewModels
{
    public class ProgressPageViewModel : BaseViewModel
    {
        private string description;
        private int totalItems;
        private int currentItems;

        public string Description { get => description; set => SetProperty(ref description, value); }
        public double Progress => (double)CurrentItem / TotalItems;

        public int TotalItems { get => totalItems; set { SetProperty(ref totalItems, value); OnPropertyChanged(nameof(Progress)); } }
        public int CurrentItem { get => currentItems; set { SetProperty(ref currentItems, value); OnPropertyChanged(nameof(Progress)); } }
    }
}
