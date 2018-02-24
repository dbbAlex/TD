using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TD_WPF.Game.Tools
{
    class GameFrameHelper
    {
        public static LinkedList<ContentControl> getEditorOptions(GameFrame gameFrame)
        {
            List<string> folder = new List<string>();
            folder.Add("Map");
            folder.Add("Shared");
            return getControls(folder, gameFrame, new RoutedEventHandler(gameFrame.optionHandler.HandleEditorOptionEvent), "editor");
        }

        public static LinkedList<ContentControl> getGameOptions(GameFrame gameFrame)
        {
            List<string> folder = new List<string>();
            folder.Add("Items");
            folder.Add("Shared");
            return getControls(folder, gameFrame, new RoutedEventHandler(gameFrame.optionHandler.HandleGameOptionEvent), "game");            
        }


        private static LinkedList<ContentControl> getControls(List<String> folder, GameFrame gameFrame, RoutedEventHandler handler, string group)
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
                        RadioButton rb = new RadioButton()
                        {
                            Name = file.Name.Substring(0, file.Name.IndexOf('.')),
                            BorderBrush = System.Windows.Media.Brushes.LightGray,
                            Content = file.FullName,
                            GroupName = group,
                            Style = Application.Current.FindResource("ImageRadioButtonStyle") as System.Windows.Style
                        };
                        Binding heightBinding = new Binding("Width");
                        heightBinding.Source = rb;
                        rb.SetBinding(RadioButton.HeightProperty, heightBinding);
                        rb.Checked += handler;
                        list.AddLast(rb);
                    }
                }
            }

            return list;
        }

        public static System.Windows.Controls.Grid getGameStats()
        {
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();

            // TODO: fill grid

            return grid;
        }
    }
}
