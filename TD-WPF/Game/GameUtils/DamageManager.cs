using System.Windows.Controls;
using TD_WPF.Game.GameObjects.DynamicGameObjects;

namespace TD_WPF.Game.GameUtils
{
    public class DamageManager
    {
        public static void ManageDamageFromEnemy(Enemy enemy, GameControl gameControl)
        {
            Label label = (Label) gameControl.FindName(ControlUtils.HealthValue);
            
            gameControl.GameCreator.Health -= enemy.Damage;
            
            if (gameControl.GameCreator.Health <= 0)
            {
                gameControl.GameCreator.Health = 0;
                gameControl.GameManager.EndGame(gameControl);
            }
            
            if (label != null) label.Content = gameControl.GameCreator.Health;
        }

        public static void ManageDamageFromShot(Shot shot, Enemy enemy, GameControl gameControl)
        {
            enemy.Health -= shot.Damage;

            if (enemy.Health <= 0) enemy.Destroy(gameControl);
        }
    }
}