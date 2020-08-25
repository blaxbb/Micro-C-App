using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using micro_c_app.Droid;

using Android.Support.V4.App;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;
using AndroidX.Core.App;
using Android.Graphics;
using Android.Content.Res;

[assembly: Xamarin.Forms.Dependency(typeof(micro_c_app.Droid.AndroidNotificationManager))]
namespace micro_c_app.Droid
{
    public class AndroidNotificationManager : INotificationManager
    {
        public event EventHandler NotificationRecieved;
        NotificationManager manager;

        const string CHANNEL_ID = "default";
        const string CHANNEL_NAME = "default";
        const string CHANNEL_DESCRIPTION = "The default channel for app notifications";
        const int PENDING_INTENT_ID = 0;

        public const string TITLE_KEY = "title";
        public const string MESSAGE_KEY = "message";
        bool initialized = false;
        int messageId = -1;

        public void Initialize()
        {
            CreateNotificationChannel();
        }

        private void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
            if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(CHANNEL_NAME);
                var channel = new NotificationChannel(CHANNEL_ID, channelNameJava, NotificationImportance.Default)
                {
                    Description = CHANNEL_DESCRIPTION
                };
                manager.CreateNotificationChannel(channel);
            }

            initialized = true;
        }

        public void RecieveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message
            };
            NotificationRecieved?.Invoke(null, args);
        }

        public int ScheduleNotification(string title, string message)
        {
            if (!initialized)
            {
                CreateNotificationChannel();
            }

            messageId++;
            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(TITLE_KEY, title);
            intent.PutExtra(MESSAGE_KEY, message);

            var pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, PENDING_INTENT_ID, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.tab_about))
                .SetSmallIcon(Resource.Drawable.tab_about)
                .SetDefaults((int)NotificationDefaults.Vibrate | (int)NotificationDefaults.Sound);
            var notification = builder.Build();
            manager.Notify(messageId, notification);
            return messageId;
        }
    }
}