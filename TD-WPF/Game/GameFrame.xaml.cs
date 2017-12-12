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
        public GameFrame()
        {
            InitializeComponent();            
            Loaded += initializeSpielfeld;
        }

        private void initializeSpielfeld(object sender, RoutedEventArgs e)
        {
            // Raster initialisieren
            int x = 20, y = 10;
            // Spielfeld holen
            Grid spiel = (Grid)this.FindName("Spielfeld");
            // Karte auf der Gezeichnet wird holen
            Canvas map = (Canvas) spiel.FindName("Map");
            // Karte anpassen
            int pixelsForCanvasX = Convert.ToInt32(map.ActualWidth);
            spiel.ColumnDefinitions[0].Width = new GridLength(pixelsForCanvasX);
            spiel.ColumnDefinitions[1].Width = new GridLength(this.ActualWidth - spiel.ColumnDefinitions[0].Width.Value);
            // Zellgröße berechnen
            int width = Convert.ToInt32(map.ActualWidth / x);
            int height = Convert.ToInt32(map.ActualHeight / y);
            // neues Spielfeld erstellens
            feld = new Game.Spielfeld(this, x, y, width, height);

        }


    }
}
