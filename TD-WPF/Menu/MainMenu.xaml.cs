using System.Windows;
using System.Windows.Controls;

namespace TD_WPF.Menu
{
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button button)) return;
            switch (button.Name)
            {
                case "Game":
                    ((ContentControl) Parent).Content = new GameMenu();
                    break;
                case "Editor":
                    ((ContentControl) Parent).Content = new EditorMenu();
                    break;
            }
        }
    }
}