using System.Collections.Generic;

namespace TD_WPF.Game.RoundObjects
{
    public class Waves
    {
        public Waves(float intervall)
        {
            this.Intervall = intervall;
        }

        private float Intervall { get; set; }
        public List<Wave> WaveList { get; set; } = new List<Wave>();
        private int WaveIndex { get; set; }
        private float LastInterval { get; set; }

        public void Start(GameControl gameControl, float currentInterval)
        {
            WaveList[WaveIndex].Start(gameControl, currentInterval);
            WaveIndex++;
            LastInterval = currentInterval;
        }

        public void Update(GameControl gameControl, float currentInterval)
        {
            var findAll = WaveList.FindAll(wave => wave.Active);
            foreach (var item in findAll) item.Update(gameControl, currentInterval);

            if (WaveIndex < WaveList.Count && currentInterval - LastInterval >= Intervall)
            {
                WaveList[WaveIndex].Start(gameControl, currentInterval);
                WaveIndex++;
                LastInterval = currentInterval;
            }
        }

        public void Render(GameControl gameControl)
        {
            foreach (var item in WaveList.FindAll(wave => wave.Active)) item.Render(gameControl);
        }
    }
}