﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Game.Enumerations;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using Label = System.Windows.Controls.Label;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;

namespace TD_WPF.Game.Utils
{
    internal static class ControlUtils
    {
        #region constants

        private const string InfoPanel = "InfoPanel";
        private const string Health = "Health";
        public const string HealthValue = "HealthValue";
        private const string Money = "Money";
        public const string MoneyValue = "MoneyValue";
        private const string ObjectInfoPanel = "TowerPanel";
        private const string Name = "Name";
        public const string NameValue = "NameValue";
        private const string Damage = "Damage";
        public const string DamageValue = "DamageValue";
        public const string DamageButton = "DamageButton";
        private const string Range = "Range";
        public const string RangeValue = "RangeValue";
        public const string RangeButton = "RangeButton";
        private const string ObjectMoney = "ObjectMoney";
        public const string ObjectMoneyValue = "ObjectmoneyValue";
        public const string ObjectMoneyButton = "ObjectMoneyButton";
        public const string Target = "Target";
        public const string TargetValue = "TargetValue";
        private const string Font = "Bauhaus 93";
        private const string ButtonGrid = "ButtonGrid";
        public const string Cancel = "Cancel";
        public const string Pause = "Pause";
        public const string Next = "Next";

        #endregion

        public static List<ContentControl> CreateControls(GameControl gameControl)
        {
            var folder = new List<string>
            {
                "Shared",
                gameControl.GameControlMode == GameControlMode.CreateMap ||
                gameControl.GameControlMode == GameControlMode.EditMap
                    ? "Map"
                    : "Items"
            };

            return CreateControls(folder, gameControl.HandleControlEvent);
        }

