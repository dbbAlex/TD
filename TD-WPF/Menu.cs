using System.Windows.Controls;

namespace TD_WPF
{
    internal class Menu
    {
        public static UserControl getMenuMain()
        {
            return new MenuMain();
        }

        public static UserControl getMenuEinstellungen()
        {
            return new MenuEinstellungen();
        }
    }
}