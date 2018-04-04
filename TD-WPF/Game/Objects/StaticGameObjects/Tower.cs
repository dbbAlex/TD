using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public Tower(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            Image = ImageTool.ResizeImage(new Bitmap(Resource.tower),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()));
        }
        

        public TargetCondition Condition { get; set; } = TargetCondition.Nearest;
        public int ShotDamage { get; set; } = Damage;
        public float Range { get; set; } = ShotRange;
        public int DamageUpdate { get; set; } = 0;
        public int RangeUpdate { get; set; } = 0;
        public int UpdateSellMoney => Convert.ToInt32(Math.Ceiling((double) (Money / 2)));

        private float ShotIntervall { get; } = 1000 * 0.7f;
        private float ShotSpeed { get; } = 0.9f;
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

        public override void Destroy(GameControl gameControl)
        {
            base.Destroy(gameControl);
            gameControl.GameCreator.Ground.Find(g => g.Tower == this).Tower = null;
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
                        if (property != 0 && !(property > distance)) continue;
                        current = enemy;
                        property = distance;
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