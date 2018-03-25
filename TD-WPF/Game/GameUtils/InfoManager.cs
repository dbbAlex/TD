using System.Windows.Controls;

namespace TD_WPF.Game.GameUtils
{
    public static class InfoManager
    {
        public static void UpdateHealth(GameControl gameControl)
        {
            Label label = (Label) gameControl.FindName(ControlUtils.HealthValue);
            if(label != null) label.Content = gameControl.GameCreator.Health;
        }
        
        public static void UpdateMoney(GameControl gameControl)
        {
            Label label = (Label) gameControl.FindName(ControlUtils.MoneyValue);
            if(label != null) label.Content = gameControl.GameCreator.Money;
        }
    }
}