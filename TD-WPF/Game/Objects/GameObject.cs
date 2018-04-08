using System;
using System.Drawing;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace TD_WPF.Game.Objects
{
    public abstract class GameObject
    {
        protected GameObject(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public GameObject()
        {
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        [ScriptIgnore] public virtual Bitmap Image { get; } = null;

        [ScriptIgnore] public Shape Shape { get; protected set; }

        [ScriptIgnore] public bool Active { get; protected set; }

        public virtual void Render(GameControl gameControl)
        {
            if (!Active) return;
            Shape.Width = Width;
            Shape.Height = Height;
            Canvas.SetLeft(Shape, X * Width);
            Canvas.SetTop(Shape, Y * Height);
        }

        public virtual void Start(GameControl gameControl)
        {
            if (Active) return;
            Shape = new Rectangle
            {
                Name = GetType().Name,
                Width = Width,
                Height = Height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
            Canvas.SetLeft(Shape, X * Width);
            Canvas.SetTop(Shape, Y * Height);
            Active = true;
            gameControl.Canvas.Children.Add(Shape);
        }


        public virtual void Update(GameControl gameControl)
        {
            if (!Active) return;
            Width = gameControl.Canvas.ActualWidth / gameControl.GameCreator.X;
            Height = gameControl.Canvas.ActualHeight / gameControl.GameCreator.Y;
        }

        public virtual void Destroy(GameControl gameControl)
        {
            Active = false;
            gameControl.Canvas.Children.Remove(Shape);
            Shape = null;
        }

        public virtual void Deaktivate()
        {
            Active = false;
        }
    }
}