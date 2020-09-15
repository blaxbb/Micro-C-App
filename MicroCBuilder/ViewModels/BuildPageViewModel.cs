using micro_c_lib.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace MicroCBuilder.ViewModels
{
    public class BuildPageViewModel : BaseViewModel
    {
        private Item selectedItem;

        public string Test { get; set; } = "AFASFSF";
        public ObservableCollection<BuildComponent> Components { get; }
        public BuildComponent.ComponentType ttt = BuildComponent.ComponentType.GPU;

        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand Reset { get; }
        public ICommand Add { get; }
        public ICommand Remove { get; }
        public Item SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public BuildPageViewModel()
        {
            Components = new ObservableCollection<BuildComponent>();
            Enum.GetValues(typeof(BuildComponent.ComponentType)).Cast<BuildComponent.ComponentType>().ToList().ForEach(c => Components.Add(new BuildComponent() { Type = c }));

            Save = new Command(DoSave);

            Load = new Command(DoLoad);

            Reset = new Command(DoReset);

            Remove = new Command<BuildComponent>(DoRemove);
            Add = new Command<BuildComponent.ComponentType>(AddItem);
        }

        private void DoRemove(BuildComponent comp)
        {
            if(comp != null && comp.Item != null)
            {
                comp.Item = null;
            }
            if(Components.Count(c => c.Type == comp.Type) > 1)
            {
                Components.Remove(comp);
            }
        }

        private void AddItem(BuildComponent.ComponentType type)
        {
            InsertAtEndByType(type);
        }

        private BuildComponent InsertAtEndByType(BuildComponent.ComponentType type)
        {
            int index = Components.Count;
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                var existing = Components[i];
                if (existing.Type == type)
                {
                    index = i + 1;
                    break;
                }
            }

            var comp = new BuildComponent() { Type = type };
            Components.Insert(index, comp);
            return comp;
        }

        private void DoReset(object obj)
        {
            foreach (var c in Components)
            {
                c.Item = null;
            }
        }

        private async void DoLoad(object obj)
        {
            //
            //Get a collection of BuildComponents from a .build file
            //
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".build");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                DoReset(obj);
                var text = await Windows.Storage.FileIO.ReadTextAsync(file);
                var components = System.Text.Json.JsonSerializer.Deserialize<List<BuildComponent>>(text);

                //
                //If there is an existing empty component, use that one, otherwise create a new one
                //
                InsertComponents(components);
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private void InsertComponents(List<BuildComponent> fromFile)
        {
            foreach (var loadedComp in fromFile)
            {
                bool found = false;
                foreach(var oldComp in Components)
                {
                    if (oldComp.Type == loadedComp.Type && oldComp.Item == null)
                    {
                        oldComp.Item = loadedComp.Item;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var comp = InsertAtEndByType(loadedComp.Type);
                    comp.Item = loadedComp.Item;
                }
            }
        }

        private async void DoSave(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(Components.Where(c => c.Item != null), new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("MCBuild", new List<string>() { ".build" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Build";

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, json);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Debug.WriteLine("File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " couldn't be saved.");
                }
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }
    }
}
