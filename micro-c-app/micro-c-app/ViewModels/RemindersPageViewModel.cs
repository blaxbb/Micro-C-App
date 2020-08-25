using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class RemindersPageViewModel : BaseViewModel
    {
        public ObservableCollection<Reminder> Reminders { get; }
        public ICommand Edit { get; }
        public ICommand Delete { get; }
        public ICommand CheckAll { get; }

        INotificationManager notificationManager { get; }
        public RemindersPageViewModel()
        {
            Title = "Reminders";

            notificationManager = DependencyService.Get<INotificationManager>();

            Reminder.LoadAll();
            Reminders = new ObservableCollection<Reminder>(Reminder.AllReminders);
            Edit = new Command<Reminder>(async (Reminder r) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var vm = new ReminderEditPageViewModel() { Reminder = r };
                    var page = new ReminderEditPage() { BindingContext = vm };
                    await Shell.Current.Navigation.PushAsync(page);
                });
            });

            Delete = new Command<Reminder>(async (Reminder r) =>
            {
                try
                {
                    Reminders.Remove(r);
                    Reminder.AllReminders.Remove(r);
                    
                    /*
                     * See QuotePageViewModel for info
                     */
                    Reminders.Clear();
                    foreach (var tmp in Reminder.AllReminders)
                    {
                        Reminders.Add(tmp);
                    }

                    Reminder.SaveAll();
                }
                catch(Exception e)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Exception", e.ToString(), "ok");
                    });
                }
            });

            CheckAll = new Command(() =>
            {
                Reminder.CheckReminders(notificationManager);
            });
        }
    }
}