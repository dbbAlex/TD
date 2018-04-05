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
    public class Base : Path
    {
        public Base(double x, double y, double width, double height, int index) : base(x, y, width, height, index)
        {
            Image = ImageTool.ResizeImage(new Bitmap(Resource.end),
                Convert.ToInt32(width), Convert.ToInt32(height));
        }
    }
}