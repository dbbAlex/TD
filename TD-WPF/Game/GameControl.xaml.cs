using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game.GameManagerTask;
using TD_WPF.Game.GameUtils;

namespace TD_WPF.Game
{
    /// <summary>
    /// Interaktionslogik für GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        #region attibutes
        public GameCreator gameCreator { get; set; }
        public GameManager gameManager { get; set; }
        #endregion

        public GameControl()
        {
            InitializeComponent();
            Loaded += initialize;


        }

        private void initialize(object sender, RoutedEventArgs e)
        {
            this.gameCreator = new GameCreator(this);
            this.gameCreator.initilizeRandomPath();
            this.gameManager = new GameManager();
            this.Dispatcher.InvokeAsync(() => this.gameManager.run(this));
        }
    }
}
