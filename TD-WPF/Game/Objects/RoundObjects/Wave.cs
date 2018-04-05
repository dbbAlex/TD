using System.Collections.Generic;
using TD_WPF.Game.Objects.DynamicGameObjects;

namespace TD_WPF.Game.Objects.RoundObjects
{
    public class Wave
    {
        public Wave(long interval)
        {
            Interval = interval;
        }

        public long Interval { get; set; }
        public long LastInterval { get; private set; }
        private long BeforePauseInterval { get; set; }
        public List<Enemy> Enemies { get; } = new List<Enemy>();
        private int EnemyIndex { get; set; }
        public bool Active { get; private set; }
        private bool Pause { get; set; }

        public void Start(GameControl gameControl, long currentInterval)
        {
            Active = true;
            Enemies[EnemyIndex].Start(gameControl);
            EnemyIndex++;
            LastInterval = currentInterval;
        }

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            var findAll = Enemies.FindAll(enemy => enemy.Active);
            foreach (var item in findAll) item.Update(gameControl);

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
            
            if (EnemyIndex < Enemies.Count && currentInterval - LastInterval >= Interval)
            {
                Enemies[EnemyIndex].Start(gameControl);
                EnemyIndex++;
                LastInterval = currentInterval;
            }
            else if (EnemyIndex == Enemies.Count && findAll.Count == 0)
            {
                Active = false;
                LastInterval = currentInterval;
            }
        }

        public void Render(GameControl gameControl)
        {
            foreach (var item in Enemies.FindAll(enemy => enemy.Active)) item.Render(gameControl);
        }
    }
}