using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamarin.Forms.PinchZoomImage
{

    public class PinchZoom : ContentView
    {
        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;

        double maxX => Math.Min(Width / 2, width / 2) - (XMargin / 2);
        double maxY => (Math.Min(Height, height) / 2) - (YMargin / 2);
        double minX => ((width - (Width / 2)) * -1) + (XMargin / 2);
        double minY => ((height - (Height / 2)) * -1) + (YMargin / 2);

        double width => Content.Width * Content.Scale;
        double height => Content.Height * Content.Scale;

        bool DoingPinch = false;

        double XMargin => (Frame.Width - Image.Width) * Content.Scale;
        double YMargin => (Frame.Height - Image.Height) * Content.Scale;

        private Image image;
        private Frame frame;

        public static readonly BindableProperty PanPercentageProperty = BindableProperty.Create(nameof(PanPercentage), typeof(Point), typeof(PinchZoom));
        public Point PanPercentage { get => (Point)GetValue(PanPercentageProperty); set => SetValue(PanPercentageProperty, value); }
        Frame Frame { get
            {
                if(frame == null)
                {
                    FindChildren();
                }
                return frame;
            }
            set => frame = value;
        }
        Image Image { get
            {
                if(image == null)
                {
                    FindChildren();
                }
                return image;
            }
            set => image = value;
        }


        public PinchZoom()
        {
            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.NumberOfTapsRequired = 2;
            tapGesture.Tapped += DoubleTapped;
            GestureRecognizers.Add(tapGesture);

            var tapGesture1 = new TapGestureRecognizer();
            tapGesture1.NumberOfTapsRequired = 1;
            tapGesture1.Tapped += Tapped;
            GestureRecognizers.Add(tapGesture1);
        }

        void FindChildren()
        {
            if (Content is Frame f)
            {
                frame = f;
                if (frame.Content is Image i)
                {
                    image = i;
                }
            }
        }

        public Point GetPanPercent()
        {
            var x = Content.TranslationX - minX;
            var y = Content.TranslationY - minY;

            x /= maxX - minX;
            y /= maxY - minY;

            x = 1 - x;
            y = 1 - y;

            return new Point(x, y);
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                DoingPinch = true;
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }

            if (e.Status == GestureStatus.Running)
            {
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (currentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (currentScale - 1)));

                Content.Scale = currentScale;
                PanPercentage = GetPanPercent();
            }

            if (e.Status == GestureStatus.Completed)
            {
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Content.Scale == 1)
            {
                return;
            }
            
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    if (DoingPinch)
                    {
                        return;
                    }
                    double newX = (e.TotalX * Scale) + xOffset;
                    double newY = (e.TotalY * Scale) + yOffset;

                    if (newX < minX)
                    {
                        newX = minX;
                    }

                    if (newX > maxX)
                    {
                        newX = maxX;
                    }

                    if (newY < minY)
                    {
                        newY = minY;
                    }

                    if (newY > maxY)
                    {
                        newY = maxY;
                    }

                    Content.TranslationX = newX;
                    Content.TranslationY = newY;
                    PanPercentage = GetPanPercent();
                    break;
                case GestureStatus.Completed:
                    DoingPinch = false;
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;
                    break;
            }
        }

        public async void DoubleTapped(object sender, EventArgs e)
        {
            if(Content.Scale > 6)
            {
                Content.Scale = 1;
                Content.TranslationX = 0;
                Content.TranslationY = 0;
                currentScale = 1;
                xOffset = 0;
                yOffset = 0;
                PanPercentage = GetPanPercent();
                return;
            }

            double multiplicator = Math.Pow(2, 1.0 / 10.0);
            startScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;

            for (int i = 0; i < 10; i++)
            {
                currentScale *= multiplicator;
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (0.5 - deltaX) * deltaWidth;

                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (0.5 - deltaY) * deltaHeight;

                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (currentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (currentScale - 1)));

                Content.Scale = currentScale;
                PanPercentage = GetPanPercent();
                await Task.Delay(10);
            }

            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
        }

        public async void Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine($"{GetPanPercent().X}, {GetPanPercent().Y}");
        }
    }
}
