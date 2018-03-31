using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Tower : GameObject
    {
        public const string Name = "Tower";
        public const int Money = 10;
        public const int Damage = 5;
        public const float ShotRange = 2;

        public Tower(float x, float y, float width, float height, float shotIntervall, float shotSpeed)
            : base(x, y, width, height)
        {
            ShotIntervall = 1000 * shotIntervall;
            ShotSpeed = shotSpeed;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.tower),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()));
        }
        

        public TargetCondition Condition { get; set; } = TargetCondition.Nearest;
        private int ShotDamage { get; } = Damage;
        private float Range { get; } = ShotRange;
        
        private float ShotIntervall { get; }
        private float ShotSpeed { get; }
        private float LastInterval { get; set; }

        public void Start(GameControl gameControl, float currentinterval)
        {
            base.Start(gameControl);
            LastInterval = currentinterval;
        }

        public void Update(GameControl gameControl, float currentInterval)
        {
            if (!Active) return;
            base.Update(gameControl);
            if (currentInterval - LastInterval >= ShotIntervall)
            {
                var target = NextEnemy(ActiveEnemies(gameControl));
                if (target == null) return;
                var shot = new Shot(X, Y, Width, Height, ShotSpeed, ShotDamage, target);
                shot.Start(gameControl);
                gameControl.Shots.Add(shot);
                LastInterval = currentInterval;
            }
        }

        private Enemy NextEnemy(List<Enemy> enemies)
        {
            Enemy current = null;
            var property = 0f;
            switch (Condition)
            {
                case TargetCondition.Nearest:
                    foreach (var enemy in enemies)
                    {
                        var distance = DistanceToObject(enemy);
                        if (property == 0 || property > distance)
                        {
                            current = enemy;
                            property = distance;
                        }
                    }

                    break;
                case TargetCondition.Healthiest:
                    foreach (var enemy in enemies)
                        if (property == 0 || property < enemy.Health)
                        {
                            current = enemy;
                            property = enemy.Health;
                        }

                    break;
                case TargetCondition.Strongest:
                    foreach (var enemy in enemies)
                        if (property == 0 || property < enemy.Damage)
                        {
                            current = enemy;
                            property = enemy.Damage;
                        }

                    break;
            }

            return current;
        }

        private List<Enemy> ActiveEnemies(GameControl gameControl)
        {
            var enemies = new List<Enemy>();

            var waves = gameControl.GameCreator.Waves;
            var waveList = waves.WaveList.FindAll(w => w.Active);
            foreach (var wave in waveList) enemies.AddRange(EnemiesInRange(wave.Enemies.FindAll(e => e.Active)));

            return enemies;
        }

        private List<Enemy> EnemiesInRange(List<Enemy> enemies)
        {
            var inRange = new List<Enemy>();
            foreach (var enemy in enemies)
                if (DistanceToObject(enemy) <= Range * Width)
                    inRange.Add(enemy);

            return inRange;
        }

        private float DistanceToObject(GameObject gameObject)
        {
            // our center
            var x = X * Width + Width / 2;
            var y = Y * Height + Height / 2;

            // their center
            var x2 = gameObject.X * gameObject.Width + gameObject.Width / 2;
            var y2 = gameObject.Y * gameObject.Height + gameObject.Height / 2;

            return (float) Math.Abs(Math.Sqrt(Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2)));
        }
    }
}