using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using micro_c_app.Droid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("micro_c_app")]
[assembly: ExportEffect(typeof(TapWithPositionGestureEffect), nameof(TapWithPositionGestureEffect))]

namespace micro_c_app.Droid
{
    public class TapWithPositionGestureEffect : PlatformEffect
    {
        private GestureDetectorCompat gestureRecognizer;
        private readonly InternalGestureDetector tapDetector;
        private ICommand tapWithPositionCommand;
        private DisplayMetrics displayMetrics;

        public TapWithPositionGestureEffect()
        {
            tapDetector = new InternalGestureDetector()
            {
                TapAction = motion =>
                {
                    var tap = tapWithPositionCommand;
                    if (tap != null)
                    {
                        var x = motion.GetX();
                        var y = motion.GetY();

                        var point = PxToDp(new Point(x, y));
                        if (tap.CanExecute(point))
                        {
                            tap.Execute(point);
                        }
                    }
                }
            };
        }

        private Point PxToDp(Point point)
        {
            var control = Control ?? Container;
            var context = control.Context;
            displayMetrics = context.Resources.DisplayMetrics;

            point.X = point.X / displayMetrics.Density;
            point.Y = point.Y / displayMetrics.Density;
            return point;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapWithPositionCommand = Gestures.GetCommand(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;
            var context = control.Context;
            displayMetrics = context.Resources.DisplayMetrics;
            tapDetector.Density = displayMetrics.Density;
            if(gestureRecognizer == null)
            {
                gestureRecognizer = new GestureDetectorCompat(context, tapDetector);
            }

            control.Touch += ControlOnTouch;

            OnElementPropertyChanged(new PropertyChangedEventArgs(""));
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Touch -= ControlOnTouch;
        }

        private void ControlOnTouch(object sender, Android.Views.View.TouchEventArgs e)
        {
            gestureRecognizer?.OnTouchEvent(e.Event);
        }


        sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            public Action<MotionEvent> TapAction { get; set; }
            public float Density { get; set; }
            public override bool OnSingleTapUp(MotionEvent e)
            {
                TapAction?.Invoke(e);
                return true;
            }
        }
    }
}