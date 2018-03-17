using System.Collections.Generic;

namespace TD_WPF.Game.RoundObjects
{
    public class Waves
    {
        public Waves(float intervall)
        {
            Intervall = 1000*intervall;
        }

        private float Intervall { get; }
        public List<Wave> WaveList { get; } = new List<Wave>();
        private int WaveIndex { get; set; }
        private bool Active { get; set; }

        public void Start(GameControl gameControl, float currentInterval)
        {
            WaveList[WaveIndex].Start(gameControl, currentInterval);
            WaveIndex++;
            Active = true;
        }

        public void Update(GameControl gameControl, float currentInterval)
        {
            if (!Active) return;
            Wave wave = WaveList[WaveIndex-1];
            if (wave.Active) wave.Update(gameControl, currentInterval);
            else if(WaveIndex < WaveList.Count && currentInterval - wave.LastInterval >= Intervall)
            {
                WaveList[WaveIndex].Start(gameControl, currentInterval);
                WaveIndex++;
            }
            else if (WaveIndex == WaveList.Count && !wave.Active)
            {
                Active = false;
            }
        }

        public void Render(GameControl gameControl)
        {
            foreach (var item in WaveList.FindAll(wave => wave.Active)) item.Render(gameControl);
        }
    }
}