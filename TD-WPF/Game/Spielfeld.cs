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
using System.Drawing;

namespace TD_WPF.Game
{
    class Spielfeld
    {
        Grid spielfeld;
        Canvas map;
        double x, y, width, height;
        Random r = new Random();

        public Spielfeld(System.Windows.Controls.UserControl container, double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.spielfeld = (Grid)container.FindName("Spielfeld");
            this.map = (Canvas)spielfeld.FindName("Map");
            initializeMap();
        }

       

        public void initializeMap()
        {
            Bitmap bmp = new Bitmap(Convert.ToInt32(x), Convert.ToInt32(y));
            Graphics g = Graphics.FromImage(bmp);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (r.NextDouble() < 0.5 && r.NextDouble() < 0.5) {
                        Wegobjekt weg = new Wegobjekt(width, height, x, y);
                        g.DrawImage(weg.image, (float)x * i, (float) y * j);
                    }
                }
            } 
        }
    }
}
