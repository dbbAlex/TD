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

namespace TD_WPF.Game.Tools
{
    class GameFrameHelper
    {
        public static LinkedList<ContentControl> getEditorOptions()
        {
            LinkedList<ContentControl> list = new LinkedList<ContentControl>();

            // TODO: fill list
            //<RadioButton Grid.Column="0" Grid.Row="0" GroupName="0" Style="{StaticResource ImageRadioButtonStyle}" BorderBrush="White" Content="/Grafik/grass.jpg" Height="{Binding Path=ActualWidth, RelativeSource={RelativeSource Self}}"/>
            //string path = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), @"/Grafik");
            String[] dirs = Directory.GetDirectories(@"../../Grafik");
            foreach (var dir in dirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);                
                if(dirInfo.Name.Equals("Map") || dirInfo.Name.Equals("Shared"))
                {
                    foreach (var file in dirInfo.GetFiles())
                    {
                        RadioButton rb = new RadioButton()
                        {
                            Name = file.Name.Substring(0, file.Name.IndexOf('.')),
                            BorderBrush = System.Windows.Media.Brushes.LightGray,
                            Content = file.FullName,
                            GroupName = "editor",
                            Style = Application.Current.FindResource("ImageRadioButtonStyle") as System.Windows.Style
                        };
                        Binding heightBinding = new Binding("Width");
                        heightBinding.Source = rb;
                        rb.SetBinding(RadioButton.HeightProperty, heightBinding);
                        list.AddLast(rb);
                    }
                }
            }

            return list;
        }

        public static LinkedList<ContentControl> getGameOptions()
        {
            LinkedList<ContentControl> list = new LinkedList<ContentControl>();

            // TODO: fill list

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
