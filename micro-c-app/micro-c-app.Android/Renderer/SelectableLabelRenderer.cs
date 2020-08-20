using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using micro_c_app.Droid.Renderer;
using micro_c_app.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace micro_c_app.Droid.Renderer
{
    public class SelectableLabelRenderer : ViewRenderer<SelectableLabel, TextView>
    {
        TextView textView;

        public SelectableLabelRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<SelectableLabel> e)
        {
            base.OnElementChanged(e);

            var label = (SelectableLabel)Element;
            if (label == null)
                return;

            if (Control == null)
            {
                textView = new TextView(this.Context);
            }

            textView.Enabled = true;
            textView.Focusable = true;
            textView.LongClickable = true;
            textView.SetTextIsSelectable(true);

            // Initial properties Set
            textView.Text = label.Text;
            textView.SetTextColor(label.TextColor.ToAndroid());
            switch (label.FontAttributes)
            {
                case FontAttributes.None:
                    textView.SetTypeface(null, Android.Graphics.TypefaceStyle.Normal);
                    break;
                case FontAttributes.Bold:
                    textView.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
                    break;
                case FontAttributes.Italic:
                    textView.SetTypeface(null, Android.Graphics.TypefaceStyle.Italic);
                    break;
                default:
                    textView.SetTypeface(null, Android.Graphics.TypefaceStyle.Normal);
                    break;
            }
            switch (label.TextDecorations)
            {
                case TextDecorations.Strikethrough:
                    textView.PaintFlags = textView.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;
                    break;
                case TextDecorations.Underline:
                    textView.PaintFlags = textView.PaintFlags | Android.Graphics.PaintFlags.UnderlineText;
                    break;
            }

            GravityFlags vert;
            switch(label.VerticalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Start:
                default:
                    vert = GravityFlags.Top;
                    break;
                case Xamarin.Forms.TextAlignment.Center:
                    vert = GravityFlags.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    vert = GravityFlags.Bottom;
                    break;
            }

            switch (label.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Start:
                    textView.Gravity = vert | GravityFlags.Left;
                    break;
                case Xamarin.Forms.TextAlignment.Center:
                    textView.Gravity = vert | GravityFlags.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    textView.Gravity = vert | GravityFlags.Right;
                    break;
            }



            textView.TextSize = (float)label.FontSize;

            SetNativeControl(textView);
        }
    }
}