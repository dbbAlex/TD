using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Tools;

namespace TD_WPF.Game.Spielobjekte.Items
{
    public class Gegner : MoveableObject
    {
        public int health { get; set; }
        public int damage { get; set; }
        public double intervalInMilli { get; set; }
        public Gegner(double width, double height, double x, double y, int health, int damage, double intervalInMilli) : base(width, height, x, y)
        {
            this.health = health;
            this.damage = damage;
            this.intervalInMilli = intervalInMilli;
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.gegner),
                Convert.ToInt32(width), Convert.ToInt32(height));
        }        
    }
}
