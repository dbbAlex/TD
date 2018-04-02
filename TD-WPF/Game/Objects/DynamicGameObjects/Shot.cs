﻿using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Game.Manager;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.Objects.DynamicGameObjects
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

        public override void Start(GameControl gameControl)
        {
            Canvas.SetLeft(Shape, X * Width + Width / 3 / 2);
            Canvas.SetTop(Shape, Y * Height + Height / 3 / 2);
            gameControl.Canvas.Children.Add(Shape);
            Active = true;
        }

        public override void Update(GameControl gameControl)
        {
            if (!Active) return;
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

            base.Update(gameControl);
        }

        public override void Render(GameControl gameControl)
        {
            if (!Active) return;
            Shape.Width = Width / 3;
            Shape.Height = Height / 3;
            Canvas.SetLeft(Shape, X * Width + Width / 3 / 2);
            Canvas.SetTop(Shape, Y * Height + Height / 3 / 2);

            // check for collision TODO: maybe chek collision with other enemies
            var intersectionDetail = Shape.RenderedGeometry.FillContainsWithDetail(Enemy.Shape.RenderedGeometry);
            if (intersectionDetail == IntersectionDetail.Intersects)
            {
                DamageManager.ManageDamageFromShot(this, Enemy, gameControl);
                Destroy(gameControl);
            }
        }
    }
}