using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using micro_c_app.iOS.Renderer;
using micro_c_app.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace micro_c_app.iOS.Renderer
{
    public class SelectableLabelRenderer :ViewRenderer<SelectableLabel, UITextView>
    {
        UITextView uiTextView;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == SelectableLabel.TextProperty.PropertyName)
            {
                uiTextView.Text = ((SelectableLabel)Element).Text ?? "";
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SelectableLabel> e)
        {
            base.OnElementChanged(e);

            var label = (SelectableLabel)Element;
            if (label == null)
                return;

            if (Control == null)
            {
                uiTextView = new UITextView();
            }

            var textDecorations = Element.TextDecorations;
#if __MOBILE__
            var newAttributedText = new NSMutableAttributedString(label?.Text ?? "");
            var strikeThroughStyleKey = UIStringAttributeKey.StrikethroughStyle;
            var underlineStyleKey = UIStringAttributeKey.UnderlineStyle;

#else
			var newAttributedText = new NSMutableAttributedString(Control.AttributedStringValue);
			var strikeThroughStyleKey = NSStringAttributeKey.StrikethroughStyle;
			var underlineStyleKey = NSStringAttributeKey.UnderlineStyle;
#endif
            var range = new NSRange(0, newAttributedText.Length);

            if ((textDecorations & TextDecorations.Strikethrough) == 0)
                newAttributedText.RemoveAttribute(strikeThroughStyleKey, range);
            else
                newAttributedText.AddAttribute(strikeThroughStyleKey, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);

            if ((textDecorations & TextDecorations.Underline) == 0)
                newAttributedText.RemoveAttribute(underlineStyleKey, range);
            else
                newAttributedText.AddAttribute(underlineStyleKey, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);

#if __MOBILE__
            uiTextView.AttributedText = newAttributedText;
#else
			uiTextView.AttributedStringValue = newAttributedText;
#endif

            uiTextView.Selectable = true;
            uiTextView.Editable = false;
            uiTextView.ScrollEnabled = true;
            uiTextView.TextContainerInset = UIEdgeInsets.Zero;
            uiTextView.TextContainer.LineFragmentPadding = 0;
            uiTextView.BackgroundColor = UIColor.Clear;

            // Initial properties Set
            uiTextView.Text = label?.Text;
            uiTextView.TextColor = label.TextColor.ToUIColor();
            switch (label.FontAttributes)
            {
                case FontAttributes.None:
                    uiTextView.Font = UIFont.SystemFontOfSize(new nfloat(label.FontSize));
                    break;
                case FontAttributes.Bold:
                    uiTextView.Font = UIFont.BoldSystemFontOfSize(new nfloat(label.FontSize));
                    break;
                case FontAttributes.Italic:
                    uiTextView.Font = UIFont.ItalicSystemFontOfSize(new nfloat(label.FontSize));
                    break;
                default:
                    uiTextView.Font = UIFont.BoldSystemFontOfSize(new nfloat(label.FontSize));
                    break;
            }

            switch (label.HorizontalTextAlignment)
            {
                case TextAlignment.Start:
                    uiTextView.TextAlignment = UITextAlignment.Left;
                    break;
                case TextAlignment.Center:
                    uiTextView.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.End:
                    uiTextView.TextAlignment = UITextAlignment.Right;
                    break;
            }

            SetNativeControl(uiTextView);
        }
    }
}