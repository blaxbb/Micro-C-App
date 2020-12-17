using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProgressView : ContentView
    {
        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(double), typeof(ProgressView), 0d);
        public double Progress { get { return (double)GetValue(ProgressProperty); } set { SetValue(ProgressProperty, value); } }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ProgressView), "", BindingMode.TwoWay);
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); OnPropertyChanged(nameof(Text)); } }

        public ProgressView()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}