        public static Grid CreateGameInfoPanel(GameControl gameControl)
        {
            var grid = new Grid {Name = InfoPanel};
            gameControl.RegisterName(grid.Name, grid);

            for (var i = 0; i < 2; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.5, GridUnitType.Star)});
            }

            for (var i = 0; i < grid.RowDefinitions.Count; i++)
            for (var j = 0; j < grid.ColumnDefinitions.Count; j++)
            {
                var label = new Label
                {
                    Name = i == 0 ? j == 0 ? Health : HealthValue : j == 0 ? Money : MoneyValue,
                    FontFamily = new FontFamily(Font),
                    Foreground = Brushes.White,
                    Content = j == 0 ? i == 0 ? Health + ":" :
                        Money + ":" :
                        i == 0 ? gameControl.GameCreator.Health.ToString() :
                        gameControl.GameCreator.Money + " (฿)"
                };
                AddAndRegisterComponent(gameControl, label, i, j, grid);
            }

            return grid;
        }

        public static Grid CreateEditorInfoPanel(GameControl gameControl)
        {
            var grid = new Grid {Name = InfoPanel};
            gameControl.RegisterName(grid.Name, grid);

            for (var i = 0; i < 2; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.5, GridUnitType.Star)});
            }

            for (var i = 0; i < grid.RowDefinitions.Count; i++)
            for (var j = 0; j < grid.ColumnDefinitions.Count; j++)
                if (j == 0)
                {
                    var label = new Label
                    {
                        Name = i == 0 ? Health : Money,
                        FontFamily = new FontFamily(Font),
                        Foreground = Brushes.White,
                        Content = i == 0 ? Health + ":" : Money + ":"
                    };
                    AddAndRegisterComponent(gameControl, label, i, j, grid);
                }
                else
                {
                    var textBox = new TextBox
                    {
                        Name = i == 0 ? HealthValue : MoneyValue,
                        FontFamily = new FontFamily(Font),
                        Foreground = Brushes.White,
                        Background = Brushes.Transparent,
                        Text = i == 0 ? "100" : "50"
                    };
                    textBox.PreviewTextInput += gameControl.ControlPreviewTextInput;
                    textBox.LostKeyboardFocus += gameControl.ControlLostKeyboardFocus;
                    AddAndRegisterComponent(gameControl, textBox, i, j, grid);
                }

            return grid;
        }

        public static Grid CreateObjectInfoPanel(GameControl gameControl)
        {
            var grid = new Grid {Name = ObjectInfoPanel};
            gameControl.RegisterName(grid.Name, grid);

            // TODO: add dropDown for target condistion -> Atk [Closest]

            for (var i = 0; i < 3; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)});
            for(var i = 0; i < 5; i++) 
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.5, GridUnitType.Star)});

            for (var i = 0; i < grid.RowDefinitions.Count; i++)
            for (var j = 0; j < grid.ColumnDefinitions.Count; j++)
                if (j == grid.ColumnDefinitions.Count - 1)
                {
                    if (i == 0 || i == grid.RowDefinitions.Count-1) continue;
                    var button = new Button
                    {
                        Name = i == 1 ? DamageButton : i == 2 ? RangeButton : ObjectMoneyButton,
                        FontFamily = new FontFamily(Font),
                        Foreground = Brushes.White,
                        Background = Brushes.Transparent,
                        Content = "",
                        Visibility = Visibility.Hidden
                    };
                    button.Click += gameControl.HandleObjectInfoEvent;
                    AddAndRegisterComponent(gameControl, button, i, j, grid);
                }
                else if(i == grid.RowDefinitions.Count -1 && j == 1)
                {
                    var comboBox = new ComboBox
                    {
                        Name = TargetValue,
                        FontFamily = new FontFamily(Font),
                        Background = Brushes.Transparent,
                        ItemsSource = Enum.GetValues(typeof(TargetCondition)),
                        Visibility = Visibility.Collapsed,
                        SelectedIndex = 0
                    };
                    comboBox.DropDownClosed += gameControl.HandleObjectInfoEvent;
                    Grid.SetColumnSpan(comboBox, 2);
                    AddAndRegisterComponent(gameControl, comboBox, i, j, grid);
                }
                else
                {
                    var name = "";
                    var content = "";
                    switch (i)
                    {
                        case 0:
                            name = j == 0 ? Name : NameValue;
                            content = j == 0 ? Name + ":" : "";
                            break;
                        case 1:
                            name = j == 0 ? Damage : DamageValue;
                            content = j == 0 ? Damage + ":" : "";
                            break;
                        case 2:
                            name = j == 0 ? Range : RangeValue;
                            content = j == 0 ? Range + ":" : "";
                            break;
                        case 3:
                            name = j == 0 ? ObjectMoney : ObjectMoneyValue;
                            content = j == 0 ? Money + ":" : "";
                            break;
                        default:
                            if (j < 1)
                            {
                                name = Target;
                                content = Target;
                            }
                            break;
                    }
                    if(i == 4 && j > 0) continue;
                    var label = new Label
                    {
                        Name = name,
                        FontFamily = new FontFamily(Font),
                        Foreground = Brushes.White,
                        Content = content
                    };
                    if (i == 4) label.Visibility = Visibility.Collapsed;
                    AddAndRegisterComponent(gameControl, label, i, j, grid);
                }

            return grid;
        }

        public static Grid CreateButtons(GameControl gameControl)
        {
            var grid = new Grid {Name = ButtonGrid};

            for (var i = 0; i < 2; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)});

            for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                var button = new Button
                {
                    Name = i == 0
                        ? (gameControl.GameControlMode == GameControlMode.CreateMap ||
                           gameControl.GameControlMode == GameControlMode.EditMap) ? Cancel : Pause
                        : (gameControl.GameControlMode == GameControlMode.CreateMap ||
                           gameControl.GameControlMode == GameControlMode.EditMap)
                            ? Next
                            : Cancel,
                    FontFamily = new FontFamily(Font),
                    Foreground = Brushes.White,
                    Background = Brushes.Transparent,
                    Content = i == 0
                        ? (gameControl.GameControlMode == GameControlMode.CreateMap ||
                           gameControl.GameControlMode == GameControlMode.EditMap) ? Cancel : Pause
                        : (gameControl.GameControlMode == GameControlMode.CreateMap ||
                           gameControl.GameControlMode == GameControlMode.EditMap)
                            ? Next
                            : Cancel
                };
                button.Click += gameControl.HandleButtonEvent;
                AddAndRegisterComponent(gameControl, button, 0, i, grid);
            }

            return grid;
        }

        private static List<ContentControl> CreateControls(ICollection<string> folder, RoutedEventHandler handler)
        {
            var list = new List<ContentControl>();

            var dirs = Directory.GetDirectories(@"../../Grafik");
            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                if (!folder.Contains(dirInfo.Name)) continue;
                foreach (var file in dirInfo.GetFiles())
                {
                    var button = new Button
                    {
                        Name = file.Name.Substring(0, file.Name.IndexOf('.')),
                        Content = file.Name.Substring(0, file.Name.IndexOf('.')),
                        Background = new ImageBrush(new BitmapImage(new Uri(file.FullName)))
                        {
                            Stretch = Stretch.Fill
                        }
                    };
                    var heightBinding = new Binding("Width") {Source = button};
                    button.SetBinding(FrameworkElement.HeightProperty, heightBinding);
                    button.Click += handler;
                    list.Add(button);
                }
            }

            return list;
        }

        private static void AddAndRegisterComponent(FrameworkElement gameControl, Control contentControl, int row,
            int column, Panel grid)
        {
            if (contentControl == null) throw new ArgumentNullException(nameof(contentControl));
            Grid.SetRow(contentControl, row);
            Grid.SetColumn(contentControl, column);
            grid.Children.Add(contentControl);
            gameControl.RegisterName(contentControl.Name, contentControl);
        }
    }
}