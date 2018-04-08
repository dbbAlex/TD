using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using TD_WPF.DataBase;
using TD_WPF.Game.Save;
using TD_WPF.Menu.Items;
using TD_WPF.Game;
using TD_WPF.Game.Enumerations;

namespace TD_WPF.Menu
{
    public partial class MapMenu : UserControl
    {
        public MapMenu(GameControlMode gameControlMode)
        {
            InitializeComponent();
            Loaded += Initialize;
            GameControlMode = gameControlMode;
        }

        private GameControlMode GameControlMode { get; }
        private List<DbObject> DbObjects { get; set; }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            DbObjects = DbManager.LoadDbObjects();
            foreach (var item in DbObjects)
            {
                var mapItem = new MapItem(item.MetaData.Guid, this)
                {
                    Image =
                    {
                        Source = Imaging.CreateBitmapSourceFromHBitmap(item.MetaData.Thumbnail.GetHbitmap(),
                            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                    }
                };
                mapItem.Created.Content +=
                    $"{item.MetaData.CreationDate.ToShortDateString()} " +
                    $"{item.MetaData.CreationDate.ToShortTimeString()}";
                mapItem.Modified.Content +=
                    $"{item.MetaData.ModifiedDate.ToShortDateString()} " +
                    $"{item.MetaData.ModifiedDate.ToShortTimeString()}";
                if (GameControlMode == GameControlMode.EditMap) mapItem.Play.Visibility = Visibility.Collapsed;
                else
                {
                    mapItem.Edit.Visibility = Visibility.Collapsed;
                    mapItem.Remove.Visibility = Visibility.Collapsed;
                }

                MapPanel.Children.Add(mapItem);
            }
        }

        public void LoadMap(MapItem mapItem)
        {
            var dbObject = DbObjects.Find(dbo => dbo.MetaData.Guid == mapItem.Guid);
            ((ContentControl) Parent).Content = new GameControl(dbObject, GameControlMode);
        }

        public void RemoveMap(MapItem mapItem)
        {
            MapPanel.Children.Remove(mapItem);
            var dbObject = DbObjects.Find(dbo => dbo.MetaData.Guid == mapItem.Guid);
            DbManager.RemoveDbObject(dbObject);
        }

        private void ControlClickHandler(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button button) || button.Name != "Back") return;
            switch (GameControlMode)
            {
                case GameControlMode.EditMap:
                    ((ContentControl) Parent).Content = new EditorMenu();
                    break;
                case GameControlMode.PlayMap:
                    ((ContentControl) Parent).Content = new GameMenu();
                    break;
            }
        }
    }
}