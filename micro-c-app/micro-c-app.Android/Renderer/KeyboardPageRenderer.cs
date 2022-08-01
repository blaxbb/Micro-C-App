using System;
using Xamarin.Forms;

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
            // Letter
            handled = true;
            _page.OnKeyUp(keyCode.ToString());
        }
        else if ((keyCode >= Keycode.Num0 && keyCode <= Keycode.Num9) ||
                    (keyCode >= Keycode.Numpad0 && keyCode <= Keycode.Num9))
        {
            // Number
            handled = true;
            _page.OnKeyUp(keyCode.ToString());
        }
        else if(keyCode == Keycode.Enter || keyCode == Keycode.Return)
        {
            handled = true;
            _page.OnEnter();
        }

        return handled || base.OnKeyUp(keyCode, e);
    }
}