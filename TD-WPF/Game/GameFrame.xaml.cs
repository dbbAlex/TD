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
        private Spielfeld feld { get; set;}
        private Image imgMap { get; set; }
        private int width, height;
        private int x = 20, y = 15;
        private bool isMapEditor = false;
        private Object loadGame = null;
            

        public GameFrame()
        {
            InitializeComponent();
            SizeChanged += controlSizeChange;    
            if(isMapEditor)
                Loaded += initializeEditor;
            else
                Loaded += initializeSpielfeld; 
        }

        private void initialize()
        {            
            // Zellgröße berechnen
            width = Convert.ToInt32(this.Map.ActualWidth / x);
            height = Convert.ToInt32(this.Map.ActualHeight / y);
            // neues Spielfeld erstellens
            feld = new Game.Spielfeld(this, x, y, width, height);
        }

        private void initializeSpielfeld(object sender, RoutedEventArgs e)
        {
            initialize();
            if(loadGame != null)
            {
                // load neccessary objects from db
            }
            else
                feld.initializeRandomMap();

        }

        private void initializeEditor(object sender, RoutedEventArgs e)
        {
            initialize();
        }

        private void canvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // remove old rectangle
            // reverse loop because it's faster than RemoveAll(item is Rectangle)
            for(int i = this.Map.Children.Count - 1; i >= 0; i--)
            {
                Object item = this.Map.Children[i];
                if (item is Rectangle)
                    this.Map.Children.RemoveAt(i);
            }

            // we need to calculate where the mouse points on the original bitmap which is strechet by canvas
            
            // get width in actual canvas size
            double xwidth = this.Map.ActualWidth / x;
            double yheight = this.Map.ActualHeight / y;

            // get original mouse position
            double originalX = e.GetPosition(this.MapImage).X;
            double OriginalY = e.GetPosition(this.MapImage).Y;            

            // calculate xy-axis
            int calcX = Convert.ToInt32(Math.Floor(originalX / xwidth));
            int calcY = Convert.ToInt32(Math.Floor(OriginalY / yheight));
            // correct xy-axis if out of range
            calcX = calcX >= x ? calcX-1 : calcX;
            calcY = calcY >= y ? calcY-1 : calcY;

            bool isFree = feld.isFreeField(calcX, calcY);

            // calculate points for field where mouse is over
            double p1 = calcX * xwidth;
            double p2 = calcY * yheight;
            
            Rectangle rec = new Rectangle()
            {
                Height = yheight-1,
                Width = xwidth-1,
                Fill = isFree ? getTransparentBrush(150, Brushes.LawnGreen.Color) : getTransparentBrush(150, Brushes.IndianRed.Color),
            };
            Canvas.SetLeft(rec, p1+1);
            Canvas.SetTop(rec, p2+1);
            //Canvas.SetLeft(rec, Convert.ToDouble(p.Y));
            //Canvas.SetTop(rec, Convert.ToDouble(p.Y));
            this.Map.Children.Add(rec);
        } 

        private void setIsMapEditor(bool isMapEditor)
        {
            this.isMapEditor = isMapEditor;
        }

        private bool isMapeEditor()
        {
            return this.isMapEditor;
        }

        private void setloadGame(Object loadGame)
        {
            this.loadGame = loadGame;
        }

        private Object getLoad()
        {
            return this.loadGame;
        }

        private SolidColorBrush getTransparentBrush(byte a, Color brushColor)
        {
            return new SolidColorBrush(Color.FromArgb(a, brushColor.R, brushColor.G, brushColor.B));
        }

        private void controlSizeChange(object sender, RoutedEventArgs e)
        {
            // remove old rectangle
            // reverse loop because it's faster than RemoveAll(item is Rectangle)
            for (int i = this.Map.Children.Count - 1; i >= 0; i--)
            {
                Object item = this.Map.Children[i];
                if (item is Rectangle)
                    this.Map.Children.RemoveAt(i);
            }
        }
    }
}
