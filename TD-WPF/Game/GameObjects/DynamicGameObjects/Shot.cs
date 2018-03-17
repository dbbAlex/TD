using System;
using System.Drawing;
using System.Windows;
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
            Image = ImageTool.ResizeImage(new Bitmap(Resource.gegner),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape = new Ellipse
            {
                Name = GetType().Name,
                Width = width,
                Height = height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public int Damage { get; set; }
        public Enemy Enemy { get; set; }

        public override void Update(GameControl gameControl)
        {
            // get unit for x and y
            var unitX = Enemy.X * Enemy.Width - X * Width;
            var unitY = Enemy.Y * Enemy.Height - Y * Height;
            // calculate distance 
            var distance = (float) Math.Sqrt(Math.Pow(X * Width - Enemy.X * Enemy.Width, 2) +
                                             Math.Pow(Y * Height - Enemy.Y * Enemy.Height, 2));
            // calculate new coordinates
            var _x = X + unitX * Speed;
            var _y = Y + unitY * Speed;
            // check for collision

            // update fields
            X = _x;
            Y = _y;

            base.Update(gameControl);
        }
    }
}