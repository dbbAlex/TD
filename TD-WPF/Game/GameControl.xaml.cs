using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Manager;
using TD_WPF.Game.Objects;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Game.Objects.StaticGameObjects;
using TD_WPF.Game.Save;
using TD_WPF.Game.Utils;
using TD_WPF.Menu;
using TD_WPF.Properties;
using Color = System.Windows.Media.Color;
using Path = TD_WPF.Game.Objects.StaticGameObjects.Path;
using Size = System.Windows.Size;

namespace TD_WPF.Game
{
    public partial class GameControl : UserControl
    {
        public GameControl(DbObject dbObject, GameControlMode gameControlMode)
        {
            InitializeComponent();
            Loaded += Initialize;
            DbObject = dbObject;
            GameControlMode = gameControlMode;
        }

        #region constants

        private const string Hint = "hint";
        private const string Info = "info";
        private const string MouseOver = "over";

        #endregion

        #region attibutes

        private DbObject DbObject { get; set; }
        public GameCreator GameCreator { get; private set; }
        public GameManager GameManager { get; set; }
        public Control SelectedControl { get; set; }
        private Ground SelectedObject { get; set; }
        public List<Mark> Marks { get; } = new List<Mark>();
        public List<Shot> Shots { get; } = new List<Shot>();
        public GameControlMode GameControlMode { get; set; }

        #endregion

        private void Initialize(object sender, RoutedEventArgs e)
        {
            GameCreator = new GameCreator(this);
            GameManager = new GameManager();
            switch (GameControlMode)
            {
                case GameControlMode.PlayRandom:
                    GameCreator.InitilizeRandomGame();
                    break;
                case GameControlMode.PlayMap:
                case GameControlMode.EditMap:
                    LoadFromDbObject();
                    break;
            }

            CreateRowsForControls();
            CreateControls();

            Dispatcher.InvokeAsync(() => GameManager.Run(this));
        }

        #region DbObject methods

        private void LoadFromDbObject()
        {
            if (DbObject == null) return;

            GameCreator.Health = DbObject.GameData.Health;
            GameCreator.Money = DbObject.GameData.Money;
            InfoManager.UpdateHealth(this);
            InfoManager.UpdateMoney(this);

            if (DbObject.GameData.Paths != null)
            {
                GameCreator.Paths.Clear();
                for (var i = 0; i < DbObject.GameData.Paths.Count; i++)
                {
                    var path = DbObject.GameData.Paths[i];
                    GameCreator.Paths.Add(path);
                    GameCreator.Paths[i].Start(this);
                }
            }

            if (DbObject.GameData.Ground != null)
            {
                GameCreator.Ground.Clear();
                for (var i = 0; i < DbObject.GameData.Ground.Count; i++)
                {
                    var ground = DbObject.GameData.Ground[i];
                    GameCreator.Ground.Add(ground);
                    GameCreator.Ground[i].Start(this);
                }
            }

            if (DbObject.GameData.Waves == null || GameControlMode == GameControlMode.EditMap) return;
            GameCreator.Waves = DbObject.GameData.Waves;
            GameCreator.Waves.Start(this, GameManager.Timer.ElapsedMilliseconds);
        }

        private DbObject CreateOrUpdateDbObject()
        {
            if (DbObject == null)
                DbObject = new DbObject
                {
                    MetaData = new MetaData(),
                    GameData = new GameData()
                };

            GameCreator.Paths.ForEach(path => path.Destroy(this));
            DbObject.GameData.Paths = GameCreator.Paths;
            GameCreator.Paths.ForEach(ground => ground.Destroy(this));
            DbObject.GameData.Ground = GameCreator.Ground;
            DbObject.GameData.Health = GameCreator.Health;
            DbObject.GameData.Money = GameCreator.Money;

            DbObject.MetaData.Thumbnail = CreateThumbnailFromCanvas();

            return DbObject;
        }

