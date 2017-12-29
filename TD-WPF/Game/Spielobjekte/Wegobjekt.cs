using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte
{
    class Wegobjekt : Spielobjekt
    {
        public Wegobjekt(double width, double height, double x, double y)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.image = new Bitmap(Properties.Resources.weg);
            this.image = ImageTool.ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
        }


    }
}
