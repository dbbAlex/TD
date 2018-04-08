using System.Windows;
using System.Windows.Controls;
using TD_WPF.Game;
using TD_WPF.Game.Enumerations;

namespace TD_WPF.Menu
{
    public partial class EditorMenu : UserControl
    {
        public EditorMenu()
        {
            InitializeComponent();
        }

        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button button)) return;
            switch (button.Name)
            {
                case "NewMap":
                    ((ContentControl) Parent).Content = new GameControl(null, GameControlMode.CreateMap);
                    break;
                case "EditMap":
                    ((ContentControl) Parent).Content = new MapMenu(GameControlMode.EditMap);
                    break;
                case "Back":
                    ((ContentControl) Parent).Content = new MainMenu();
                    break;
            }
        }
    }
}