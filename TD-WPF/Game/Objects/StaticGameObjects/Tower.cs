﻿using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Tower : GameObject
    {
        public const string Name = "Tower";
        public const int Money = 10;
        public const int Damage = 5;
        public const double ShotRange = 2;

        public Tower(double x, double y, double width, double height)
            : base(x, y, width, height)
        {
            Image = Resource.tower;
        }


        public TargetCondition Condition { get; set; } = TargetCondition.Nearest;
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
            base.Start(gameControl);
            LastInterval = currentinterval;
        }

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            base.Update(gameControl);

            if (gameControl.GameManager.Pause)
            {
                if (!Pause)
                {
                    Pause = true;
                    BeforePauseInterval = currentInterval - LastInterval;
                }

                return;
            }

            if (Pause)
            {
                LastInterval = currentInterval - BeforePauseInterval;
                Pause = false;
            }

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
            var property = 0d;
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