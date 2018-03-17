using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TD_WPF.Game.GameObjects.DynamicGameObjects;

namespace TD_WPF.Game.RoundObjects
{
    public class Wave
    {
        public Wave(float interval)
        {
            this.Interval = 1000*interval;
        }

        private float Interval { get; set; }
        private float LastInterval { get; set; }
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        private int EnemyIndex { get; set; } = 0;
        public bool Active { get; private set; }

        public void Start(GameControl gameControl, float currentInterval)
        {
            Active = true;
            Enemies[EnemyIndex].Start(gameControl);
            EnemyIndex++;
            LastInterval = currentInterval;
        }

        public void Update(GameControl gameControl, float currentInterval)
        {
            var findAll = Enemies.FindAll(enemy => enemy.Active);
            foreach (var item in findAll) item.Update(gameControl);

            if (EnemyIndex < Enemies.Count && currentInterval - LastInterval >= Interval)
            {
                Enemies[EnemyIndex].Start(gameControl);
                EnemyIndex++;
            }else if (EnemyIndex > Enemies.Count() && findAll.Count > 0)
            {
                Active = false;
            }
        }

        public void Render(GameControl gameControl)
        {
            foreach (var item in Enemies.FindAll(enemy => enemy.Active)) item.Render(gameControl);
        }
    }
}