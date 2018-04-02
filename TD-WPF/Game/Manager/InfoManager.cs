using System;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using TD_WPF.Game.Objects;
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
            switch (control.Name)
            {
                case "Tower":
                    UpdateObjectinfoPanelByType(gameControl, typeof(Tower));
                    break;
                case "ground":
                    UpdateObjectinfoPanelByType(gameControl, typeof(Ground));
                    break;
            }

            Button damageButton = (Button) gameControl.FindName(ControlUtils.DamageButton);
            Button rangeButton = (Button) gameControl.FindName(ControlUtils.RangeButton);
            Button moneyButton = (Button) gameControl.FindName(ControlUtils.ObjectMoneyButton);

            if (damageButton != null) damageButton.Visibility = Visibility.Hidden;
            if (rangeButton != null) rangeButton.Visibility = Visibility.Hidden;
            if (moneyButton != null) moneyButton.Visibility = Visibility.Hidden;
        }

        public static void UpdateObjectInfoPanelByGameObject(GameControl gameControl, GameObject gameObject)
        {
            UpdateObjectinfoPanelByType(gameControl, gameObject.GetType());
            
            // TODO: enabel update buttons
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
                ? (string) fieldInfos.First(f => f.Name.Equals("Damage")).GetRawConstantValue().ToString()
                : "";
            var rangeValue = fieldInfos.Exists(f => f.Name.Equals("ShotRange"))
                ? (string) fieldInfos.First(f => f.Name.Equals("ShotRange")).GetRawConstantValue().ToString()
                : "";
            var moneyValue = fieldInfos.Exists(f => f.Name.Equals("Money"))
                ? (string) fieldInfos.First(f => f.Name.Equals("Money")).GetRawConstantValue().ToString()
                : "";

            Label name = (Label) gameControl.FindName(ControlUtils.NameValue);
            Label damage = (Label) gameControl.FindName(ControlUtils.DamageValue);
            Label range = (Label) gameControl.FindName(ControlUtils.RangeValue);
            Label money = (Label) gameControl.FindName(ControlUtils.ObjectMoneyValue);

            if (name != null) name.Content = nameValue;
            if (damage != null) damage.Content = damageValue;
            if (range != null) range.Content = rangeValue;
            if (money != null) money.Content = moneyValue;
        }
    }
}