using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game.GameObjects.DynamicGameObjects;

namespace TD_WPF.Game.RoundObjects
{
    public class Wave
    {
        public float intervall { get; set; }
        public List<Enemy> enemies { get; set; } = new List<Enemy>();
        public int enemyIndex { get; set; } = 0;
        public float lastDeltaTime = 0;
        public bool active { get; set; } = false;

        public Wave(float intervall)
        {
            this.intervall = intervall;
        }

        public void start(GameControl gameControl)
        {
            active = true;
            enemies[enemyIndex].start(gameControl);
            enemyIndex++;
        }

        public void update(GameControl gameControl, float deltaTime)
        {
            foreach (var item in enemies.FindAll(enemy => enemy.active))
            {
                item.update(gameControl, deltaTime);
            }

            if(enemyIndex < enemies.Count && (deltaTime - lastDeltaTime >= intervall || lastDeltaTime == 0))
            {
                enemies[enemyIndex].start(gameControl);
                enemyIndex++;
            }
            lastDeltaTime = deltaTime;
        }

        public void render(GameControl gameControl)
        {
            foreach (var item in enemies.FindAll(enemy => enemy.active))
            {
                item.render(gameControl);
            }
        }
    }
}
