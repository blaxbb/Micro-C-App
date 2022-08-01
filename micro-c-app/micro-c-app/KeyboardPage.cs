using System;
using Xamarin.Forms;

namespace micro_c_app
{
    public class KeyboardPage : ContentPage
    {
        public string Name { get; set; }

        public virtual void OnEnter() { return; }

        public virtual void OnKeyUp(string text) { return; }
    }
}