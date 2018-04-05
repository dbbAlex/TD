using System.Collections.Generic;

namespace TD_WPF.Game.Objects.RoundObjects
{
    public class Waves
    {
        public Waves(long interval)
        {
            Interval = interval;
        }

        public long Interval { get; set; }
        public List<Wave> WaveList { get; } = new List<Wave>();
        private int WaveIndex { get; set; }
        private bool Active { get; set; }

        public void Start(GameControl gameControl, long currentInterval)
        {
            WaveList[WaveIndex].Start(gameControl, currentInterval);
            WaveIndex++;
            Active = true;
        }

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            var wave = WaveList[WaveIndex - 1];
            if (wave.Active)
            {
                wave.Update(gameControl, currentInterval);
            }
            else if (WaveIndex < WaveList.Count && currentInterval - wave.LastInterval >= Interval)
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

        public void Deaktivate()
        {
            Active = false;
        }
    }
}