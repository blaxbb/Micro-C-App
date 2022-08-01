using System;
using Xamarin.Forms;

public class KeyboardPage : ContentPage
{
    public string Name { get; set; }

    public virtual void OnEnter() { return; }

    public virtual void OnKeyUp(string text) { return; }
}