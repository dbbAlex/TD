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
            feld = new Game.Spielfeld(this, 20,10);
        }

        
    }
}
