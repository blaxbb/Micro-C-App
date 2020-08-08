using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace micro_c_app.Models
{
    public class BuildComponent : INotifyPropertyChanged
    {
        private string sku;
        private string productID;

        public enum ComponentType
        {
            CPU,
            Motherboard,
            RAM
        }
        public ComponentType Type { get; set; }

        public string SKU { get => sku; set { SetProperty(ref sku, value); OnPropertyChanged(nameof(ComponentLabel)); } }
        public string ProductID { get => productID; set => SetProperty(ref productID, value); }

        public string ComponentLabel => $"{Type.ToString()} - {SKU}";

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
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
