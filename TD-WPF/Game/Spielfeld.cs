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
    public class Spielfeld
    {
        #region attributes
        GameFrame container;
        public LinkedList<Spielobjekt> strecke { get; set; }
        public LinkedList<Spielobjekt> tower { get; set; }
        public LinkedList<Spielobjekt> towerground { get; set; }
        private Random r = new Random();

        #endregion
        public Spielfeld(GameFrame container)
        {
            this.container = container;
            this.strecke = new LinkedList<Spielobjekt>();
            this.tower = new LinkedList<Spielobjekt>();
            this.towerground = new LinkedList<Spielobjekt>();
        }
               
        public void update()
        {
            addBitmapToCanvas(drawRoute());
        }

        public Bitmap drawRoute()
        {
            // Bitmap zum zeichnen erstellen
            Bitmap bmp = new Bitmap(this.container.width * this.container.x, this.container.height * this.container.y);
            Graphics g = Graphics.FromImage(bmp);

            //Iterate through list and draw on bitmap
            foreach (var obj in strecke)
            {
                g.DrawImage(obj.image, Convert.ToInt32(this.container.width * obj.x), Convert.ToInt32(this.container.height * obj.y));
            }

            foreach (var obj in towerground)
            {
                g.DrawImage(obj.image, Convert.ToInt32(this.container.width * obj.x), Convert.ToInt32(this.container.height * obj.y));
            }

            foreach (var obj in tower)
            {
                g.DrawImage(obj.image, Convert.ToInt32(this.container.width * obj.x), Convert.ToInt32(this.container.height * obj.y));
            }

            if (this.container.showGrid)
                g.DrawImage(showGrid(), 0, 0);

            return bmp;
        }

        public void addBitmapToCanvas(Bitmap bmp)
        {
            // convert Bitmap to Image
            IntPtr hBitmap = bmp.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSource bmpImg = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            this.container.MapImage.Source = bmpImg;
        }

        public bool isFreeField(int pX, int pY, List<LinkedList<Spielobjekt>> lists)
        {
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    if (pX == Convert.ToInt32(item.x) && pY == Convert.ToInt32(item.y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region Methods for level editor

        public void initializeMapEditor()
        {
            addBitmapToCanvas(drawRoute());
        }

        public Bitmap showGrid()
        {
            Bitmap bmp = new Bitmap(this.container.width * this.container.x, 
                this.container.height * this.container.y);
            Graphics g = Graphics.FromImage(bmp);

            for(int i = this.container.width; i < this.container.width * this.container.x; i += this.container.width)
            {
                g.DrawLine(Pens.Black, i, 0, i, this.container.height * this.container.y);
            }
            for (int i = this.container.height; i < this.container.height * this.container.y; i += this.container.height)
            {
                g.DrawLine(Pens.Black, 0, i, this.container.width * this.container.x, i);
            }

            return bmp;
        }
                
        #endregion
        
        #region Methods for random game generation

        public void initializeRandomMap()
        {
            calculateRoute();
            addBitmapToCanvas(drawRoute());
        }

        private void calculateRoute()
        {
            List<Spielobjekt> space = new List<Spielobjekt>();
            // Min/max Wegobjekte festlegen
            int min = Convert.ToInt32(this.container.x * this.container.y * 0.1);
            int max = Convert.ToInt32(this.container.x * this.container.y * 0.2);
            int weg = r.Next(min, max);

            // Felder festlegen
            Startpunkt start = randomField();
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
                next = strecke.Count + 1 == weg ? 
                    new Endpunkt(this.container.width, this.container.height, list[index].x, list[index].y) : list[index];
                strecke.AddLast(next);

                //add space to list
                list.Remove(next);
                space.AddRange(list);

                current = next;

            } while (strecke.Count < weg);

            // add towerGround
            int towerGroundSize = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(strecke.Count / 20)));

            do
            {
                int random = r.Next(strecke.Count);
                LinkedListNode<Spielobjekt> node = strecke.First;
                while(random > 0)
                {
                    node = node.Next;
                    random--;
                }
                List<Spielobjekt> list = calculatePossibleFields(getPossibleNeighbourFields(node.Value), null);
                if (list.Count == 0)
                    continue;

                int index = r.Next(list.Count);
                towerground.AddLast(new Turmfundament(this.container.width, this.container.height, 
                    list[index].x, list[index].y));

                towerGroundSize--;
            } while (towerGroundSize != 0);
                        
        }
                
        public List<Spielobjekt> getPossibleNeighbourFields(Spielobjekt obj)
        {
            List<Spielobjekt> list = new List<Spielobjekt>();

            //unten
            if(obj.y + 1 < this.container.y)
                list.Add(new Wegobjekt(this.container.width, this.container.height, obj.x, obj.y + 1));

            //oben
            if (obj.y - 1 >= 0)
                list.Add(new Wegobjekt(this.container.width, this.container.height, obj.x, obj.y - 1));

            //links
            if (obj.x - 1 >= 0)
                list.Add(new Wegobjekt(this.container.width, this.container.height, obj.x-1, obj.y));

            //rechts
            if (obj.x + 1 < this.container.x)
                list.Add(new Wegobjekt(this.container.width, this.container.height, obj.x+1, obj.y));

            return list;
        }

        public List<Spielobjekt> calculatePossibleFields(List<Spielobjekt> fields, List<Spielobjekt> space)
        {
            List<Spielobjekt> removable = new List<Spielobjekt>();
            List<LinkedList<Spielobjekt>> lists = new List<LinkedList<Spielobjekt>> { this.strecke,
                this.tower, space != null ? new LinkedList<Spielobjekt> (space) : new LinkedList<Spielobjekt>() };
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    foreach (var element in fields)
                    {
                        if (item.x == element.x && item.y == element.y)
                            removable.Add(element);
                    }
                }
            }            
            
            foreach (var item in removable)
            {
                fields.Remove(item);
            }
            
            return fields;
        }

        private Startpunkt randomField()
        {
            return new Startpunkt(this.container.width, this.container.height, 
                r.Next(this.container.x), r.Next(this.container.y));
        }

        #endregion

    }
}
