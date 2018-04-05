using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Properties;
using TD_WPF.Tools;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Path : GameObject
    {
        public Path(double x, double y, double width, double height, int index) : base(x, y, width, height)
        {
            Index = index;
            Image = ImageTool.ResizeImage(new Bitmap(Resource.path),
                Convert.ToInt32(width), Convert.ToInt32(height));
        }

        public int Index { get; }
    }
}