using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.Objects;
using TD_WPF.Game.Objects.StaticGameObjects;
using TD_WPF.Game.Utils;

namespace TD_WPF.Game.Manager
{
    public static class InfoManager
    {
        public static void UpdateHealth(GameControl gameControl)
        {
            var control = gameControl.FindName(ControlUtils.HealthValue);
            if (control != null && control is Label label)
                label.Content = gameControl.GameCreator.Health;
            else if (control != null && control is TextBox textBox)
                textBox.Text = gameControl.GameCreator.Health.ToString();
        }

        public static void UpdateMoney(GameControl gameControl)
        {
            var control = gameControl.FindName(ControlUtils.MoneyValue);
            if (control != null && control is Label label)
                label.Content = gameControl.GameCreator.Money + " (฿)";
            else if (control != null && control is TextBox textBox)
                textBox.Text = gameControl.GameCreator.Money.ToString();
        }

        public static void UpdateObjectInfoPanelByControl(GameControl gameControl, Control control)
        {
            switch (control.Name)
            {
                case "Tower":
                    UpdateObjectinfoPanelByType(gameControl, typeof(Tower));
                    break;
                case "Ground":
                    UpdateObjectinfoPanelByType(gameControl, typeof(Ground));
                    break;
            }

            var damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
            var rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
            var moneyButton = (Button) gameControl.FindName(ControlUtils.ObjectMoneyButton);

            if (damageButton != null) damageButton.Visibility = Visibility.Hidden;
            if (rangeButton != null) rangeButton.Visibility = Visibility.Hidden;
            if (moneyButton != null) moneyButton.Visibility = Visibility.Hidden;
        }

        public static void UpdateObjectInfoPanelByGameObject(GameControl gameControl, GameObject gameObject)
        {
            UpdateObjectinfoPanelByType(gameControl, gameObject.GetType());
            if (gameObject is Tower tower)
            {
                UpdateObjectInfoPanleNonConstValue(gameControl, tower.ShotDamage.ToString(),
                    tower.Range.ToString(CultureInfo.InvariantCulture));

                var damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
                var rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
                if (damageButton != null && tower.DamageUpdate < 2)
                {
                    damageButton.Content = "+35% (" + tower.UpdateSellMoney + "฿)";
                    damageButton.Visibility = Visibility.Visible;
                }
                else if (damageButton != null)
                {
                    damageButton.Visibility = Visibility.Hidden;
                }

                if (rangeButton != null && tower.RangeUpdate < 2)
                {
                    rangeButton.Content = "+35% (" + tower.UpdateSellMoney + "฿)";
                    rangeButton.Visibility = Visibility.Visible;
                }
                else if (rangeButton != null)
                {
                    rangeButton.Visibility = Visibility.Hidden;
                }
            }

            var moneyButton = (Button) gameControl.FindName(ControlUtils.ObjectMoneyButton);

            if (moneyButton != null)
            {
                if (gameObject is Tower t)
                    moneyButton.Content = "sell (" + t.UpdateSellMoney + "฿)";
                if (gameObject is Ground ground)
                    moneyButton.Content = "sell (" + ground.UpdateSellMoney + "฿)";

                moneyButton.Visibility = Visibility.Visible;
            }
        }

        private static void UpdateObjectinfoPanelByType(GameControl gameControl, Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                            BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            var nameValue = fieldInfos.Exists(f => f.Name.Equals("Name"))
                ? (string) fieldInfos.First(f => f.Name.Equals("Name")).GetRawConstantValue()
                : "";
            var damageValue = fieldInfos.Exists(f => f.Name.Equals("Damage"))
                ? fieldInfos.First(f => f.Name.Equals("Damage")).GetRawConstantValue().ToString()
                : "";
            var rangeValue = fieldInfos.Exists(f => f.Name.Equals("ShotRange"))
                ? fieldInfos.First(f => f.Name.Equals("ShotRange")).GetRawConstantValue().ToString()
                : "";
            var moneyValue = fieldInfos.Exists(f => f.Name.Equals("Money"))
                ? fieldInfos.First(f => f.Name.Equals("Money")).GetRawConstantValue().ToString()
                : "";

            UpdateObjectInfoPanel(gameControl, nameValue, damageValue, rangeValue, moneyValue);
        }

        private static void UpdateObjectInfoPanel(GameControl gameControl, string nameValue, string damageValue,
            string rangeValue, string moneyValue)
        {
            var name = (Label) gameControl.FindName(ControlUtils.NameValue);
            var money = (Label) gameControl.FindName(ControlUtils.ObjectMoneyValue);

            if (name != null) name.Content = nameValue;
            if (money != null) money.Content = moneyValue;

            UpdateObjectInfoPanleNonConstValue(gameControl, damageValue, rangeValue);
        }

        private static void UpdateObjectInfoPanleNonConstValue(GameControl gameControl, string damageValue,
            string rangeValue)
        {
            var damage = (Label) gameControl.FindName(ControlUtils.DamageValue);
            var range = (Label) gameControl.FindName(ControlUtils.RangeValue);

            if (damage != null) damage.Content = damageValue;
            if (range != null) range.Content = rangeValue;
        }
    }
}