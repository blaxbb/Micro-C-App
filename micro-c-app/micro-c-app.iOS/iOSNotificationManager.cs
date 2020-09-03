using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Foundation;
using UIKit;
using UserNotifications;
using micro_c_app;

[assembly: Xamarin.Forms.Dependency(typeof(micro_c_app.iOS.iOSNotificationManager))]
namespace micro_c_app.iOS
{
    public class iOSNotificationManager : INotificationManager
    {
        int messageId = -1;

        bool hasNotificationsPermission;

        public event EventHandler NotificationReceived;

        public void Initialize()
        {
            // request the permission to use local notifications
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                hasNotificationsPermission = approved;
            });
        }

        public int ScheduleNotification(string title, string message)
        {
            // EARLY OUT: app doesn't have permissions
            if (!hasNotificationsPermission)
            {
                return -1;
            }

            messageId++;

            try
            {
                var content = new UNMutableNotificationContent()
                {
                    Title = title,
                    Subtitle = "",
                    Body = message,
                    Badge = 1
                };


            // Local notifications can be time or location based
            // Create a time-based trigger, interval is in seconds and must be greater than 0
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.25, false);

            var request = UNNotificationRequest.FromIdentifier(messageId.ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    throw new Exception($"Failed to schedule notification: {err}");
                }
            });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return messageId;
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message
            };
            NotificationReceived?.Invoke(null, args);
        }
    }
}