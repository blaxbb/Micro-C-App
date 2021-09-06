using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class ExportSignsControl : UserControl
    {
        public ExportSignsControl()
        {
            this.InitializeComponent();
            UsernameTextBox.Text = Settings.SignUsername();
            PasswordTextBox.Password = Settings.SignPassword();
            SavePasswordCheckBox.IsChecked = !string.IsNullOrWhiteSpace(PasswordTextBox.Password);
            BaseUrlTextBox.Text = Settings.SignBaseUrl();
        }

        public string Title => TitleTextBox.Text;
        public string SignType => SignTypeComboxBox.SelectedItem.ToString();
        public string Username => UsernameTextBox.Text;
        public string Password => PasswordTextBox.Password;
        public string BaseUrl => BaseUrlTextBox.Text;
        public bool SavePassword => SavePasswordCheckBox.IsChecked ?? false;
    }
}
