using Foundation;
using micro_c_app;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(micro_c_app.iOS.KeyboardHelper))]
namespace micro_c_app.iOS
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public KeyboardHelper()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }
        public event EventHandler<KeyboardHelperEventArgs> KeyboardChanged;

        private void OnKeyboardNotification(NSNotification notification)
        {
            var visible = notification.Name == UIKeyboard.WillShowNotification;
            var keyboardFrame = visible ? UIKeyboard.FrameEndFromNotification(notification) : UIKeyboard.FrameBeginFromNotification(notification);
            if(KeyboardChanged != null)
            {
                KeyboardChanged(this, new KeyboardHelperEventArgs(visible, (float)keyboardFrame.Height));
            }
        }
    }
}