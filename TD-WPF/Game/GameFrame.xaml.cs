using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TD_WPF.Game;

namespace TD_WPF
{
    /// <summary>
    /// Interaktionslogik für GameFrame.xaml
    /// </summary>
    public partial class GameFrame : UserControl
    {
        private Spielfeld feld;
        private Canvas map;
        private int width, height;
        private int x = 20, y = 15;

        public GameFrame()
        {
            InitializeComponent();            
            Loaded += initializeSpielfeld;
        }

        private void initializeSpielfeld(object sender, RoutedEventArgs e)
        {
            // Raster initialisieren
            //int x = 20, y = 15;
            // Spielfeld holen
            Grid spiel = (Grid)this.FindName("Spielfeld");
            // Karte auf der Gezeichnet wird holen
            map = (Canvas) spiel.FindName("Map");
            // Karte anpassen
            int pixelsForCanvasX = Convert.ToInt32(map.ActualWidth);
            spiel.ColumnDefinitions[0].Width = new GridLength(pixelsForCanvasX);
            spiel.ColumnDefinitions[1].Width = new GridLength(this.ActualWidth - spiel.ColumnDefinitions[0].Width.Value);
            // Zellgröße berechnen
            width = Convert.ToInt32(map.ActualWidth / x);
            height = Convert.ToInt32(map.ActualHeight / y);
            // neues Spielfeld erstellens
            feld = new Game.Spielfeld(this, x, y, width, height);
            feld.initializeRandomMap();

        }

        private void CanvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // remove old rectangle
            // reverse loop because it's faster than RemoveAll(item is Rectangle)
            for(int i = map.Children.Count - 1; i >= 0; i--)
            {
                Object item = map.Children[i];
                if (item is Rectangle)
                    map.Children.RemoveAt(i);
            }

            // we need to calculate where the mouse points on the original bitmap which is strechet by canvas

            // calculate real bitmap size
            int fieldWidth = x * width;
            int fieldHeight = y * height;

            // get actual canvas size
            double mapWidth = map.ActualWidth;
            double mapHeight = map.ActualHeight;

            // get decrease percentage
            double decreaseWidth = (mapWidth - fieldWidth) / mapWidth;
            double decreaseHeight = (mapHeight - fieldHeight) / mapHeight;

            // get original mouse position
            double originalX = e.GetPosition(this).X;
            double OriginalY = e.GetPosition(this).Y;

            // calculate bitmap mouse position 
            double decreasedX = originalX - (originalX * decreaseWidth);
            double decreasedY = OriginalY - (OriginalY * decreaseHeight);

            // calculate xy-axis
            int calcX = Convert.ToInt32(decreasedX) / width;
            int calcY = Convert.ToInt32(decreasedY) / height;
            // correct xy-axis if out of range
            calcX = calcX >= x ? calcX-1 : calcX;
            calcY = calcY >= y ? calcY-1 : calcY;

            bool isFree = feld.isFreeField(calcX, calcY);

            // calculate points for field where mouse is over
            int p1 = calcX * width;
            int p2 = calcY * height;
            
            Rectangle rec = new Rectangle()
            {
                Height = height -1,
                Width = width -1,
                Fill = isFree ? Brushes.LawnGreen : Brushes.IndianRed,
            };
            Canvas.SetLeft(rec, Convert.ToDouble(p1)+1);
            Canvas.SetTop(rec, Convert.ToDouble(p2)+1);
            //Canvas.SetLeft(rec, Convert.ToDouble(p.Y));
            //Canvas.SetTop(rec, Convert.ToDouble(p.Y));
            map.Children.Add(rec);
        } 

    }
}
