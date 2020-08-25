using micro_c_app.Models;
using micro_c_app.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ReminderEditPageViewModel : BaseViewModel
    {
        private Reminder reminder;
        private string message;
        private bool newItem;

        public Reminder Reminder { get => reminder; set { SetProperty(ref reminder, value); Message = reminder.Message; } }
        public string Message { get => message; set => SetProperty(ref message, value); }
        public bool NewItem { get => newItem; set => newItem = SetProperty(ref newItem, value); }

        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public ReminderEditPageViewModel()
        {
            Title = "Edit Reminder";
            Save = new Command(async () =>
            {
                Reminder.Message = message;
                if (NewItem)
                {
                    Reminder.Add(Reminder);
                }
                Reminder.SaveAll();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.Navigation.PopAsync();
                });
            });
            Cancel = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.Navigation.PopAsync();
                });
            });
        }
    }
}