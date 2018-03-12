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
using TD_WPF.Game.Handler;
using TD_WPF.Game.Spielobjekte;
using TD_WPF.Game.Spielobjekte.Items;
using TD_WPF.Game.Tools;

namespace TD_WPF
{
    public partial class GameFrame : UserControl
    {
        #region attributes
        public Spielfeld feld { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int x { get; set; } = 20;
        public int y { get; set; } = 15;
        public bool isMapEditor { get; set; } = !true;
        public bool showGrid { get; set; } = true;
        public Object loadGame { get; set; } = null;
        public OptionEventHandler optionHandler { get; set; }
        public List<Spielobjekt> possibleHint { get; set; } = new List<Spielobjekt>();

        #endregion

        public GameFrame()
        {
            InitializeComponent();
            optionHandler = new OptionEventHandler(this);
            SizeChanged += controlSizeChange;    
            if(isMapEditor)
                Loaded += initializeEditor;
            else
                Loaded += initializeSpielfeld; 
        }

        #region initialization
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
            feld.initializeMapEditor();
        }

        private void initialize()
        {
            // Zellgröße berechnen
            width = Convert.ToInt32(this.Map.ActualWidth / x);
            height = Convert.ToInt32(this.Map.ActualHeight / y);
            // neues Spielfeld erstellens
            feld = new Game.Spielfeld(this);

            calcualteRows();
            fillItems();
        }

