using System;
using System.Windows.Controls;
using TD_WPF.Game.Spielobjekte;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Collections;
using System.Collections.Generic;

namespace TD_WPF.Game
{
    class Spielfeld
    {
        Grid spielfeld;
        Canvas map;
        int x, y, width, height;
        LinkedList<Wegobjekt> weg = new LinkedList<Wegobjekt>();
        Random r = new Random();

        public Spielfeld(System.Windows.Controls.UserControl container, int x, int y, int width, int height)
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
                        g.DrawImage(weg.image, Convert.ToInt32(width * i), Convert.ToInt32(height * j));
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

        private void erstelleWeg()
        {
            // Bitmap zum zeichnen erstellen
            Bitmap bmp = new Bitmap(Convert.ToInt32(map.ActualWidth), Convert.ToInt32(map.ActualHeight));
            Graphics g = Graphics.FromImage(bmp);

            // Start und Ziel festlegen
            Wegobjekt start = startEndPoint(null);
            Wegobjekt ziel = startEndPoint(start);

            // Min/max Wegobjekte festlegen
            int min = Convert.ToInt32(x * y * 0.1);
            int max = Convert.ToInt32(x * y * 0.75);
            Wegobjekt last = start;
            Wegobjekt current = start;
            do
            {
                //x+1 y
                //x-1 y
                //x y+1
                //x y-1


            }while()


            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            
            /*Versuch Bitmap in BitmapImage zu convertieren*/
            IntPtr hBitmap = bmp.GetHbitmap();

            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();

            BitmapSource bmpImg = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);

            image.Source = bmpImg;
            map.Children.Add(image);
        }

        private Wegobjekt startEndPoint(Wegobjekt o)
        {
            Wegobjekt w = null;
            double direction = r.NextDouble();
            int horizontal = 0;
            int vertical = 0;
            if(direction< 0.25) // Norden
            {
                vertical = -1;
            }else if(direction < 0.5) // Osten
            {
                horizontal = 1;
            }else if(direction < 0.75) // Süden
            {
                vertical = 1;
            }
            else // Westen
            {
                horizontal = -1;
            }
            do
            {
                int value = r.Next((horizontal != 0 ? y : x));

                w = new Wegobjekt(width, height, horizontal != 0 ? horizontal : value, vertical != 0 ? vertical : value);
            } while (w.Equals(o));
            

            return w;
        }
    }
}
