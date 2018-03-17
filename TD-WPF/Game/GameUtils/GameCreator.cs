using System.Collections.Generic;
using TD_WPF.Game.GameObjects.StaticGameObjects;
using TD_WPF.Game.RoundObjects;

namespace TD_WPF.Game.GameUtils
{
    public class GameCreator // equivalent zur Spielfeldklasse
    {
        public GameCreator(GameControl gameControl)
        {
            this.GameControl = gameControl;
        }

        public void InitilizeRandomPath()
        {
            Paths = GameUtils.GenerateRandomPath((float) GameControl.Canvas.ActualWidth,
                (float) GameControl.Canvas.ActualHeight, X, Y);
            Ground = GameUtils.GenerateRandomGround(Paths, (float) GameControl.Canvas.ActualWidth,
                (float) GameControl.Canvas.ActualHeight, X, Y);
        }

        public void InitializeRandomWaves()
        {
            Waves = GameUtils.GenerateRandomWaves(10f, 5f, (Spawn) Paths[0]);
        }

        #region attributes
        // base and spawn will be stored in paths too
        public List<Path> Paths { get; private set; } = new List<Path>(); 
        // we dont need towers because we will get them by iterationg the ground which referes to the tower
        public List<Ground> Ground { get; private set; } = new List<Ground>();          

        public Waves Waves { get; private set; }
        private GameControl GameControl { get; set; }
        public int X { get; set; } = 20;
        public int Y { get; set; } = 15;

        #endregion
    }
}