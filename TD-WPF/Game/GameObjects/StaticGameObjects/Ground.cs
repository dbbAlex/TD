using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Properties;
using TD_WPF.Tools;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Ground : Path
    {
        public Ground(float x, float y, float width, float height) : base(x, y, width, height)
        {
            image = ImageTool.ResizeImage(new Bitmap(Resource.ground),
                Convert.ToInt32(width), Convert.ToInt32(height));
            shape = new Rectangle
            {
                Name = GetType().Name,
                Width = this.width,
                Height = this.height,
                Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()))
            };
        }

        public Tower tower { get; set; } = null;
    }
}