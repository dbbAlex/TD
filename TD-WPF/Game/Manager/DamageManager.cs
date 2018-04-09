using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Menu;
using TD_WPF.Menu.Dialog;

namespace TD_WPF.Game.Manager
{
    public static class DamageManager
    {
        public static void ManageDamageFromEnemy(Enemy enemy, GameControl gameControl)
        {
            gameControl.GameCreator.Health -= enemy.Damage;

            if (gameControl.GameCreator.Health <= 0)
            {
                gameControl.GameCreator.Health = 0;
                var dialog = new Dialog(Window.GetWindow(gameControl), DialogType.GameOver);
                dialog.ShowDialog();
                switch (gameControl.GameControlMode)
                {
                    case GameControlMode.CreateMap:
                        ((ContentControl) gameControl.Parent).Content = new EditorMenu();
                        break;
                    case GameControlMode.PlayRandom:
                        ((ContentControl) gameControl.Parent).Content = new GameMenu();
                        break;
                    case GameControlMode.EditMap:
                    case GameControlMode.PlayMap:
                        ((ContentControl) gameControl.Parent).Content = new MapMenu(gameControl.GameControlMode);
                        break;
                }

                gameControl.GameManager.EndLoop();
            }

            InfoManager.UpdateHealth(gameControl);
        }

        public static void ManageDamageFromShot(Shot shot, Enemy enemy, GameControl gameControl)
        {
            enemy.Health -= shot.Damage;

            if (enemy.Health > 0) return;
            MoneyManager.EnemyDestroyed(enemy, gameControl);
            enemy.Destroy(gameControl);
        }
    }
}