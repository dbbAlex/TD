using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_WPF.Game.RoundObjects
{
    public class Waves
    {
        public float intervall { get; set; }
        public List<Wave> waves { get; set; } = new List<Wave>();
        public int waveIndex { get; set; } = 0;
        public float lastDeltaTime { get; set; } = 0;

        public Waves(float intervall)
        {
            this.intervall = intervall;
        }

        public void start(GameControl gameControl)
        {
            waves[waveIndex].start(gameControl);
            waveIndex++;
        }

        public void update(GameControl gameControl, float deltaTime)
        {
            foreach (var item in waves.FindAll(wave => wave.active))
            {
                item.update(gameControl, deltaTime);
            }

            if (waveIndex < waves.Count && (deltaTime - lastDeltaTime >= intervall || lastDeltaTime == 0))
            {
                waves[waveIndex].start(gameControl);
                waveIndex++;
            }
            lastDeltaTime = deltaTime;
        }

        public void render(GameControl gameControl)
        {
            foreach (var item in waves.FindAll(wave => wave.active))
            {
                item.render(gameControl);
            }
        }
    }
}