        private Bitmap CreateThumbnailFromCanvas()
        {
            var bmp = new Bitmap(Convert.ToInt32(Math.Ceiling(Canvas.ActualWidth)),
                Convert.ToInt32(Math.Ceiling(Canvas.ActualHeight)));
            var g = Graphics.FromImage(bmp);

            // draw background
            g.DrawImage(Resource.grass, 0, 0);


            //Iterate through list and draw on bitmap
            var lists = new List<List<Path>>() {new List<Path>(GameCreator.Ground), GameCreator.Paths};
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    var resizeImage = ImageUtil.ResizeImage(item.Image, Convert.ToInt32(Math.Ceiling(item.Width)),
                        Convert.ToInt32(Math.Ceiling(item.Height)));
                    g.DrawImage(resizeImage, Convert.ToInt32(item.X * item.Width),
                        Convert.ToInt32(item.Y * item.Height));
                }
            }

            return ImageUtil.ResizeImage(bmp, 200, 150);
        }

        #endregion

        #region control creation

        private void CreateRowsForControls()
        {
            ControlGrid.RowDefinitions.Clear();
            var p = ControlGrid.ActualWidth / 2 / ControlGrid.ActualHeight;
            var last = p;
            RowDefinition gridRow;
            for (; last < 1; last += p)
            {
                gridRow = new RowDefinition {Height = new GridLength(p, GridUnitType.Star)};
                ControlGrid.RowDefinitions.Add(gridRow);
            }

            if (last == 1) return;

            gridRow = new RowDefinition {Height = new GridLength(1 - (last - p), GridUnitType.Star)};
            ControlGrid.RowDefinitions.Add(gridRow);
        }

        private void CreateControls()
        {
            // info panel
            var infoPanel = GameControlMode == GameControlMode.CreateMap || GameControlMode == GameControlMode.EditMap
                ? ControlUtils.CreateEditorInfoPanel(this)
                : ControlUtils.CreateGameInfoPanel(this);
            Grid.SetRow(infoPanel, 0);
            Grid.SetColumnSpan(infoPanel, ControlGrid.ColumnDefinitions.Count);
            ControlGrid.Children.Add(infoPanel);
            ControlGrid.RegisterName(infoPanel.Name, infoPanel);

            // controls
            var controls = ControlUtils.CreateControls(this);
            var rows = Convert.ToInt32(Math.Ceiling(controls.Count / 2d));
            var index = 0;

            void AddControls()
            {
                for (var row = 1; row <= rows; row++)
                for (var column = 0; column < ControlGrid.ColumnDefinitions.Count; column++)
                {
                    Grid.SetRow(controls[index], row);
                    Grid.SetColumn(controls[index], column);
                    ControlGrid.Children.Add(controls[index]);
                    index++;
                    if (index == controls.Count) return;
                }
            }

            AddControls();
            rows++;

            if (GameControlMode == GameControlMode.PlayRandom || GameControlMode == GameControlMode.PlayMap)
            {
                var objectInfoPanel = ControlUtils.CreateObjectInfoPanel(this);
                Grid.SetRow(objectInfoPanel, rows);
                Grid.SetColumnSpan(objectInfoPanel, ControlGrid.ColumnDefinitions.Count);
                ControlGrid.Children.Add(objectInfoPanel);
                ControlGrid.RegisterName(objectInfoPanel.Name, objectInfoPanel);
            }

            var buttonGrid = ControlUtils.CreateButtons(this);
            Grid.SetRow(buttonGrid, ControlGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(buttonGrid, ControlGrid.ColumnDefinitions.Count);
            ControlGrid.Children.Add(buttonGrid);
            ControlGrid.RegisterName(buttonGrid.Name, buttonGrid);
        }

        #endregion

        #region control event handling

        public void HandleObjectInfoEvent(object sender, EventArgs a)
        {
            if (!(sender is Button button) || GameManager.Pause) return;
            switch (button.Name)
            {
                case ControlUtils.DamageButton:
                    MoneyManager.UpdateTower(SelectedObject.Tower, TowerUpdateSelection.Damage, this);
                    break;
                case ControlUtils.RangeButton:
                    MoneyManager.UpdateTower(SelectedObject.Tower, TowerUpdateSelection.Range, this);
                    break;
                case ControlUtils.ObjectMoneyButton:
                    MoneyManager.SellObject(this, SelectedObject);
                    break;
            }
        }

        public void HandleControlEvent(object sender, EventArgs a)
        {
            SelectedControl = (Button) sender;
            RemoveHintMarks();
            CreateHintMarks();
            InfoManager.UpdateObjectInfoPanelByControl(this, SelectedControl);
        }

        public void HandleButtonEvent(object sender, EventArgs a)
        {
            if (!(sender is Button button)) return;
            switch (button.Name)
            {
                case ControlUtils.Pause:
                    if (!GameManager.End)
                        GameManager.Pause = !GameManager.Pause;
                    break;
                case ControlUtils.Cancel:
                    switch (GameControlMode)
                    {
                        case GameControlMode.CreateMap:
                            ((ContentControl) Parent).Content = new EditorMenu();
                            break;
                        case GameControlMode.PlayRandom:
                            ((ContentControl) Parent).Content = new GameMenu();
                            break;
                        case GameControlMode.EditMap:
                        case GameControlMode.PlayMap:
                            ((ContentControl) Parent).Content = new MapMenu(GameControlMode);
                            break;
                    }

                    GameManager.EndLoop();
                    break;
                case ControlUtils.Next:
                    if (GameCreator.Paths.Count > 0 &&
                        GameCreator.Paths[GameCreator.Paths.Count - 1].PathIdentifier == PathIdentifier.Base)
                        ((ContentControl) Parent).Content =
                            new WaveCreatorControl(CreateOrUpdateDbObject(), GameControlMode);
                    GameManager.EndLoop();
                    break;
            }
        }

        #region hint methods

        public void RemoveHintMarks()
        {
            for (var i = Marks.Count - 1; i >= 0; i--)
                if (Marks[i].Code.Equals(Hint) || Marks[i].Code.Equals(Info))
                {
                    Canvas.Children.Remove(Marks[i].Shape);
                    Marks.RemoveAt(i);
                }
        }

        public void CreateHintMarks()
        {
            if (GameControlMode == GameControlMode.CreateMap || GameControlMode == GameControlMode.EditMap)
            {
                if (SelectedControl == null) return;
                if ((SelectedControl.Name.Equals("Path") || SelectedControl.Name.Equals("End")) &&
                    GameCreator.Paths.Count > 0 &&
                    GameCreator.Paths[GameCreator.Paths.Count - 1].PathIdentifier != PathIdentifier.Base)
                {
                    var list = GameUtils.PossiblePaths(GameUtils.NextPaths(GameCreator.X,
                            GameCreator.Y,
                            Canvas.ActualWidth / GameCreator.X, Canvas.ActualHeight / GameCreator.Y,
                            GameCreator.Paths[GameCreator.Paths.Count - 1].X,
                            GameCreator.Paths[GameCreator.Paths.Count - 1].Y, 0), null, GameCreator.Paths,
                        new List<Path>(GameCreator.Ground));

                    foreach (var item in list)
                    {
                        var mark = new Mark(item.X, item.Y, item.Width, item.Height, Color.FromArgb(150, 124, 252, 0),
                            Hint);
                        mark.Start(this);
                        Marks.Add(mark);
                    }
                }
                else if (SelectedControl.Name.Equals("Remove"))
                {
                    var lists =
                        new List<List<Path>> {GameCreator.Paths, new List<Path>(GameCreator.Ground)};
                    foreach (var list in lists)
                    foreach (var item in list)
                    {
                        var mark = new Mark(item.X, item.Y, item.Width, item.Height,
                            Color.FromArgb(150, 124, 252, 0),
                            Hint);
                        mark.Start(this);
                        Marks.Add(mark);
                    }
                }
            }
            else
            {
                if (GameCreator.Ground.Count <= 0 || SelectedControl == null) return;
                if (SelectedControl.Name.Equals("Ground"))
                    foreach (var item in GameCreator.Ground)
                    {
                        var mark = new Mark(item.X, item.Y, item.Width, item.Height,
                            Color.FromArgb(57, 247, 255, 16), Info);
                        mark.Start(this);
                        Marks.Add(mark);
                    }

                if (SelectedControl.Name.Equals("Tower"))
                    foreach (var item in GameCreator.Ground)
                        if (item.Tower == null)
                        {
                            var mark = new Mark(item.X, item.Y, item.Width, item.Height,
                                Color.FromArgb(150, 124, 252, 0), Hint);
                            mark.Start(this);
                            Marks.Add(mark);
                        }
                        else
                        {
                            var mark = new Mark(item.X, item.Y, item.Width, item.Height,
                                Color.FromArgb(57, 247, 255, 16), Info);
                            mark.Start(this);
                            Marks.Add(mark);
                        }
            }
        }

        #endregion

        #endregion

        #region editor control handling

        public void ControlPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public void ControlLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is TextBox textBox)) return;
            switch (textBox.Name)
            {
                case ControlUtils.HealthValue:
                    if (string.IsNullOrWhiteSpace(textBox.Text) || long.Parse(textBox.Text) <= 0)
                    {
                        GameCreator.Health = 100;
                        textBox.Text = 100.ToString();
                    }
                    else
                    {
                        GameCreator.Health = int.Parse(textBox.Text);
                    }

                    break;
                case ControlUtils.MoneyValue:
                    if (string.IsNullOrWhiteSpace(textBox.Text) || long.Parse(textBox.Text) <= 0)
                    {
                        GameCreator.Money = 100;
                        textBox.Text = 100.ToString();
                    }
                    else
                    {
                        GameCreator.Money = int.Parse(textBox.Text);
                    }

                    break;
            }
        }

        #endregion

        #region mouse event handling

        private void MouseMoveOverCanvas(object sender, MouseEventArgs e)
        {
            if (Canvas.IsMouseOver &&
                ((GameControlMode == GameControlMode.CreateMap || GameControlMode == GameControlMode.EditMap)
                 && SelectedControl != null && SelectedControl.Name.Equals("Spawn") && GameCreator.Paths.Count == 0)
                || (SelectedControl != null && SelectedControl.Name.Equals("Ground"))
                || SelectedControl == null)
            {
                var p = e.GetPosition(Canvas);
                var x = Math.Floor(p.X / (Canvas.ActualWidth / GameCreator.X));
                var y = Math.Floor(p.Y / (Canvas.ActualHeight / GameCreator.Y));

                var color = Color.FromArgb(150, 124, 252, 0);

                void Work()
                {
                    foreach (var list in new List<List<Path>>
                    {
                        GameCreator.Paths,
                        new List<Path>(GameCreator.Ground)
                    })
                    foreach (var item in list)
                        if (x == item.X && y == item.Y)
                        {
                            color = Color.FromArgb(150, 205, 92, 92);
                            return;
                        }
                }

                Work();


                var mark = Marks.Find(m => m.Code.Equals(MouseOver));
                if (mark == null)
                {
                    mark = new Mark(x, y, Canvas.ActualWidth / GameCreator.X,
                        Canvas.ActualHeight / GameCreator.Y, color, MouseOver);
                    mark.Start(this);
                    Marks.Add(mark);
                }
                else
                {
                    mark.X = x;
                    mark.Y = y;
                    mark.Color = color;
                }
            }
            else
            {
                var mark = Marks.Find(m => m.Code.Equals(MouseOver));
                if (mark == null) return;
                Marks.Remove(mark);
                Canvas.Children.Remove(mark.Shape);
            }
        }

        private void MouseClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (!Canvas.IsMouseOver) return;
            var p = e.GetPosition(Canvas);
            var x = Math.Floor(p.X / (Canvas.ActualWidth / GameCreator.X));
            var y = Math.Floor(p.Y / (Canvas.ActualHeight / GameCreator.Y));

            var isEmptySpace = true;

            void EvaluateIsEmptySpace()
            {
                foreach (var list in new List<List<Path>>
                {
                    GameCreator.Paths,
                    new List<Path>(GameCreator.Ground)
                })
                foreach (var item in list)
                    if (x == item.X && y == item.Y)
                    {
                        isEmptySpace = false;
                        return;
                    }
            }

            EvaluateIsEmptySpace();

            GameObject hint = null;
            GameObject info = null;
            foreach (var item in Marks)
                if (x == item.X && y == item.Y)
                {
                    if (item.Code.Equals(Hint))
                    {
                        hint = item;
                        break;
                    }

                    if (!item.Code.Equals(Info)) continue;
                    info = item;
                    break;
                }

            if (info != null)
                foreach (var item in GameCreator.Ground)
                    if (item.X == info.X && item.Y == info.Y)
                    {
                        InfoManager.UpdateObjectInfoPanelByGameObject(this,
                            item.Tower != null ? (GameObject) item.Tower : item);
                        SelectedObject = item;
                    }

            if ((GameControlMode == GameControlMode.CreateMap || GameControlMode == GameControlMode.EditMap) &&
                SelectedControl != null && isEmptySpace)
            {
                if (SelectedControl.Name.Equals("Path") && GameCreator.Paths.Count > 0
                                                        && GameCreator.Paths[GameCreator.Paths.Count - 1]
                                                            .PathIdentifier != PathIdentifier.Base
                                                        && hint != null)
                {
                    var path = new Path(hint.X, hint.Y, hint.Width, hint.Height, GameCreator.Paths.Count,
                        PathIdentifier.Path);
                    path.Start(this);
                    GameCreator.Paths.Add(path);
                    RemoveHintMarks();
                    CreateHintMarks();
                }
                else if (SelectedControl.Name.Equals("Spawn")
                         && GameCreator.Paths.Count == 0)
                {
                    var spawn = new Path(x, y, Canvas.ActualWidth / GameCreator.X,
                        Canvas.ActualHeight / GameCreator.Y, 0, PathIdentifier.Spawn);
                    spawn.Start(this);
                    GameCreator.Paths.Add(spawn);
                }
                else if (SelectedControl.Name.Equals("End") && GameCreator.Paths.Count > 0
                                                            && GameCreator.Paths[GameCreator.Paths.Count - 1]
                                                                .GetType() ==
                                                            typeof(Path) && hint != null)
                {
                    var basePoint = new Path(hint.X, hint.Y, hint.Width, hint.Height, GameCreator.Paths.Count,
                        PathIdentifier.Base);
                    basePoint.Start(this);
                    GameCreator.Paths.Add(basePoint);
                    RemoveHintMarks();
                }
                else if (SelectedControl.Name.Equals("Ground"))
                {
                    var ground = new Ground(x, y, Canvas.ActualWidth / GameCreator.X,
                        Canvas.ActualHeight / GameCreator.Y, GameCreator.Ground.Count, PathIdentifier.Ground);
                    ground.Start(this);
                    GameCreator.Ground.Add(ground);
                }
            }
            else if ((GameControlMode == GameControlMode.CreateMap || GameControlMode == GameControlMode.EditMap) &&
                     SelectedControl != null && !isEmptySpace)
            {
                if (SelectedControl.Name.Equals("Remove") && hint != null)
                {
                    var path = GameCreator.Paths.FirstOrDefault(item => item.X == hint.X && item.Y == hint.Y);
                    var ground = GameCreator.Ground.FirstOrDefault(item => item.X == hint.X && item.Y == hint.Y);

                    if (path != null && ground == null)
                    {
                        var indexOf = GameCreator.Paths.IndexOf(path);
                        for (var i = GameCreator.Paths.Count - 1; i >= indexOf; i--)
                        {
                            var item = GameCreator.Paths[i];
                            item.Destroy(this);
                            GameCreator.Paths.Remove(item);
                        }
                    }
                    else if ((path == null) & (ground != null))
                    {
                        ground.Destroy(this);
                        GameCreator.Ground.Remove(ground);
                    }
                }
            }
            else if ((GameControlMode == GameControlMode.PlayRandom || GameControlMode == GameControlMode.PlayMap) && SelectedControl != null)
            {
                if (SelectedControl.Name.Equals("Ground") && isEmptySpace)
                {
                    MoneyManager.BuildGround(this, x, y);
                }
                else if (hint != null)
                {
                    var ground = GameCreator.Ground.First(g => g.X == hint.X && g.Y == hint.Y);
                    if (ground != null) MoneyManager.BuildTower(ground, SelectedControl.Name, this);
                }
            }

            RemoveHintMarks();
            CreateHintMarks();
        }

        #endregion
    }
}