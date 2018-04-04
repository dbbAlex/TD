using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.Manager;
using TD_WPF.Game.Objects.RoundObjects;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.Objects.DynamicGameObjects
{
    public class Enemy : DynamicGameObject
    {
        public Enemy(float x, float y, float width, float height, float speed, int health, int damage, Wave wave, int money) :
            base(x, y, width, height, speed)
        {
            Wave = wave;
            Health = health;
            Money = money;
            Damage = damage;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.enemy),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape = new Ellipse
            {
                Name = GetType().Name,
                Width = width / 2,
                Height = height / 2,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public int Health { get; set; }
        public int Money { get; set; }
        public int Damage { get; set; }
        public Wave Wave { get; set; }
        private int PathPosition { get; set; }


        public override void Start(GameControl gameControl)
        {
            Active = true;
            base.Update(gameControl);
            Canvas.SetLeft(Shape, X * Width + Width / 2 / 2);
            Canvas.SetTop(Shape, Y * Height + Height / 2 / 2);
            gameControl.Canvas.Children.Add(Shape);
        }

        public override void Update(GameControl gameControl)
        {
            if (!Active) return;
            if (!gameControl.GameManager.Pause)
            {
                var next = gameControl.GameCreator.Paths.FirstOrDefault(p => p.Index == PathPosition + 1);
                if (next == null)
                {
                    // reached the end destroy and substract
                    DamageManager.ManageDamageFromEnemy(this, gameControl);
                    Destroy(gameControl);
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
            }

            base.Update(gameControl);
        }

        public override void Render(GameControl gameControl)
        {
            Shape.Width = Width / 2;
            Shape.Height = Height / 2;
            Canvas.SetLeft(Shape, X * Width + Width / 2 / 2);
            Canvas.SetTop(Shape, Y * Height + Height / 2 / 2);
        }
    }
}