using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte
{
    class Wegobjekt : Spielobjekt
    {
        public Wegobjekt(double width, double height, double x, double y) : base(width, height, x, y)
        {
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.weg), 
                Convert.ToInt32(width), Convert.ToInt32(height));
        }


    }
}
