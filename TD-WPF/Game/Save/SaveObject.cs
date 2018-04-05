using System.Collections.Generic;
using TD_WPF.Game.Objects.RoundObjects;
using TD_WPF.Game.Objects.StaticGameObjects;

namespace TD_WPF.Game.Save
{
    public class SaveObject
    {
        public string Identifier { get; set; } 
        public List<Path> Paths { get; set; }
        public List<Ground> Ground { get; set; }
        public Waves Waves { get; set; }
        public int Health { get; set; }
        public int Money { get; set; }
    }
}