using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game;
using TD_WPF.Game.GameObjects.DynamicGameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;
using TD_WPF.Game.RoundObjects;

namespace TD_WPF.Game.GameUtils
{
    public class GameCreator // equivalent zur Spielfeldklasse
    {
        #region attributes
        public LinkedList<Path> paths { get; set; } = new LinkedList<Path>(); // base and spawn will be stored in paths too
        public LinkedList<Ground> ground { get; set; } = new LinkedList<Ground>(); // we dont need towers because we will get them by iterationg the ground which referes to the tower         
        public Waves waves { get; set; } = null;
        public GameControl gameControl { get; set; }
        public int x { get; set; } = 20;
        public int y { get; set; } = 15;
        #endregion

        public GameCreator(GameControl gameControl)
        {
            this.gameControl = gameControl;
        }

        public void initilizeRandomPath()
        {
            paths = GameUtils.GenerateRandomPath((float)gameControl.Canvas.ActualWidth,
               (float)gameControl.Canvas.ActualHeight, this.x, this.y);
            ground = GameUtils.GenerateRandomGround(paths, (float)gameControl.Canvas.ActualWidth,
               (float)gameControl.Canvas.ActualHeight, this.x, this.y);
        }

        public void initializeRandomWaves()
        {
            waves = GameUtils.GenerateRandomWaves(5F, 2F, (Spawn)paths.First.Value);
        }


    }
}
