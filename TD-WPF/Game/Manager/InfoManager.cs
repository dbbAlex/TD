using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.Objects;
using TD_WPF.Game.Objects.StaticGameObjects;
using TD_WPF.Game.Objects.StaticGameObjects.TowerExtensions;
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
            var damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
            var rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
            var moneyButton = (Button) gameControl.FindName(ControlUtils.ObjectMoneyButton);

            if (damageButton != null) damageButton.Visibility = Visibility.Hidden;
            if (rangeButton != null) rangeButton.Visibility = Visibility.Hidden;
            if (moneyButton != null) moneyButton.Visibility = Visibility.Hidden;
            
            if (control == null) return;
            if (control.Name == "Ground")
                UpdateObjectinfoPanelByType(gameControl, typeof(Ground));
            else
                switch (control.Name)
                {
                    case "Tower":
                        UpdateObjectinfoPanelByType(gameControl, typeof(Tower));
                        break;
                    case "Sniper":
                        UpdateObjectinfoPanelByType(gameControl, typeof(Sniper));
                        break;
                    case "Rapid":
                        UpdateObjectinfoPanelByType(gameControl, typeof(Rapid));
                        break;
                }
        }

        public static void UpdateObjectInfoPanelByGameObject(GameControl gameControl, GameObject gameObject)
        {
            UpdateObjectinfoPanelByType(gameControl, gameObject.GetType());
            if (gameObject is Tower tower)
            {
                UpdateObjectInfoPanleNonConstValue(gameControl,
                    Math.Round((double) tower.FireDamage, 2).ToString(CultureInfo.InvariantCulture),
                    Math.Round(tower.FireRange, 2).ToString(CultureInfo.InvariantCulture));

                var damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
                var rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
                var comboBox = (ComboBox) gameControl.FindName(ControlUtils.TargetValue);
                if (comboBox != null) comboBox.SelectedItem = tower.Condition;
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

            if (moneyButton == null) return;
            switch (gameObject)
            {
                case Tower t:
                    moneyButton.Content = "sell (" + t.UpdateSellMoney + "฿)";
                    break;
                case Ground ground:
                    moneyButton.Content = "sell (" + Ground.UpdateSellMoney + "฿)";
                    break;
            }

            moneyButton.Visibility = Visibility.Visible;
        }

        private static void UpdateObjectinfoPanelByType(FrameworkElement gameControl, Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                            BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            var nameValue = fieldInfos.Exists(f => f.Name.Equals("Name"))
                ? (string) fieldInfos.First(f => f.Name.Equals("Name")).GetRawConstantValue()
                : "";
            var damageValue = fieldInfos.Exists(f => f.Name.Equals("Damage"))
                ? Math.Round(Convert.ToDouble(fieldInfos.First(f => f.Name.Equals("Damage")).GetRawConstantValue()), 2)
                    .ToString(CultureInfo.InvariantCulture)
                : "";
            var rangeValue = fieldInfos.Exists(f => f.Name.Equals("Range"))
                ? Math.Round(Convert.ToDouble(fieldInfos.First(f => f.Name.Equals("Range")).GetRawConstantValue()),
                        2)
                    .ToString(CultureInfo.InvariantCulture)
                : "";
            var moneyValue = fieldInfos.Exists(f => f.Name.Equals("Money"))
                ? fieldInfos.First(f => f.Name.Equals("Money")).GetRawConstantValue().ToString()
                : "";

            UpdateObjectInfoPanel(gameControl, nameValue, damageValue, rangeValue, moneyValue);
        }

        private static void UpdateObjectInfoPanel(FrameworkElement gameControl, string nameValue, string damageValue,
            string rangeValue, string moneyValue)
        {
            var name = (Label) gameControl.FindName(ControlUtils.NameValue);
            var money = (Label) gameControl.FindName(ControlUtils.ObjectMoneyValue);

            if (name != null) name.Content = nameValue;
            if (money != null) money.Content = moneyValue;

            UpdateObjectInfoPanleNonConstValue(gameControl, damageValue, rangeValue);
        }

        private static void UpdateObjectInfoPanleNonConstValue(FrameworkElement gameControl, string damageValue,
            string rangeValue)
        {
            var damage = (Label) gameControl.FindName(ControlUtils.DamageValue);
            var range = (Label) gameControl.FindName(ControlUtils.RangeValue);

            if (damage != null) damage.Content = damageValue;
            if (range != null) range.Content = rangeValue;
        }
    }
}