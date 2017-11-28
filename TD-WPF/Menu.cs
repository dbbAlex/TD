using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TD_WPF
{
    class Menu
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
