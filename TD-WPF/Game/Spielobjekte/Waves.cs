using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_WPF.Game.Spielobjekte
{
    public class Waves
    {
        public LinkedList<Wave> waves { get; set; } = new LinkedList<Wave>();
        public LinkedList<Wave> spawned { get; set; } = new LinkedList<Wave>();
        public double intervalInMilli { get; set; } = 10000; 

        public Waves(LinkedList<Wave> waves, double intervallInMilli)
        {
            this.waves = waves;
            this.intervalInMilli = intervalInMilli;
        }

        public Waves()
        {

        }
    }
}
