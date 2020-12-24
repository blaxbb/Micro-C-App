using micro_c_app.Views;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app
{
    public static class AnalyticsService
    {
        public static void Track(string name, string value)
        {
            if (!SettingsPage.AnalyticsEnabled())
            {
                return;
            }

            Track(name, "value", value);
        }

        public static void Track(string name, params string[] values)
        {
            if (!SettingsPage.AnalyticsEnabled())
            {
                return;
            }
            
            if (values.Length % 2 != 0)
            {
                System.Diagnostics.Debug.WriteLine("Error: IAnalytics.Track parameter 'values' must be an even total!");
            }

            var props = new Dictionary<string, string>();
            for (int i = 0; i < values.Length - 1; i += 2)
            {
                var key = values[i];
                var value = values[i + 1];
                props[key] = value;
            }

            Analytics.TrackEvent(name, props);
        }

        public static void TrackError(Exception e, string value)
        {
            TrackError(e, "value", value);
        }

        public static void TrackError(Exception e, params string[] values)
        {
            if (!SettingsPage.AnalyticsEnabled())
            {
                return;
            }

            if (values.Length % 2 != 0)
            {
                System.Diagnostics.Debug.WriteLine("Error: IAnalytics.TrackError parameter 'values' must be an even total!");
            }

            var props = new Dictionary<string, string>();
            for (int i = 0; i < values.Length - 1; i += 2)
            {
                var key = values[i];
                var value = values[i + 1];
                props[key] = value;
            }

            Crashes.TrackError(e, props);
        }
    }
}
