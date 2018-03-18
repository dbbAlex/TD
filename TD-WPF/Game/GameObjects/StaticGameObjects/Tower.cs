using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Game.GameObjects.DynamicGameObjects;
using TD_WPF.Game.GameUtils;
using TD_WPF.Game.RoundObjects;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Tower : GameObject
    {
        public Tower(float x, float y, float width, float height, float shotIntervall, float shotSpeed, int shotDamage,
            float range)
            : base(x, y, width, height)
        {
            Range = range;
            ShotIntervall = shotIntervall;
            ShotSpeed = shotSpeed;
            ShotDamage = shotDamage;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.tower),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()));
        }

        public TargetCondition Condition { get; set; } = TargetCondition.Nearest;
        private float ShotIntervall { get; set; }
        private float ShotSpeed { get; set; }
        private int ShotDamage { get; set; }
        private float Range { get; set; }
        private float LastInterval { get; set; }

        private float LastShot { get; set; }
        private List<Shot> Shots { get; set; }

        public void Start(GameControl gameControl, float currentinterval)
        {
            base.Start(gameControl);
            LastInterval = currentinterval;
        }

        public void Update(GameControl gameControl, float currentInterval)
        {
            base.Update(gameControl);
            if (currentInterval - LastInterval >= ShotIntervall)
            {
                Enemy target = NextEnemy(ActiveEnemies((gameControl)));
                if (target == null) return;
                Shot shot = new Shot(X, Y, Width, Height, ShotSpeed, ShotDamage, target);
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
                    foreach (Enemy enemy in enemies)
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
                    foreach (Enemy enemy in enemies)
                    {
                        if (property == 0 || property < enemy.Health)
                        {
                            current = enemy;
                            property = enemy.Health;
                        }
                    } 
                    break;
                case TargetCondition.Strongest:
                    foreach (Enemy enemy in enemies)
                    {
                        if (property == 0 || property < enemy.Damage)
                        {
                            current = enemy;
                            property = enemy.Damage;
                        }
                    }
                    break;
            }

            return current;
        }

        private List<Enemy> ActiveEnemies(GameControl gameControl)
        {
            List<Enemy> enemies = new List<Enemy>();

            Waves waves = gameControl.GameCreator.Waves;
            List<Wave> waveList = waves.WaveList.FindAll(w => w.Active);
            foreach (Wave wave in waveList)
            {
                enemies.AddRange(EnemiesInRange(wave.Enemies.FindAll(e => e.Active)));
            }

            return enemies;
        }

        private List<Enemy> EnemiesInRange(List<Enemy> enemies)
        {
            List<Enemy> inRange = new List<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                if (DistanceToObject(enemy) <= Range) inRange.Add(enemy);
            }

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