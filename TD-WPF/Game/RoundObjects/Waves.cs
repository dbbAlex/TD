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
        public List<Wave> waves { get; set; }
        public int waveIndex { get; set; }

        public Waves(float intervall, List<Wave> waves)
        {
            this.intervall = intervall;
            this.waves = waves;
        }
    }
}
