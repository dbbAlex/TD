﻿using System;
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
    public class Base : Path
    {
        public Base(float x, float y, float width, float height, int index) : base(x, y, width, height, index)
        {
            this.Image = ImageTool.ResizeImage(new Bitmap(Resource.ziel),
                Convert.ToInt32(width), Convert.ToInt32(height));
            this.Shape.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()));
        }
    }
}