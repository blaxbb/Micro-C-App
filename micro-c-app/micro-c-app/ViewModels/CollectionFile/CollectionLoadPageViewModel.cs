using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class CollectionLoadPageViewModel<T> : BaseViewModel
    {
        string filename;
        private List<string> items;

        public string Filename { get => filename; set => SetProperty(ref filename, value); }
        public ICommand Cancel { get; }
        public ICommand Load { get; }

        public List<string> Items { get => items; set => SetProperty(ref items, value); }

        public string FolderPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Folder);
        public string Path => System.IO.Path.Combine(FolderPath, Filename);
        private string Folder { get; }

        public List<T> Result { get; private set; }

        public CollectionLoadPageViewModel()
        {
            Cancel = new Command(async () =>
            {
                await Shell.Current.Navigation.PopModalAsync();
            });

            Load = new Command(async () =>
            {
                try
                {
                    if (File.Exists(Path))
                    {
                        var text = File.ReadAllText(Path);
                        Result = JsonSerializer.Deserialize<List<T>>(text);
                        MessagingCenter.Send<CollectionLoadPageViewModel<T>>(this, "load");
                        await Shell.Current.Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.DisplayAlert("Error", "File not found", "Ok");
                        });
                    }
                }
                catch(Exception e)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Exception", e.ToString(), "Ok");
                    });
                }
            });
        }

        public CollectionLoadPageViewModel(string folder) : this()
        {
            Folder = folder;
            if (!System.IO.Directory.Exists(FolderPath))
            {
                return;
            }

            var files = System.IO.Directory.EnumerateFiles(FolderPath);
            Items = files.Select(f => System.IO.Path.GetFileName(f)).ToList();
        }
    }
}
