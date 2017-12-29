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
        LinkedList<Spielobjekt> strecke = new LinkedList<Spielobjekt>();
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
            calculateRoute();
            drawRoute();
        }

        private void calculateRoute()
        {
            List<Spielobjekt> space = new List<Spielobjekt>();
            // Min/max Wegobjekte festlegen
            int min = Convert.ToInt32(x * y * 0.1);
            int max = Convert.ToInt32(x * y * 0.2);
            int weg = r.Next(min, max);

            // Felder festlegen
            Endpunkt start = randomField();
            strecke.AddFirst(start);

            Spielobjekt current = start;
            do
            {
                Spielobjekt next;
                List<Spielobjekt> list = calculatePossibleFields(getPossibleNeighbourFields(current), space);

                // check if list is empty
                if (list.Count == 0)
                {
                    // calculate new route
                    strecke.Clear();
                    space.Clear();
                    strecke.AddFirst(start);
                    current = start;
                    continue;
                }

                // get next object
                int index = r.Next(list.Count);
                next = strecke.Count + 1 == weg ? new Endpunkt(width, height, list[index].x, list[index].y) : list[index];
                strecke.AddLast(next);

                //add space to list
                list.Remove(next);
                space.AddRange(list);

                current = next;

            } while (strecke.Count < weg);            
        }

        private void drawRoute()
        {
            // Bitmap zum zeichnen erstellen
            Bitmap bmp = new Bitmap(Convert.ToInt32(map.ActualWidth), Convert.ToInt32(map.ActualHeight));
            Graphics g = Graphics.FromImage(bmp);

            //Iterate through list and draw on bitmap
            foreach (var obj in strecke)
            {
                g.DrawImage(obj.image, Convert.ToInt32(width*obj.x), Convert.ToInt32(height*obj.y));                
            }
            
            // convert Bitmap to Image
            IntPtr hBitmap = bmp.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSource bmpImg = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bmpImg;

            //add BitmapImage to canvas
            map.Children.Add(image);
        }

        private List<Spielobjekt> getPossibleNeighbourFields(Spielobjekt obj)
        {
            List<Spielobjekt> list = new List<Spielobjekt>();

            //unten
            if(obj.y + 1 < y)
                list.Add(new Wegobjekt(width, height, obj.x, obj.y + 1));

            //oben
            if (obj.y - 1 >= 0)
                list.Add(new Wegobjekt(width, height, obj.x, obj.y - 1));

            //links
            if (obj.x - 1 >= 0)
                list.Add(new Wegobjekt(width, height, obj.x-1, obj.y));

            //rechts
            if (obj.x + 1 < x)
                list.Add(new Wegobjekt(width, height, obj.x+1, obj.y));

            return list;
        }

        private List<Spielobjekt> calculatePossibleFields(List<Spielobjekt> fields, List<Spielobjekt> space)
        {
            List<Spielobjekt> removable = new List<Spielobjekt>();
            foreach (var item in strecke)
            {
                foreach (var element in fields)
                {
                    if (item.x == element.x && item.y == element.y)
                        removable.Add(element);
                }
            }
            foreach (var item in space)
            {
                foreach (var element in fields)
                {
                    if (item.x == element.x && item.y == element.y)
                        removable.Add(element);
                }
            }
            foreach (var item in removable)
            {
                fields.Remove(item);
            }
            
            return fields;
        }

        private Endpunkt randomField()
        {
            return new Endpunkt(width, height, r.Next(x), r.Next(y));
        }        
    }
}
