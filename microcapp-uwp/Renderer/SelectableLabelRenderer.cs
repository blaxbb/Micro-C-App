using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using micro_c_app.UWP.Renderer;
using micro_c_app.Views;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace micro_c_app.UWP.Renderer
{
    public class SelectableLabelRenderer : ViewRenderer<SelectableLabel, TextBlock>
    {
        TextBlock textBlock;

        public SelectableLabelRenderer()
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(e.PropertyName == SelectableLabel.TextProperty.PropertyName)
            {
                textBlock.Text = ((SelectableLabel)Element).Text ?? "";
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
                textBlock = new TextBlock();
            }

            textBlock.IsTextSelectionEnabled = true;
            textBlock.Text = label.Text ?? "";
            switch (label.FontAttributes)
            {
                case FontAttributes.Bold:
                    textBlock.FontWeight = FontWeights.Bold;
                    break;
                case FontAttributes.Italic:
                    textBlock.FontStyle = Windows.UI.Text.FontStyle.Italic;
                    break;
                case FontAttributes.None:
                default:
                    textBlock.FontStyle = Windows.UI.Text.FontStyle.Normal;
                    break;
            }
            switch (label.TextDecorations)
            {
                case Xamarin.Forms.TextDecorations.Strikethrough:
                    textBlock.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                    break;
                case Xamarin.Forms.TextDecorations.Underline:
                    textBlock.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                    break;
            }

            switch(label.VerticalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Start:
                default:
                    textBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                    break;
                case Xamarin.Forms.TextAlignment.Center:
                    textBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    textBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                    break;
            }

            switch (label.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Start:
                    textBlock.HorizontalTextAlignment = Windows.UI.Xaml.TextAlignment.Start;
                    break;
                case Xamarin.Forms.TextAlignment.Center:
                    textBlock.HorizontalTextAlignment = Windows.UI.Xaml.TextAlignment.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    textBlock.HorizontalTextAlignment = Windows.UI.Xaml.TextAlignment.End;
                    break;
            }

            if (label.FontSize > 0)
            {
                textBlock.FontSize = label.FontSize;
            }

            SetNativeControl(textBlock);
        }
    }
}