using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace micro_c_app.Models
{
    public class Reminder : NotifyPropertyChangedItem
    {
        public static List<Reminder> AllReminders { get; set; }

        private string name;
        private string sku;
        private string url;
        private string message;
        private bool notified;
        private string pictureURL;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string SKU { get => sku; set => SetProperty(ref sku, value); }
        public string PictureURL { get => pictureURL; set => SetProperty(ref pictureURL, value); }
        public string URL { get => url; set => SetProperty(ref url, value); }
        public string Message { get => message ?? ""; set => SetProperty(ref message, value); }
        public bool Notified { get => notified; set => SetProperty(ref notified, value); }

        public const string FILENAME = "Reminders.json";
        static string Path => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FILENAME);


        public Reminder()
        {

        }

        public Reminder(Item item)
        {
            Name = item.Name;
            SKU = item.SKU;
            URL = item.URL;
            PictureURL = item.PictureUrls?.FirstOrDefault() ?? "";
        }

        public async Task<bool> CheckStock()
        {
            var item = await Item.FromUrl(URL);
            return item.Stock != "Sold Out";
        }

        public static void Add(Reminder reminder)
        {
            AllReminders.Add(reminder);
        }

        public static List<Reminder> LoadAll()
        {
            if (AllReminders != null)
            {
                return AllReminders;
            }

            try
            {
                if (File.Exists(Path))
                {
                    var text = File.ReadAllText(Path);
                    AllReminders = System.Text.Json.JsonSerializer.Deserialize<List<Reminder>>(text);
                }
                else
                {
                    AllReminders = new List<Reminder>();
                }
            }
            catch (Exception e)
            {
                Xamarin.Forms.Shell.Current.DisplayAlert("Exception", e.ToString(), "OK").Wait();
                AllReminders = new List<Reminder>();
            }

            return AllReminders;
        }

        static Task saveTask;
        static bool repeatSaveTask;

        public static void SaveAll()
        {
            //dont start saving if already saving
            if (saveTask == null || saveTask.IsCompleted)
            {
                repeatSaveTask = false;
                saveTask = Task.Run(() =>
                {
                    WriteFile();
                }).ContinueWith((_) =>
                {
                    if (repeatSaveTask)
                    {
                        SaveAll();
                    }
                });
            }
            else
            {
                //if something updated, schedule another save
                repeatSaveTask = true;
            }
        }

        private static void WriteFile()
        {
            if (AllReminders == null)
            {
                return;
            }

            try
            {
                var text = JsonSerializer.Serialize(AllReminders);
                File.WriteAllText(Path, text);
            }
            catch (Exception e)
            {
                Xamarin.Forms.Shell.Current.DisplayAlert("Exception", e.ToString(), "OK").Wait();
            }
        }

        public static void CheckReminders(INotificationManager notificationManager)
        {
            Task.Run(async () =>
            {
                Reminder.LoadAll();
                List<Reminder> notified = new List<Reminder>();
                foreach (var reminder in Reminder.AllReminders.Where(r => !r.Notified))
                {
                    var status = await reminder.CheckStock();

                    if (status)
                    {
                        notificationManager?.ScheduleNotification($"Restocked: {reminder.Name}", reminder.Message);
                        //await Device.InvokeOnMainThreadAsync(async () =>
                        //{
                        //    await Shell.Current.DisplayAlert("Reminder", $"{reminder.Name} - {reminder.Message}", "ok");
                        //});
                        notified.Add(reminder);
                    }
                }
                if (notified.Count > 0)
                {
                    foreach (var r in notified)
                    {
                        r.Notified = true;
                    }
                    Reminder.SaveAll();
                }
            });
        }
    }
}
