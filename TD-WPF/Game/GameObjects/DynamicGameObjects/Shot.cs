using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Shot : DynamicGameObject
    {
        public int damage { get; set; }
        public Enemy enemy { get; set; }
        public Shot(float x, float y, float width, float height, float speed, int damage, Enemy enemy) : base(x, y, width, height, speed)
        {
            this.damage = damage;
            this.enemy = enemy;
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
            // get unit for x and y
            float unitX = this.enemy.x * this.enemy.width - this.x * this.width;
            float unitY = this.enemy.y * this.enemy.height - this.y * this.height;
            // calculate distance 
            float distance = (float)Math.Sqrt(Math.Pow((this.x * this.width - this.enemy.x * this.enemy.width), 2) + Math.Pow((this.y * this.height - this.enemy.y * this.enemy.height), 2));
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
