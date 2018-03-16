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
    internal class ControlUtils
    {
        public static LinkedList<ContentControl> creatEditorConrtols(GameControl gameControl)
        {
            var folder = new List<string>();
            folder.Add("Map");
            folder.Add("Shared");
            return createControls(folder, gameControl.HandleControlEvent, "editor");
        }

        public static LinkedList<ContentControl> createGameControls(GameControl gaemControl)
        {
            var folder = new List<string>();
            folder.Add("Items");
            folder.Add("Shared");
            return createControls(folder, gaemControl.HandleControlEvent, "game");
        }


        private static LinkedList<ContentControl> createControls(List<string> folder, RoutedEventHandler handler,
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