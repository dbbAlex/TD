using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.RoundObjects;
using TD_WPF.Properties;
using TD_WPF.Tools;
using Path = TD_WPF.Game.GameObjects.StaticGameObjects.Path;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Enemy : DynamicGameObject
    {
        public Enemy(float x, float y, float width, float height, float speed, int health, int damage, Wave wave, int pathPosition) :
            base(x, y, width, height, speed)
        {
            Wave = wave;
            Health = health;
            Damage = damage;
            PathPosition = pathPosition;
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

        public int Health { get; set; }
        public int Damage { get; set; }
        public Wave Wave { get; set; }
        public bool Active { get; private set; }
        private  int PathPosition { get; set; }


        public override void Start(GameControl gameControl)
        {
            base.Start(gameControl);
            Active = true;
        }

        public override void Update(GameControl gameControl)
        {
            Path next = gameControl.GameCreator.Paths.FirstOrDefault(p => p.Index == PathPosition+1);
            if (next == null)
            {
                Active = false;
                return;
            }

            if (X < next.X)
            {
                X += Speed;
                if (X >= next.X)
                {
                    X = next.X;
                    PathPosition++;
                }
            }
            else if (X > next.X)
            {
                X -= Speed;
                if (X <= next.X)
                {
                    X = next.X;
                    PathPosition++;
                }
            }
            else if (Y < next.Y)
            {
                Y += Speed;
                if (Y >= next.Y)
                {
                    Y = next.Y;
                    PathPosition++;
                }
            }
            else if (Y > next.Y)
            {
                Y -= Speed;
                if (Y <= next.Y)
                {
                    Y = next.Y;
                    PathPosition++;
                }
            }
            base.Update(gameControl);
        }
    }
}