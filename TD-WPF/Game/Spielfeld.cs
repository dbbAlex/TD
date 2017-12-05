using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TD_WPF.Game.Spielobjekte;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace TD_WPF.Game
{
    class Spielfeld
    {
        ContentControl container;
        Grid spielfeld, map;
        int x, y;
        double width, heigth;
        Random r = new Random();

        public Spielfeld(System.Windows.Controls.UserControl container, int x, int y)
        {
            this.container = container;
            this.x = x;
            this.y = y;
            this.spielfeld = (Grid)container.FindName("Spielfeld");
            initializeMap();
        }

       

        public void initializeMap()
        {
               
        }
    }
}
