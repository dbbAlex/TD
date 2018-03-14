using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TD_WPF.Game.GameUtils
{
    class ControlUtils
    {
        public static LinkedList<ContentControl> creatEditorConrtols(GameControl gameControl)
        {
            List<string> folder = new List<string>();
            folder.Add("Map");
            folder.Add("Shared");
            return createControls(folder, new RoutedEventHandler(gameControl.HandleControlEvent), "editor");
        }

        public static LinkedList<ContentControl> createGameControls(GameControl gaemControl)
        {
            List<string> folder = new List<string>();
            folder.Add("Items");
            folder.Add("Shared");
            return createControls(folder, new RoutedEventHandler(gaemControl.HandleControlEvent), "game");
        }


        private static LinkedList<ContentControl> createControls(List<String> folder, RoutedEventHandler handler, string group)
        {
            LinkedList<ContentControl> list = new LinkedList<ContentControl>();

            String[] dirs = Directory.GetDirectories(@"../../Grafik");
            foreach (var dir in dirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if (folder.Contains(dirInfo.Name))
                {
                    foreach (var file in dirInfo.GetFiles())
                    {
                        Button b = new Button()
                        {
                            Name = file.Name.Substring(0, file.Name.IndexOf('.')),
                            Content = file.Name.Substring(0, file.Name.IndexOf('.')),
                            Background = new ImageBrush(new BitmapImage(new Uri(file.FullName)))
                            {
                                Stretch = Stretch.Fill
                            }
                        };
                        Binding heightBinding = new Binding("Width");
                        heightBinding.Source = b;
                        b.SetBinding(Button.HeightProperty, heightBinding);
                        b.Click += handler;
                        list.AddLast(b);
                    }
                }
            }

            return list;
        }
    }
}
