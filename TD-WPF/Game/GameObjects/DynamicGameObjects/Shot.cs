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
            this.damage = damage;
            this.enemy = enemy;
            image = ImageTool.ResizeImage(new Bitmap(Resource.gegner),
                Convert.ToInt32(width), Convert.ToInt32(height));
            shape = new Ellipse
            {
                Name = GetType().Name,
                Width = width,
                Height = height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public int damage { get; set; }
        public Enemy enemy { get; set; }

        public override void update(GameControl gameControl, float deltaTime)
        {
            // get unit for x and y
            var unitX = enemy.x * enemy.width - x * width;
            var unitY = enemy.y * enemy.height - y * height;
            // calculate distance 
            var distance = (float) Math.Sqrt(Math.Pow(x * width - enemy.x * enemy.width, 2) +
                                             Math.Pow(y * height - enemy.y * enemy.height, 2));
            // calculate new coordinates
            var _x = x + unitX * speed;
            var _y = y + unitY * speed;
            // check for collision

            // update fields
            x = _x;
            y = _y;

            // call base update for size
            base.update(gameControl, deltaTime);
        }
    }
}