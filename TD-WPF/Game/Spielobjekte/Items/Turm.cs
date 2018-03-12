using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte.Items
{
    class Turm : Spielobjekt
    {
        public double interval { get; set; }
        public int damage { get; set; }
        public int range { get; set; }
        public Turm(double width, double height, double x, double y) : base(width, height, x, y)
        {
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.tower), 
                Convert.ToInt32(width), Convert.ToInt32(height));
        }
    }
}

