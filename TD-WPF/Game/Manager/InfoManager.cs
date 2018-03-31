using System.Dynamic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using TD_WPF.Game.Objects.StaticGameObjects;
using TD_WPF.Game.Utils;

namespace TD_WPF.Game.Manager
{
    public static class InfoManager
    {
        public static void UpdateHealth(GameControl gameControl)
        {
            var label = (Label) gameControl.FindName(ControlUtils.HealthValue);
            if (label != null) label.Content = gameControl.GameCreator.Health;
        }

        public static void UpdateMoney(GameControl gameControl)
        {
            var label = (Label) gameControl.FindName(ControlUtils.MoneyValue);
            if (label != null) label.Content = gameControl.GameCreator.Money;
        }

        public static void UpdateObjectInfoPanelByControl(GameControl gameControl, Control control)
        {
            Label name = (Label) gameControl.FindName(ControlUtils.NameValue);
            Label damage = (Label) gameControl.FindName(ControlUtils.DamageValue);
            Label range = (Label) gameControl.FindName(ControlUtils.RangeValue);
            Label money = (Label) gameControl.FindName(ControlUtils.ObjectMoneyValue);

            Button damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
            Button rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
            Button moneyButton = (Button) gameControl.FindName(ControlUtils.ObjectMoneyButton);
 
            var nameValue = "";
            var damageValue = "";
            var rangeValue = "";
            var moneyValue = "";
            
            switch (control.Name)
            {
                    case "Tower":
                        nameValue = Tower.Name;
                        damageValue = Tower.Damage.ToString();
                        rangeValue = Tower.ShotRange.ToString(CultureInfo.CurrentCulture);
                        moneyValue = Tower.Money.ToString();
                        break;
                    case "ground":
                        nameValue = Ground.Name;
                        moneyValue = Ground.Money.ToString();
                        break;
            }

            if (name != null) name.Content = nameValue;
            if (damage != null) damage.Content = damageValue;
            if (range != null) range.Content = rangeValue;
            if (money != null) money.Content = moneyValue;

            if (damageButton != null) damageButton.Visibility = Visibility.Hidden;
            if (rangeButton != null) rangeButton.Visibility = Visibility.Hidden;
            if (moneyButton != null) moneyButton.Visibility = Visibility.Hidden;
        }
    }
}