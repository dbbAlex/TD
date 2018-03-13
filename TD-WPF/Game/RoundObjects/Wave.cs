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
        public List<Enemy> enemies { get; set; }
        public int enemyIndex { get; set; } = 0;

        public Wave(float intervall, List<Enemy> enemies)
        {
            this.intervall = intervall;
            this.enemies = enemies;
        }
    }
}
