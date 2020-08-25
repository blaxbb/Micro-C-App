using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app
{
    public interface INotificationManager
    {
        event EventHandler NotificationRecieved;
        void Initialize();
        int ScheduleNotification(string title, string message);
        void RecieveNotification(string title, string message);
    }

    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