        private void calcualteRows()
        {
            this.Control.RowDefinitions.Clear();
            Double p = this.Control.ActualWidth / 2 / this.Control.ActualHeight;
            Double last = p;
            for (; last < 1; last += p)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(p, GridUnitType.Star);
                this.Control.RowDefinitions.Add(gridRow);
            }
            if (last != 1)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(1 - (last - p), GridUnitType.Star);
                this.Control.RowDefinitions.Add(gridRow);
            }
        }

        private void fillItems()
        {
            LinkedList<ContentControl> mapItems = isMapEditor ? GameFrameHelper.getEditorOptions(this) : GameFrameHelper.getGameOptions(this);
            LinkedListNode<ContentControl> current = mapItems.First;
            LinkedListNode<ContentControl> last = mapItems.Last;

            Action fillOptions = delegate
            {   
                for (int row = 0; row < this.Control.RowDefinitions.Count; row++)
                {
                    for (int column = 0; column < this.Control.ColumnDefinitions.Count; column++)
                    {                        
                        Grid.SetRow(current.Value, row);
                        Grid.SetColumn(current.Value, column);
                        this.Control.Children.Add(current.Value);
                        if (current == last)
                            return;
                        current = current.Next;
                    }
                }
            };
            fillOptions();
        }
        #endregion

        #region mouselistener
        private void canvasMouseClick(object sender, MouseEventArgs e)
        {
            if (this.Map.IsMouseOver)
            {
                Point p = getXYMousePosition(e);
                if (isMapEditor && optionHandler.nameOfEditorOption != null && 
                    feld.isFreeField(Convert.ToInt32(p.X), Convert.ToInt32(p.Y), 
                    new List<LinkedList<Spielobjekt>> {this.feld.strecke, this.feld.tower}))
                {
                    if (optionHandler.nameOfEditorOption.Equals("weg") && this.feld.strecke.Count > 0
                        && !(this.feld.strecke.Last.Value.GetType() == typeof(Endpunkt))
                        && isPossibleOption(p.X, p.Y))
                    {
                        redraw(new Wegobjekt(this.width, this.height, p.X, p.Y), this.feld.strecke);
                    }
                    else if (optionHandler.nameOfEditorOption.Equals("spawn")
                        && this.feld.strecke.Count == 0)
                    {
                        redraw(new Startpunkt(this.width, this.height, p.X, p.Y), this.feld.strecke);
                    }
                    else if (optionHandler.nameOfEditorOption.Equals("ziel") && this.feld.strecke.Count > 0
                        && this.feld.strecke.Last.Value.GetType() == typeof(Wegobjekt) && isPossibleOption(p.X, p.Y))
                    {
                        redraw(new Endpunkt(this.width, this.height, p.X, p.Y), this.feld.strecke);
                    }
                    else if (optionHandler.nameOfEditorOption.Equals("ground"))
                    {
                        redraw(new Turmfundament(this.width, this.height, p.X, p.Y), this.feld.towerground);
                    }
                }
                else if (!isMapEditor && optionHandler.nameOfGameOption != null)
                {
                    if (optionHandler.nameOfGameOption.Equals("ground") && 
                        feld.isFreeField(Convert.ToInt32(p.X), Convert.ToInt32(p.Y),
                    new List<LinkedList<Spielobjekt>> { this.feld.strecke, this.feld.towerground }))
                    {
                        redraw(new Turmfundament(this.width, this.height, p.X, p.Y), this.feld.towerground);
                    }
                    else if (isPossibleOption(p.X, p.Y))
                    {
                        redraw(new Turm(this.width, this.height, p.X, p.Y), this.feld.tower);
                    }
                }
            }
        }

        private void canvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (((this.optionHandler.nameOfEditorOption != null && (this.optionHandler.nameOfEditorOption.Equals("ground") || 
                (this.optionHandler.nameOfEditorOption.Equals("spawn") && this.feld.strecke.Count == 0))) || 
                (this.optionHandler.nameOfGameOption != null && (this.optionHandler.nameOfGameOption.Equals("ground")))) 
                && this.Map.IsMouseOver)
            {
                removeRecatngles();
                Point p = getXYMousePosition(e);

                bool isFree = feld.isFreeField(Convert.ToInt32(p.X), Convert.ToInt32(p.Y), new List<LinkedList<Spielobjekt>> { this.feld.strecke, this.feld.towerground });

                Rectangle rec = new Rectangle()
                {
                    Height = (this.Map.ActualHeight / y) - 1,
                    Width = (this.Map.ActualWidth / x) - 1,
                    Fill = isFree ? getTransparentBrush(150, Brushes.LawnGreen.Color) : getTransparentBrush(150, Brushes.IndianRed.Color),
                };
                Canvas.SetLeft(rec, p.X * (this.Map.ActualWidth / x) + 1);
                Canvas.SetTop(rec, p.Y * (this.Map.ActualHeight / y) + 1);
                this.Map.Children.Add(rec);
            }
        }

        private Point getXYMousePosition(System.Windows.Input.MouseEventArgs e)
        {
            // calculate xy-axis
            int calcX = Convert.ToInt32(Math.Floor(e.GetPosition(this.MapImage).X / (this.Map.ActualWidth / x)));
            int calcY = Convert.ToInt32(Math.Floor(e.GetPosition(this.MapImage).Y / (this.Map.ActualHeight / y)));
            // correct xy-axis if out of range
            calcX = calcX >= x ? calcX - 1 : calcX;
            calcY = calcY >= y ? calcY - 1 : calcY;
            return new Point(calcX, calcY);
        }
        #endregion

        #region further events
        private void controlSizeChange(object sender, RoutedEventArgs e)
        {
            removeRecatngles();
        }

        #endregion

        #region other methods
        private bool isPossibleOption(double px, double py)
        {
            bool possible = false;

            if(possibleHint.Count != 0)
            {
                foreach (var item in possibleHint)
                {
                    if(item.x == px && item.y == py)
                    {
                        possible = true;
                        break;
                    }
                }
            }

            return possible;
        }

        private void redraw(Spielobjekt o, LinkedList<Spielobjekt> l)
        {
            l.AddLast(o);
            removeRecatngles();
            possibleHint.Clear();
            this.feld.addBitmapToCanvas(this.feld.drawRoute());
            showHints();
        }
        
        public void removeRecatngles()
        {
            // remove old rectangle
            // reverse loop because it's faster than RemoveAll(item is Rectangle)
            for (int i = this.Map.Children.Count - 1; i >= 0; i--)
            {
                Object item = this.Map.Children[i];
                if (item is Rectangle && ((Rectangle)item).Name.Length == 0)
                    this.Map.Children.RemoveAt(i);
            }
        }

        public SolidColorBrush getTransparentBrush(byte a, Color brushColor)
        {
            return new SolidColorBrush(Color.FromArgb(a, brushColor.R, brushColor.G, brushColor.B));
        }
               
        public void showHints()
        {
            if (this.isMapEditor)
            {
                if ((this.optionHandler.nameOfEditorOption.Equals("weg")
                    || this.optionHandler.nameOfEditorOption.Equals("ziel")) && this.feld.strecke.Count > 0
                    && this.feld.strecke.Last.Value.GetType() != typeof(Endpunkt))
                {
                    Spielobjekt last = this.feld.strecke.Last.Value;
                    possibleHint = this.feld.calculatePossibleFields(this.feld.getPossibleNeighbourFields(last),
                        new List<Spielobjekt>());
                }
            }
            else
            {
                if (!this.optionHandler.nameOfGameOption.Equals("ground") && this.feld.towerground.Count > 0)
                {
                    foreach (var item in this.feld.towerground)
                    {
                        possibleHint.Add(item);
                        foreach (var element in this.feld.tower)
                        {
                            if(item.x == element.x && item.y == element.y)
                            {
                                possibleHint.Remove(item);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var item in possibleHint)
            {
                double p1 = item.x * (this.Map.ActualWidth / this.x);
                double p2 = item.y * (this.Map.ActualHeight / this.y);
                Rectangle rec = new Rectangle()
                {
                    Width = (this.Map.ActualWidth / this.x) - 1,
                    Height = (this.Map.ActualHeight / this.y) - 1,
                    Fill = this.getTransparentBrush(150, System.Windows.Media.Brushes.LawnGreen.Color)
                };
                Canvas.SetLeft(rec, p1 + 1);
                Canvas.SetTop(rec, p2 + 1);
                this.Map.Children.Add(rec);
            }
        }
        #endregion
    }
}
