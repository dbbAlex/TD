using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Ground : Path
    {
        public Ground(float x, float y, float width, float height, int index) : base(x, y, width, height, index)
        {
            Image = ImageTool.ResizeImage(new Bitmap(Resource.ground),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()));
        }

        public Tower Tower { get; set; } = null;
    }
}