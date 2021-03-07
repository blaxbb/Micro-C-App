using DataFlareClient;
using micro_c_app.Views;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{

    public class ImportPageViewModel<T> : BaseViewModel
    {
        private string folder;
        private ObservableCollection<string> localFiles;
        private bool localFilesMode;
        private ObservableCollection<Flare> networkFlares;
        private bool loading;
        private string selectedFile;
        private Flare selectedFlare;

        public ICommand ListLocalFiles { get; }
        public ICommand ListNetworkFlares { get; }
        public ICommand SetLocalMode { get; }
        public ICommand SetNetworkMode { get; }
        public ICommand LoadCommand { get; }
        public ObservableCollection<string> LocalFiles { get => localFiles; set => SetProperty(ref localFiles, value); }
        public ObservableCollection<Flare> NetworkFlares { get => networkFlares; set => SetProperty(ref networkFlares, value); }

        public string FolderPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Folder);
        public string Folder { get => folder; set => SetProperty(ref folder, value); }
        public string Path(string filename) => System.IO.Path.Combine(FolderPath, filename);
        public bool LocalFilesMode { get => localFilesMode; set => SetProperty(ref localFilesMode, value); }
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        public string SelectedFile { get => selectedFile; set => SetProperty(ref selectedFile, value); }
        public Flare SelectedFlare { get => selectedFlare; set => SetProperty(ref selectedFlare, value); }

        public List<T> Result { get; private set; }

        public ImportPageViewModel()
        {
            LocalFiles = new ObservableCollection<string>();
            LocalFilesMode = true;
            ListLocalFiles = new Command(() =>
            {
                Loading = true;
                LocalFiles?.Clear();
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    return;
                }

                var files = System.IO.Directory.EnumerateFiles(FolderPath);
                LocalFiles = new ObservableCollection<string>(files.Select(f => System.IO.Path.GetFileName(f)).ToList());
                LocalFilesMode = true;
                Loading = false;
            });

            ListNetworkFlares = new Command(async () =>
            {
                Loading = true;
                var flares = await Flare.GetTag("https://dataflare.bbarrett.me/api/Flare", $"micro-c-{SettingsPage.StoreID()}");
                NetworkFlares = new ObservableCollection<Flare>(flares);
                LocalFilesMode = false;
                Loading = false;
            });

            SetLocalMode = new Command(() =>
            {
                ListLocalFiles.Execute(null);

            });

            SetNetworkMode = new Command(() =>
            {
                ListNetworkFlares.Execute(null);
            });

            LoadCommand = new Command(async () =>
            {
                if (LocalFilesMode && !string.IsNullOrWhiteSpace(SelectedFile))
                {
                    var path = Path(SelectedFile);
                    if (File.Exists(path))
                    {
                        var text = File.ReadAllText(path);
                        Result = JsonConvert.DeserializeObject<List<T>>(text);
                        await Shell.Current.Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.DisplayAlert("Error", "File not found", "Ok");
                        });
                        await Shell.Current.Navigation.PopModalAsync();
                    }
                }
                else if(SelectedFlare != null)
                {
                    var components = JsonConvert.DeserializeObject<List<BuildComponent>>(SelectedFlare.Data);
                    if (typeof(T) == typeof(Item))
                    {
                        Result = components.Select(c => c.Item).Cast<T>().ToList();
                    }
                    else if(typeof(T) == typeof(BuildComponent))
                    {
                        Result = components.Cast<T>().ToList();
                    }
                    await Shell.Current.Navigation.PopModalAsync();
                }
            });
        }
    }
}
