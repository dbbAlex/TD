using System;
using System.Windows;
using System.Windows.Controls;

namespace TD_WPF.Menu.Items
{
    public partial class MapItem : UserControl
    {
        public MapItem(Guid guid, MapMenu mapMenu)
        {
            InitializeComponent();
            Guid = guid;
            MapMenu = mapMenu;
        }

        public Guid Guid { get; }
        private MapMenu MapMenu { get; }
        
        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            if(!(e.Source is Button button)) return;
            switch (button.Name)
            {
                case "Play":
                case "Edit":
                    MapMenu.LoadMap(this);
                    break;
                case "Remove":
                    MapMenu.RemoveMap(this);
                    break;
            }
        }
    }
}
