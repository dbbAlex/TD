using System;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Game.Objects.StaticGameObjects;

namespace TD_WPF.Game.Manager
{
    public static class MoneyManager
    {
        public static void UpdateTower(Tower tower, TowerUpdateSelection updateSelection, GameControl gameControl)
        {
            switch (updateSelection)
            {
                    case TowerUpdateSelection.Damage:
                        if (tower.DamageUpdate < 2 && gameControl.GameCreator.Money >= tower.UpdateSellMoney)
                        {
                            gameControl.GameCreator.Money -= tower.UpdateSellMoney;
                            tower.ShotDamage = Convert.ToInt32(Math.Ceiling(tower.ShotDamage * 1.35));
                            tower.DamageUpdate++;
                            InfoManager.UpdateObjectInfoPanelByGameObject(gameControl, tower);
                        }
                        break;
                    case TowerUpdateSelection.Range:
                        if (tower.RangeUpdate < 2 && gameControl.GameCreator.Money >= tower.UpdateSellMoney)
                        {
                            gameControl.GameCreator.Money -= tower.UpdateSellMoney;
                            tower.Range = Convert.ToSingle(tower.Range * 1.35);
                            tower.RangeUpdate++;
                            InfoManager.UpdateObjectInfoPanelByGameObject(gameControl, tower);
                        }
                        break;
            }
            InfoManager.UpdateMoney(gameControl);
        }

        public static void BuildTower(Ground ground, string towerName, GameControl gameControl)
        {
            if(gameControl.GameManager.Pause) return;
            // TODO: add more Tower
            switch (towerName)
            {
                case "Tower":
                    if (Tower.Money <= gameControl.GameCreator.Money)
                    {
                        gameControl.GameCreator.Money -= Tower.Money;
                        var tower = new Tower(ground.X, ground.Y, ground.Width, ground.Height);
                        tower.Start(gameControl);
                        ground.Tower = tower;
                    }

                    break;
            }

            InfoManager.UpdateMoney(gameControl);

            gameControl.RemoveHintMarks();
            gameControl.CreateHintMarks();
        }

        public static void BuildGround(GameControl gameControl, double x, double y)
        {
            if (Ground.Money > gameControl.GameCreator.Money || gameControl.GameManager.Pause) return;
            gameControl.GameCreator.Money -= Ground.Money;
            var ground = new Ground(x, y, gameControl.Canvas.ActualWidth / gameControl.GameCreator.X,
                gameControl.Canvas.ActualHeight / gameControl.GameCreator.Y,
                gameControl.GameCreator.Ground.Count);
            ground.Start(gameControl);
            gameControl.GameCreator.Ground.Add(ground);

            InfoManager.UpdateMoney(gameControl);
        }

        public static void SellObject(GameControl gameControl, Ground ground)
        {
            if (ground.Tower != null)
            {
                gameControl.GameCreator.Money += ground.Tower.UpdateSellMoney;
                ground.Tower.Destroy(gameControl);
            }
            else
            {
                gameControl.GameCreator.Money += ground.UpdateSellMoney;
                ground.Destroy(gameControl);
            }
            
            InfoManager.UpdateMoney(gameControl);
            InfoManager.UpdateObjectInfoPanelByControl(gameControl, gameControl.SelectedControl);
            gameControl.RemoveHintMarks();
            gameControl.CreateHintMarks();
        }

        public static void EnemyDestroyed(Enemy enemy, GameControl gameControl)
        {
            gameControl.GameCreator.Money += enemy.Money;

            InfoManager.UpdateMoney(gameControl);

            gameControl.RemoveHintMarks();
            gameControl.CreateHintMarks();
        }
    }
}