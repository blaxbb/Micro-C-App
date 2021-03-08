using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models
{
    public static class HelpMessages
    {
        public static string[] Messages = {
            "You can change categories on this screen in the settings menu.",
        };

        public static string? GetNextMessage()
        {
            var index = SettingsPage.HelpMessageIndex();
            if(index >= 0 && index < Messages.Length)
            {
                return Messages[index];
            }

            return null;
        }
    }
}
