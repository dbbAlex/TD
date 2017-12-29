using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte
{
    class Endpunkt : Spielobjekt
    {
        public Endpunkt(double width, double height, double x, double y)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            image = new Bitmap(Properties.Resources.spawn);
            image = ImageTool.ResizeImage(image, Convert.ToInt32(width), Convert.ToInt32(height));
        }


    }
}
