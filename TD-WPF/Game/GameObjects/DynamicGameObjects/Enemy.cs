using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.RoundObjects;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Enemy : DynamicGameObject
    {
        public Enemy(float x, float y, float width, float height, float speed, int health, int damage, Wave wave) :
            base(x, y, width, height, speed)
        {
            this.wave = wave;
            this.health = health;
            this.damage = damage;
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

        public int health { get; set; }
        public int damage { get; set; }
        public Wave wave { get; set; }
        public bool active { get; set; }

        public override void start(GameControl gameControl)
        {
            base.start(gameControl);
            active = true;
        }

        public override void update(GameControl gameControl, float deltaTime)
        {
            // get next path
            var next = GameUtils.GameUtils.GetNextPath(GameUtils.GameUtils.GetPathPosition(this, gameControl),
                gameControl);
            if (next == null)
            {
                active = false;
                wave.enemies.Remove(this);
                return;
            }

            if (x != next.x)
            {
                x *= speed * deltaTime;
                if (x > next.x)
                    x = next.x;
            }
            else if (y != next.y)
            {
                y *= speed * deltaTime;
                if (y > next.y)
                    y = next.y;
            }

            // call base update for size
            base.update(gameControl, deltaTime);
        }
    }
}