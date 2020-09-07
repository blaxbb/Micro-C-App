using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class CollectionSavePageViewModel : BaseViewModel
    {
        private string filename;
        private string? errorText;

        public string Filename { get => filename; set => SetProperty(ref filename, value); }
        public string? ErrorText { get => errorText; set => SetProperty(ref errorText, value); }

        private IEnumerable<object> Items { get; set; }

        public ICommand Cancel { get; }
        public ICommand Save { get; }

        private string Folder { get; }

        public string FolderPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Folder);
        public string Path => System.IO.Path.Combine(FolderPath, Filename);

        public CollectionSavePageViewModel()
        {
            Title = "Save";
            Cancel = new Command(async () =>
            {
                await Shell.Current.Navigation.PopModalAsync();
            });

            Save = new Command(async () =>
            {
                var (result, text) = ValidateFilename(Filename);

                if (result)
                {
                    if (File.Exists(Path))
                    {
                        var overwrite = await Device.InvokeOnMainThreadAsync<bool>(async () =>
                        {
                            return await Shell.Current.DisplayAlert("Save", $"File {Filename} exists, would you like to overwrite?", "Ok", "Cancel");
                        });
                        if (!overwrite)
                        {
                            return;
                        }
                    }

                    await SaveFile();
                    await Shell.Current.Navigation.PopModalAsync();
                }
                else
                {
                    ErrorText = text;
                }
            });
        }

        private async Task SaveFile()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var text = System.Text.Json.JsonSerializer.Serialize(Items);
                    File.WriteAllText(Path, text);
                    await Shell.Current.DisplayAlert("Success", Path, "Ok");
                });
            }
            catch(Exception e)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", e.ToString(), "Ok");
                });
            }
        }

        private static (bool result, string? text) ValidateFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return (false, "Error: Filename is empty");
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(filename, "^[a-zA-Z-_0-9]+$"))
            {
                return (false, "Error: Only characters a-Z 0-9 - _ allowed.");
            }

            return (true, default);
        }

        public CollectionSavePageViewModel(string folder, IEnumerable<object> items, string filename = "") : this()
        {
            Items = items;
            Filename = filename;
            Folder = folder;
        }

    }
}
