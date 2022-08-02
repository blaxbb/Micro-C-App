using Android.Content;
using Android.Runtime;
using Android.Views;
using micro_c_app;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(KeyboardPage), typeof(KeyboardPageRenderer))]
public class KeyboardPageRenderer : PageRenderer
{
    private KeyboardPage _page => Element as KeyboardPage;

    public KeyboardPageRenderer(Context context) : base(context)
    {
        Focusable = true;
        FocusableInTouchMode = true;
    }

    protected override void OnElementChanged(
        ElementChangedEventArgs<Page> e)
    {
        base.OnElementChanged(e);

        if (Visibility == ViewStates.Visible)
            RequestFocus();

        _page.Appearing += (sender, args) =>
        {
            RequestFocus();
        };
    }

    public override bool OnKeyUp(
        [GeneratedEnum] Keycode keyCode,
        KeyEvent e)
    {
        var handled = false;

        if (keyCode >= Keycode.A && keyCode <= Keycode.Z)
        {
            handled = true;
            _page.OnKeyUp(keyCode.ToString());
        }
        else if (keyCode >= Keycode.Num0 && keyCode <= Keycode.Num9)
        {
            var val = (int)(keyCode - Keycode.Num0);
            handled = true;
            _page.OnKeyUp(val.ToString());
        }
        else if (keyCode >= Keycode.Numpad0 && keyCode <= Keycode.Numpad9)
        {
            var val = (int)(keyCode - Keycode.Numpad0);
            handled = true;
            _page.OnKeyUp(val.ToString());
        }
        else if(keyCode == Keycode.Enter || keyCode == Keycode.NumpadEnter)
        {
            handled = true;
            _page.OnEnter();
        }
        else if(keyCode == Keycode.Minus)
        {
            handled = true;
            _page.OnKeyUp("-");
        }

        return handled || base.OnKeyUp(keyCode, e);
    }
}