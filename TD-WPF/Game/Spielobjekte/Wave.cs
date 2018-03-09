using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game.Spielobjekte.Items;

namespace TD_WPF.Game.Spielobjekte
{
    public class Wave
    {
        public LinkedList<Gegner> enemy { get; set; } = new LinkedList<Gegner>();
        public LinkedList<Gegner> spawned { get; set; } = new LinkedList<Gegner>();
        public double intervalInMilli { get; set; } = 2000;

        public Wave(LinkedList<Gegner> enemy, double intervalInMilli)
        {
            this.enemy = enemy;
            this.intervalInMilli = intervalInMilli;
        }

        public Wave()
        {

        }
    }
}
