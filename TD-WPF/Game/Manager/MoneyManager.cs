using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Game.Objects.StaticGameObjects;

namespace TD_WPF.Game.Manager
{
    public static class MoneyManager
    {
        public static void UpdateTower(Tower tower, TowerUpdateSelection updateSelection, GameControl gameControl)
        {
            // TODO: implement
        }

        public static void BuildTower(Ground ground, string towerName, GameControl gameControl)
        {
            switch (towerName)
            {
                case "Tower":
                    gameControl.GameCreator.Money -= Tower.Money;
                    var tower = new Tower(ground.X, ground.Y, ground.Width, ground.Height, 0.7f, 0.9f);
                    tower.Start(gameControl);
                    ground.Tower = tower;
                    break;
            }

            InfoManager.UpdateMoney(gameControl);

            gameControl.RemoveHintMarks();
            gameControl.CreateHintMarks();
        }

        public static void BuildGround(GameControl gameControl, float x, float y)
        {
            if (Ground.Money > gameControl.GameCreator.Money) return;
            gameControl.GameCreator.Money -= Ground.Money;
            var ground = new Ground(x, y, (float) gameControl.Canvas.ActualWidth / gameControl.GameCreator.X,
                (float) gameControl.Canvas.ActualHeight / gameControl.GameCreator.Y,
                gameControl.GameCreator.Ground.Count);
            ground.Start(gameControl);
            gameControl.GameCreator.Ground.Add(ground);

            InfoManager.UpdateMoney(gameControl);
        }

        public static void EnemyDestroyed(Enemy enemy, GameControl gameControl)
        {
            // TODO: maybe add Money field to enemy
            gameControl.GameCreator.Money += enemy.Money;

            InfoManager.UpdateMoney(gameControl);

            gameControl.RemoveHintMarks();
            gameControl.CreateHintMarks();
        }
    }
}