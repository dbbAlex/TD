using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Path : GameObject
    {
        public Path(float x, float y, float width, float height) : base(x, y, width, height)
        {
            this.image = ImageTool.ResizeImage(new Bitmap(Properties.Resource.weg),
                Convert.ToInt32(width), Convert.ToInt32(height));
            this.shape = new System.Windows.Shapes.Rectangle()
            {
                Name = this.GetType().Name,
                Width = this.width,
                Height = this.height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(this.image.GetHbitmap(),
                                  IntPtr.Zero,
                                  Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions()))
            };
        }
    }
}
