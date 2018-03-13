using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game;
using TD_WPF.Game.GameObjects.DynamicGameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;

namespace TD_WPF.Game.GameUtils
{
    public class GameCreator // equivalent zur Spielfeldklasse
    {
        #region attributes
        #region gameObject lists
        #region static objects
        public LinkedList<Path> paths { get; set; } = new LinkedList<Path>(); // base and spawn will be stored in paths too
        public LinkedList<Ground> ground { get; set; } = new LinkedList<Ground>(); // we dont need towers because we will get them by iterationg the ground which referes to the tower 
        #endregion
        #region dynamic objects
        public LinkedList<Enemy> enemies { get; set; } = new LinkedList<Enemy>();
        public LinkedList<Shot> shots { get; set; } = new LinkedList<Shot>();
        #endregion
        #endregion
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
            paths = GameUtils.generateRandomPath((float)gameControl.Canvas.ActualWidth,
               (float)gameControl.Canvas.ActualHeight, this.x, this.y);
            ground = GameUtils.generateRandomGround(paths, (float)gameControl.Canvas.ActualWidth,
               (float)gameControl.Canvas.ActualHeight, this.x, this.y);
        }
    }
}
