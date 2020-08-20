using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

/*
 * https://medium.com/@HeikkiDev/selectable-label-on-xamarin-forms-9b050267bf8e
 */
namespace micro_c_app.Views
{
    public class SelectableLabel : View
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SelectableLabel), default(string));
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SelectableLabel), Color.Black);
        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SelectableLabel), FontAttributes.None);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SelectableLabel), -1.0);
        public static readonly BindableProperty TextDecorationsPropery = BindableProperty.Create(nameof(TextDecorations), typeof(TextDecorations), typeof(SelectableLabel), TextDecorations.None);
        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(SelectableLabel), TextAlignment.Start);
        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(nameof(VerticalTextAlignment), typeof(TextAlignment), typeof(SelectableLabel), TextAlignment.Start);

        public SelectableLabel()
        {

        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        [TypeConverter(typeof(TextDecorationConverter))]
        public TextDecorations TextDecorations
        {
            get { return (TextDecorations)GetValue(TextDecorationsPropery); }
            set { SetValue(TextDecorationsPropery, value); }
        }

        public TextAlignment HorizontalTextAlignment
        {
            get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        public TextAlignment VerticalTextAlignment
        {
            get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }
    }
}