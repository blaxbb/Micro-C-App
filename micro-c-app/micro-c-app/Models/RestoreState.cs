using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using micro_c_app.ViewModels;

namespace micro_c_app.Models
{
    public class RestoreState
    {
        public static RestoreState Instance;

        public List<BuildComponent> BuildComponents { get; set; }
        public List<Item> QuoteItems { get; set; }
        public BuildViewModel BuildVM { get; set; }

        public const string FILENAME = "RestoreState.json";
        static string Path => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.txt");
        public static void Load()
        {
            if (Instance == null)
            {
                Instance = new RestoreState();
            }
            try
            {
                if (File.Exists(Path))
                {
                    var text = File.ReadAllText(Path);
                    Instance = JsonSerializer.Deserialize<RestoreState>(text);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static Task saveTask;
        static bool repeatSaveTask;
        public static void Save()
        {
            //dont start saving if already saving
            if (saveTask == null || saveTask.IsCompleted)
            {
                repeatSaveTask = false;
                saveTask = Task.Run(() =>
                {
                    if (Instance == null)
                    {
                        Instance = new RestoreState();
                    }

                    Instance.WriteFile();
                }).ContinueWith((_) =>
                {
                    if (repeatSaveTask)
                    {
                        RestoreState.Save();
                    }
                });
            }
            else
            {
                //if something updated, schedule another save
                repeatSaveTask = true;
            }
        }

        private void WriteFile()
        {
            try
            {
                var text = JsonSerializer.Serialize(Instance, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(Path, text);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
