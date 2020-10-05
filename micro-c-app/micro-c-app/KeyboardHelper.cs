using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace micro_c_app
{
    //https://stackoverflow.com/a/31683183
    public static class KeyboardHelper
    {
        private static IKeyboardHelper? keyboardHelper = null;
        public static void Init()
        {
            if(keyboardHelper == null)
            {
                keyboardHelper = DependencyService.Get<IKeyboardHelper>();
            }
        }

        public static event EventHandler<KeyboardHelperEventArgs> KeyboardChanged
        {
            add
            {
                Init();
                if (keyboardHelper != null)
                {
                    keyboardHelper.KeyboardChanged += value;
                }
            }
            remove
            {
                Init();
                if (keyboardHelper != null)
                {
                    keyboardHelper.KeyboardChanged -= value;
                }
            }
        }
    }

    public interface IKeyboardHelper
    {
        event EventHandler<KeyboardHelperEventArgs> KeyboardChanged;
    }

    public class KeyboardHelperEventArgs : EventArgs
    {
        public readonly bool Visible;
        public readonly float Height;

        public KeyboardHelperEventArgs(bool visible, float height)
        {
            Visible = visible;
            Height = height;
        }
    }
}
