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
    public class Tower : GameObject
    {
        public Tower(float x, float y, float width, float height, float shotIntervall, float shotSpeed, int shotDamage)
            : base(x, y, width, height)
        {
            ShotIntervall = shotIntervall;
            ShotSpeed = shotSpeed;
            ShotDamage = shotDamage;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.tower),
                Convert.ToInt32(width), Convert.ToInt32(height));
            Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()));
        }

        private float ShotIntervall { get; set; }
        private float ShotSpeed { get; set; }
        private int ShotDamage { get; set; }
    }
}