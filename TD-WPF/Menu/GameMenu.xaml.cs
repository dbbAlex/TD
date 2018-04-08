using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game;
using TD_WPF.Game.Enumerations;

namespace TD_WPF.Menu
{
    public partial class GameMenu : UserControl
    {
        public GameMenu()
        {
            InitializeComponent();
        }

        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button button)) return;
            switch (button.Name)
            {
                case "NewGame":
                    ((ContentControl) Parent).Content = new GameControl(null, GameControlMode.PlayRandom);
                    break;
                case "LoadGame":
                    ((ContentControl) Parent).Content = new MapMenu(GameControlMode.PlayMap);
                    break;
                case "Back":
                    ((ContentControl) Parent).Content = new MainMenu();
                    break;
            }
        }
    }
}