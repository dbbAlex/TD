using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Shot : DynamicGameObject
    {
        public Shot(float x, float y, float width, float height, float speed, int damage, Enemy enemy) : base(x, y,
            width, height, speed)
        {
            Damage = damage;
            Enemy = enemy;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.shot),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape = new Ellipse
            {
                Name = GetType().Name,
                Width = width / 3,
                Height = height / 3,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public int Damage { get; set; }
        public Enemy Enemy { get; set; }
        public bool Active { get; set; }

        public override void Start(GameControl gameControl)
        {
            Canvas.SetLeft(Shape, X * Width + (Width / 3) / 2);
            Canvas.SetTop(Shape, Y * Height + (Height / 3) / 2);
            gameControl.Canvas.Children.Add(Shape);
            Active = true;
        }

        public override void Update(GameControl gameControl)
        {
            /*// get enemy center coordinates
            var enemyCenterX = Enemy.X * Enemy.Width + Enemy.Width / 2;
            var enemyCenterY = Enemy.Y * Enemy.Health + Enemy.Height / 2;
            
            // get our center coordinates
            var centerX = X * Width + Width / 2;
            var centerY = Y * Height + Height / 2;
            
            // get unit for x and y
            var unitX =  enemyCenterX - centerX;
            var unitY = enemyCenterY - centerY;
            
            // calculate distance 
            var distance = (float) Math.Sqrt(Math.Pow(enemyCenterX - centerX, 2) +
                                             Math.Pow(enemyCenterY - centerY, 2));*/


            // set x coordinate
            if (X < Enemy.X)
            {
                X += Speed;
                if (X >= Enemy.X) X = Enemy.X;
            }
            else if (X > Enemy.X)
            {
                X -= Speed;
                if (X <= Enemy.X) X = Enemy.X;
            }

            // set y coordinate
            if (Y < Enemy.Y)
            {
                Y += Speed;
                if (Y >= Enemy.Y) Y = Enemy.Y;
            }
            else if (Y > Enemy.Y)
            {
                Y -= Speed;
                if (Y <= Enemy.Y) Y = Enemy.Y;
            }
            // check for collision
            /*var intersectionDetail = Shape.RenderedGeometry.FillContainsWithDetail(Enemy.Shape.RenderedGeometry);
            if (intersectionDetail == IntersectionDetail.Intersects)
            {
                Active = false;
                // move collision detection to render() because fillContainsWithDetails works with shapes position
                // TODO: create destroy method in GameObject
                // TODO: create a class to manage damage and call destroy methods
            }*/
            // update fields

            base.Update(gameControl);
        }

        public override void Render(GameControl gameControl)
        {
            Shape.Width = Width / 3;
            Shape.Height = Height / 3;
            Canvas.SetLeft(Shape, X * Width + (Width / 3) / 2);
            Canvas.SetTop(Shape, Y * Height + (Height / 3) / 2);
        }
    }
}