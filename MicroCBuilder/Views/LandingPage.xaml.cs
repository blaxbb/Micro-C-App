using MicroCBuilder.ViewModels;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MicroCBuilder.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : Page
    {
        public delegate void CreateBuildEventHandler(object sender, BuildInfo info);
        public event CreateBuildEventHandler OnCreateBuild;

        string[] files = new string[]
        {
                "TIER_5",
                "TIER_4",
                "TIER_3",
                "TIER_2",
                "TIER_1",
                "EMPTY",
        };

        public LandingPage()
        {
            this.InitializeComponent();
            if (DataContext is LandingPageViewModel vm)
            {
                vm.OnCreateBuild += CreateBuild;
                TimeSpan period = TimeSpan.FromSeconds(30);

                ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        vm.UpdateNetworkFlares?.Execute(null);
                    });


                }, period);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            foreach(var name in files)
            {
                string fname = $@"Assets\BuildTemplates\{name}.build";
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await InstallationFolder.GetFileAsync(fname);
                if (File.Exists(file.Path))
                {
                    var json = File.ReadAllText(file.Path);
                    var info = new ViewModels.BuildInfo()
                    {
                        Name = name,
                        Components = JsonConvert.DeserializeObject<List<BuildComponent>>(json)
                    };
                    if(DataContext is LandingPageViewModel vm)
                    {
                        vm.BuildTemplates.Add(info);
                    }
                }
            }
        }

        private void CreateBuild(object sender, BuildInfo info)
        {
            if (info != null)
            {
                OnCreateBuild?.Invoke(this, info);
            }
        }

        private void FlareDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if(e.OriginalSource is FrameworkElement ele && ele.DataContext is FlareInfo info)
            {
                CreateBuild(sender, new BuildInfo()
                {
                    Name = info.Flare.Title,
                    Components = info.Components
                });
            }
        }
    }


}
