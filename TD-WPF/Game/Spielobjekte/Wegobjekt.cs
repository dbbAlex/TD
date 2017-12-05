using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte
{
    class Wegobjekt : Spielobjekt
    {
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        
        public Bitmap image = new Bitmap(Properties.Resources.weg);

        public Wegobjekt(double width, double height, double x, double y)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            image = ImageTool.ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
        }


    }
}
