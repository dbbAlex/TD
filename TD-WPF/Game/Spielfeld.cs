using System;
using System.Windows.Controls;
using TD_WPF.Game.Spielobjekte;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Collections;

namespace TD_WPF.Game
{
    class Spielfeld
    {
        Grid spielfeld;
        Canvas map;
        double x, y, width, height;
        Random r = new Random();

        public Spielfeld(System.Windows.Controls.UserControl container, double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.spielfeld = (Grid)container.FindName("Spielfeld");
            this.map = (Canvas)spielfeld.FindName("Map");
            initializeMap();
        }

       
        // Erstmal zum testen ob wir überhaubt die map mit random weg objekten erstellen können
        public void initializeMap()
        {
            Bitmap bmp = new Bitmap(Convert.ToInt32(map.ActualWidth), Convert.ToInt32(map.ActualHeight));
            Graphics g = Graphics.FromImage(bmp);
            ArrayList list = new ArrayList();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (r.NextDouble() < 0.5 && r.NextDouble() < 0.5) {
                        Wegobjekt weg = new Wegobjekt(width, height, x, y);
                        g.DrawImage(weg.image, (float)width * i, (float) height * j);
                        list.Add(weg);
                    }
                }
            }
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            /*Versuch Bitmap in BitmapImage zu convertieren*/
            IntPtr hBitmap = bmp.GetHbitmap();

            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();

            BitmapSource bmpImg = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);

            image.Source = bmpImg;
            map.Children.Add(image);
            bmp.Save("C://Users//Adrian.Fennert//Desktop//BeispielBMP.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
