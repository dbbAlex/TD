using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TD_WPF.Game.GameObjects
{
    public abstract class GameObject
    {
        protected GameObject(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            Shape = new System.Windows.Shapes.Rectangle
            {
                Name = GetType().Name,
                Width = this.Width,
                Height = this.Height,
            };
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        protected Bitmap Image { get; set; }
        public Shape Shape { get; protected set; }

        public virtual void Render(GameControl gameControl)
        {
            Shape.Width = Width;
            Shape.Height = Height;
            Canvas.SetLeft(Shape, X * Width);
            Canvas.SetTop(Shape, Y * Height);
        }

        public virtual void Start(GameControl gameControl)
        {
            Canvas.SetLeft(Shape, X * Width);
            Canvas.SetTop(Shape, Y * Height);
            gameControl.Canvas.Children.Add(Shape);
        }


        public virtual void Update(GameControl gameControl)
        {
            Width = (float) gameControl.Canvas.ActualWidth / gameControl.GameCreator.X;
            Height = (float) gameControl.Canvas.ActualHeight / gameControl.GameCreator.Y;
        }
    }
}