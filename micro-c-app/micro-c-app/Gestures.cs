using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app
{
    //
    //https://forums.xamarin.com/discussion/comment/253375/#Comment_253375
    //
    public static class Gestures
    {
        public static readonly BindableProperty TappedProperty = BindableProperty.CreateAttached("Tapped", typeof(ICommand), typeof(Gestures), null, propertyChanged: CommandChanged);
        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is View view)
            {
                _ = GetOrCreateEffect(view);
            }
        }

        private static GestureEffect GetOrCreateEffect(View view)
        {
            var effect = (GestureEffect)view.Effects.FirstOrDefault(e => e is GestureEffect);
            if(effect == null)
            {
                effect = new GestureEffect();
                view.Effects.Add(effect);
            }

            return effect;
        }

        public static ICommand GetCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(TappedProperty);
        }

        public static void SetTapped(BindableObject view, ICommand value)
        {
            view.SetValue(TappedProperty, value);
        }

    }

    public class GestureEffect : RoutingEffect
    {
        public GestureEffect() : base("micro_c_app.TapWithPositionGestureEffect")
        {

        }
    }
}
