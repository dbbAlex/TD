using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Enemy : DynamicGameObject
    {
        public int health { get; set; }
        public int damage { get; set; }
        public Enemy(float x, float y, float width, float height, float speed, int health, int damage) : base(x, y, width, height, speed)
        {
            this.health = health;
            this.damage = damage;
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.gegner),
                Convert.ToInt32(width), Convert.ToInt32(height));
            this.shape = new Ellipse()
            {
                Name = this.GetType().Name,
                Width = width,
                Height = height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(this.image.GetHbitmap(),
                                  IntPtr.Zero,
                                  Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public override void update(GameControl gameControl, float deltaTime)
        {
            // get next path
            StaticGameObjects.Path next = GameUtils.GameUtils.GetNextPath(GameUtils.GameUtils.GetPathPosition(this, gameControl), gameControl);

            // get unit for x and y
            float unitX = next.x * next.width - this.x * this.width;
            float unitY = next.y * next.height - this.y * this.height;
            // calculate distance 
            float distance = (float)Math.Sqrt(Math.Pow((this.x * this.width - next.x * next.width), 2) + Math.Pow((this.y * this.height - next.y * next.height), 2));
            // calculate new coordinates
            float _x = x + unitX * speed;
            float _y = y + unitY * speed;
            // check for collision

            // update fields
            this.x = _x;
            this.y = _y;

            // call base update for size
            base.update(gameControl, deltaTime);
        }
    }
}
