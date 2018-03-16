using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.RoundObjects;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Enemy : DynamicGameObject
    {
        public int health { get; set; }
        public int damage { get; set; }
        public Wave wave { get; set; }
        public bool active { get; set; } = false;
        public Enemy(float x, float y, float width, float height, float speed, int health, int damage, Wave wave) : base(x, y, width, height, speed)
        {
            this.wave = wave;
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

        public override void start(GameControl gameControl)
        {
            base.start(gameControl);
            active = true;
        }

        public override void update(GameControl gameControl, float deltaTime)
        {
            // get next path
            StaticGameObjects.Path next = GameUtils.GameUtils.GetNextPath(GameUtils.GameUtils.GetPathPosition(this, gameControl), gameControl);
            if(next == null)
            {
                this.active = false;
                this.wave.enemies.Remove(this);
                return;
            }

            if(this.x != next.x)
            {
                this.x *= speed * deltaTime;
                if (this.x > next.x)
                    this.x = next.x;
            }
            else if(this.y != next.y)
            {
                this.y *= speed * deltaTime;
                if (this.y > next.y)
                    this.y = next.y;
            }
            // call base update for size
            base.update(gameControl, deltaTime);
        }
    }
}
