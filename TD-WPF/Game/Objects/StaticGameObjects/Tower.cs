using System;
using System.Collections.Generic;
using System.Linq;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Tower : GameObject
    {
        public const string Name = "Tower";
        public const int Money = 10;
        private const int Damage = 5;
        private const double ShotRange = 2;

        public Tower(double x, double y, double width, double height,
            TargetCondition targetCondition = TargetCondition.Closest)
            : base(x, y, width, height)
        {
            Condition = targetCondition;
            Image = Resource.tower;
        }


        public TargetCondition Condition { get; set; }
        public int ShotDamage { get; set; } = Damage;
        public double Range { get; set; } = ShotRange;
        public int DamageUpdate { get; set; }
        public int RangeUpdate { get; set; }
        public static int UpdateSellMoney => Convert.ToInt32(Math.Ceiling(Money / 2d));

        private double ShotIntervall { get; } = 1000 * 0.7;
        private double ShotSpeed { get; } = 0.9;
        private long LastInterval { get; set; }
        private long BeforePauseInterval { get; set; }
        private bool Pause { get; set; }

        public void Start(GameControl gameControl, long currentinterval)
        {
            if (Active) return;
            base.Start(gameControl);
            LastInterval = currentinterval;
        }

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            base.Update(gameControl);

            if (gameControl.GameManager.Pause)
            {
                if (Pause) return;
                Pause = true;
                BeforePauseInterval = currentInterval - LastInterval;
                return;
            }

            if (Pause)
            {
                LastInterval = currentInterval - BeforePauseInterval;
                Pause = false;
            }

            if (!(currentInterval - LastInterval >= ShotIntervall)) return;
            var target = NextEnemy(ActiveEnemies(gameControl));
            if (target == null) return;
            var shot = new Shot(X, Y, Width, Height, ShotSpeed, ShotDamage, target);
            shot.Start(gameControl);
            gameControl.Shots.Add(shot);
            LastInterval = currentInterval;
        }

        public override void Destroy(GameControl gameControl)
        {
            base.Destroy(gameControl);
            gameControl.GameCreator.Ground.Find(g => g.Tower == this).Tower = null;
        }

        private Enemy NextEnemy(IEnumerable<Enemy> enemies)
        {
            Enemy current = null;
            var property = 0d;

            foreach (var enemy in enemies)
            {
                var distance = DistanceToObject(enemy);
                switch (Condition)
                {
                    case TargetCondition.Closest:
                        
                        if (property != 0 && !(property > distance)) continue;
                        current = enemy;
                        property = distance;
                        break;
                    case TargetCondition.Farthest:
                        if (property != 0 && !(property < distance)) continue;
                        current = enemy;
                        property = distance;
                        break;
                    case TargetCondition.Strongest:
                        if (property == 0 || property < enemy.Damage)
                        {
                            current = enemy;
                            property = enemy.Damage;
                        }
                        break;
                    case TargetCondition.Weakest:
                        if (property == 0 || property > enemy.Damage)
                        {
                            current = enemy;
                            property = enemy.Damage;
                        }
                        break;
                    case TargetCondition.Healthiest:
                        if (property == 0 || property < enemy.Health)
                        {
                            current = enemy;
                            property = enemy.Health;
                        }
                        break;
                    case TargetCondition.Unhealthiest:
                        if (property == 0 || property > enemy.Health)
                        {
                            current = enemy;
                            property = enemy.Health;
                        }
                        break;
                    case TargetCondition.Fastest:
                        if (property == 0 || property < enemy.Speed)
                        {
                            current = enemy;
                            property = enemy.Speed;
                        }
                        break;
                    case TargetCondition.Slowest:
                        if (property == 0 || property > enemy.Speed)
                        {
                            current = enemy;
                            property = enemy.Speed;
                        }
                        break;
                }
            }

            return current;
        }

        private IEnumerable<Enemy> ActiveEnemies(GameControl gameControl)
        {
            var enemies = new List<Enemy>();

            var waves = gameControl.GameCreator.Waves;
            var waveList = waves.WaveList.FindAll(w => w.Active);
            foreach (var wave in waveList) enemies.AddRange(EnemiesInRange(wave.Enemies.FindAll(e => e.Active)));

            return enemies;
        }

        private IEnumerable<Enemy> EnemiesInRange(IEnumerable<Enemy> enemies)
        {
            return enemies.Where(enemy => DistanceToObject(enemy) <= Range * Width).ToList();
        }

        private double DistanceToObject(GameObject gameObject)
        {
            // our center
            var x = X * Width + Width / 2;
            var y = Y * Height + Height / 2;

            // their center
            var x2 = gameObject.X * gameObject.Width + gameObject.Width / 2;
            var y2 = gameObject.Y * gameObject.Height + gameObject.Height / 2;

            return Math.Abs(Math.Sqrt(Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2)));
        }
    }
}