using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TD_WPF.Game.GameUtils
{
    internal static class ControlUtils
    {
        public const string InfoPanel = "InfoPanel";
        private const string Health = "Health";
        public const string HealthValue = "HealthValue";
        private const string Money = "Money";
        public const string MoneyValue = "MoneyValue";

        public static LinkedList<ContentControl> CreatEditorConrtols(GameControl gameControl)
        {
            var folder = new List<string>();
            folder.Add("Map");
            folder.Add("Shared");
            return CreateControls(folder, gameControl.HandleControlEvent, "editor");
        }

        public static LinkedList<ContentControl> CreateGameControls(GameControl gaemControl)
        {
            var folder = new List<string>();
            folder.Add("Items");
            folder.Add("Shared");
            return CreateControls(folder, gaemControl.HandleControlEvent, "game");
        }

        public static Grid CreateInoPanel(GameControl gameControl)
        {
            Grid grid = new Grid {Name = InfoPanel};
            gameControl.RegisterName(grid.Name, grid);

            for (int i = 0; i < 2; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.5, GridUnitType.Star)});
            }

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < grid.ColumnDefinitions.Count; j++)
                {
                    Label label = new Label
                    {
                        Name = i == 0 ? j == 0 ? Health : HealthValue : j == 0 ? Money : MoneyValue,
                        FontFamily = new FontFamily("Bauhaus 93"),
                        Foreground = Brushes.White,
                        Content = j == 0 ? i == 0 ? Health + ":" :
                            Money + ":" :
                            i == 0 ? gameControl.GameCreator.Health.ToString() :
                            gameControl.GameCreator.Money.ToString()
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    grid.Children.Add(label);
                    gameControl.RegisterName(label.Name, label);
                }
            }

            return grid;
        }

        private static LinkedList<ContentControl> CreateControls(List<string> folder, RoutedEventHandler handler,
            string group)
        {
            var list = new LinkedList<ContentControl>();

            var dirs = Directory.GetDirectories(@"../../Grafik");
            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                if (folder.Contains(dirInfo.Name))
                    foreach (var file in dirInfo.GetFiles())
                    {
                        var b = new Button
                        {
                            Name = file.Name.Substring(0, file.Name.IndexOf('.')),
                            Content = file.Name.Substring(0, file.Name.IndexOf('.')),
                            Background = new ImageBrush(new BitmapImage(new Uri(file.FullName)))
                            {
                                Stretch = Stretch.Fill
                            }
                        };
                        var heightBinding = new Binding("Width");
                        heightBinding.Source = b;
                        b.SetBinding(FrameworkElement.HeightProperty, heightBinding);
                        b.Click += handler;
                        list.AddLast(b);
                    }
            }

            return list;
        }
    }
}