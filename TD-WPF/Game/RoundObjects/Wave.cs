﻿using System.Collections.Generic;
using TD_WPF.Game.GameObjects.DynamicGameObjects;

namespace TD_WPF.Game.RoundObjects
{
    public class Wave
    {
        public float lastDeltaTime;

        public Wave(float intervall)
        {
            this.intervall = intervall;
        }

        public float intervall { get; set; }
        public List<Enemy> enemies { get; set; } = new List<Enemy>();
        public int enemyIndex { get; set; }
        public bool active { get; set; }

        public void start(GameControl gameControl)
        {
            active = true;
            enemies[enemyIndex].start(gameControl);
            enemyIndex++;
        }

        public void update(GameControl gameControl, float deltaTime)
        {
            foreach (var item in enemies.FindAll(enemy => enemy.active)) item.update(gameControl, deltaTime);

            if (enemyIndex < enemies.Count && (deltaTime - lastDeltaTime >= intervall || lastDeltaTime == 0))
            {
                enemies[enemyIndex].start(gameControl);
                enemyIndex++;
            }

            lastDeltaTime = deltaTime;
        }

        public void render(GameControl gameControl)
        {
            foreach (var item in enemies.FindAll(enemy => enemy.active)) item.render(gameControl);
        }
    }
}