using TD_WPF.Game.Objects.DynamicGameObjects;

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
                gameControl.GameManager.EndGame(gameControl);
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