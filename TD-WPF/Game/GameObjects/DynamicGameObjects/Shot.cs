using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.DynamicGameObjects
{
    public class Shot : DynamicGameObject
    {
        public int damage { get; set; }
        public Shot(float x, float y, float width, float height, float speed, int damage) : base(x, y, width, height, speed)
        {
            this.damage = damage;
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.gegner),
                Convert.ToInt32(width), Convert.ToInt32(height));
            this.shape = new Ellipse()
            {
                Name = this.GetType().Name,
                Width = width,
                Height = height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(this.image.GetHbitmap(),
                                  IntPtr.Zero,
                                  Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public override void update(GameControl gameControl)
        {
            base.update(gameControl);
        }
    }
}
