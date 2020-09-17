using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class BuildSummaryControl : UserControl, INotifyPropertyChanged
    {
        public float SubTotal
        {
            get { return (float)GetValue(SubTotalProperty); }
            set { SetValue(SubTotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTotal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTotalProperty =
            DependencyProperty.Register("SubTotal", typeof(float), typeof(BuildSummaryControl), new PropertyMetadata(0f, new PropertyChangedCallback(PropChanged)));

        const float TAX = .075f;
        public float TaxAmt => SubTotal * TAX;
        public float Total => SubTotal * (1 + TAX);
        public float CCDiscount => Total * .05f;
        public float CCTotal => Total * .95f;


        private static void PropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BuildSummaryControl control)
            {
                control.UpdateProperties();
            }
        }

        public BuildSummaryControl()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(TaxAmt));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(CCDiscount));
            OnPropertyChanged(nameof(CCTotal));
            OnPropertyChanged(nameof(SubTotal));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
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
