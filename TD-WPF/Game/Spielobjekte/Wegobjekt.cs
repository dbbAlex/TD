using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte
{
    class Wegobjekt : Spielobjekt
    {
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public Bitmap image = new Bitmap(Image.FromFile("/Grafik/Weg.jpg"));

        public Wegobjekt(int width, int height, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            image = ImageTool.ResizeImage(image, width, height);
        }


    }
}
