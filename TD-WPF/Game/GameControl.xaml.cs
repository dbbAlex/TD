using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TD_WPF.Game.Enumerations;
using TD_WPF.Game.Manager;
using TD_WPF.Game.Objects;
using TD_WPF.Game.Objects.DynamicGameObjects;
using TD_WPF.Game.Objects.StaticGameObjects;
using TD_WPF.Game.Save;
using TD_WPF.Game.Utils;
using TD_WPF.Menu;

namespace TD_WPF.Game
{
    public partial class GameControl : UserControl
    {
        public GameControl()
        {
            InitializeComponent();
            Loaded += Initialize;
            IsEditor = false;
            IsRandom = true;
        }

        public GameControl(SaveObject saveObject)
        {
            InitializeComponent();
            Loaded += Initialize;
            SaveObject = saveObject;
            StartMode = false;
        }

        #region constants

        private const string Hint = "hint";
        private const string Info = "info";

        private void Initialize(object sender, RoutedEventArgs e)
        {
            GameCreator = new GameCreator(this);
            GameManager = new GameManager();
            if (IsRandom)
                GameCreator.InitilizeRandomGame();
            CreateRowsForControls();
            CreateControls();

            Dispatcher.InvokeAsync(() => GameManager.Run(this));
            LoadFromSaveObject();
        }

        private SaveObject CreateOrUpdateSaveObject()
        {
            if (SaveObject == null) SaveObject = new SaveObject();

            GameCreator.Paths.ForEach(path => path.Destroy(this));
            SaveObject.Paths = GameCreator.Paths;
            GameCreator.Paths.ForEach(ground => ground.Destroy(this));
            SaveObject.Ground = GameCreator.Ground;
            // TODO: health and money is null
            SaveObject.Health = GameCreator.Health;
            SaveObject.Money = GameCreator.Money;

            return SaveObject;
        }

        private void LoadFromSaveObject()
        {
            if (SaveObject == null) return;

            GameCreator.Health = SaveObject.Health;
            GameCreator.Money = SaveObject.Money;
            InfoManager.UpdateHealth(this);
            InfoManager.UpdateMoney(this);

            if (SaveObject.Paths != null)
            {
                GameCreator.Paths.Clear();
                for (var i = 0; i < SaveObject.Paths.Count; i++)
                {
                    var path = SaveObject.Paths[i];
                    GameCreator.Paths.Add(path);
                    GameCreator.Paths[i].Start(this);
                }
            }

            if (SaveObject.Ground != null)
            {
                GameCreator.Ground.Clear();
                for (var i = 0; i < SaveObject.Ground.Count; i++)
                {
                    var ground = SaveObject.Ground[i];
                    GameCreator.Ground.Add(ground);
                    GameCreator.Ground[i].Start(this);
                }
            }

            if (SaveObject.Waves != null && !IsEditor) GameCreator.Waves = SaveObject.Waves;
        }

        private const string MouseOver = "over";

        #endregion

        #region attibutes

        private SaveObject SaveObject { get; set; }
        public GameCreator GameCreator { get; private set; }
        public GameManager GameManager { get; set; }
        public Control SelectedControl { get; set; }
        private Ground SelectedObject { get; set; }
        public List<Mark> Marks { get; } = new List<Mark>();
        public List<Shot> Shots { get; } = new List<Shot>();
        public bool IsEditor { get; set; } = !false;
        private bool IsRandom { get; }
        private readonly bool StartMode = true;

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
            var infoPanel =
                IsEditor ? ControlUtils.CreateEditorInfoPanel(this) : ControlUtils.CreateGameInfoPanel(this);
            Grid.SetRow(infoPanel, 0);
            Grid.SetColumnSpan(infoPanel, ControlGrid.ColumnDefinitions.Count);
            ControlGrid.Children.Add(infoPanel);
            ControlGrid.RegisterName(infoPanel.Name, infoPanel);

            // controls
            var controls = IsEditor
                ? ControlUtils.CreatEditorConrtols(this)
                : ControlUtils.CreateGameControls(this);
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

            if (!IsEditor)
            {
                // object info panel
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
                    // TODO: go back to menu
                    if (StartMode && IsRandom)
                    {
                        ((ContentControl) Parent).Content = new GameMenu();
                    }
                    else if (StartMode && !IsRandom)
                    {
                        // map chooser game 
                    }
                    else if (!StartMode && !IsEditor)
                    {
                        // editor map chooser
                    }
                    else
                    {
                        ((ContentControl) Parent).Content = new EditorMenu();
                    }

                    break;
                case ControlUtils.Next:
                    if (GameCreator.Paths.Count > 0 &&
                        GameCreator.Paths[GameCreator.Paths.Count - 1].GetType() == typeof(Base))
                        ((ContentControl) Parent).Content = new WaveCreatorControl(CreateOrUpdateSaveObject());
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
            if (IsEditor)
            {
                if (SelectedControl == null) return;
                if ((SelectedControl.Name.Equals("Path") || SelectedControl.Name.Equals("End")) &&
                    GameCreator.Paths.Count > 0 &&
                    GameCreator.Paths[GameCreator.Paths.Count - 1].GetType() != typeof(Base))
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
                if (GameCreator.Ground.Count <= 0) return;
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
            if (Canvas.IsMouseOver && (IsEditor && SelectedControl == null || IsEditor && SelectedControl != null
                                                                                       && SelectedControl.Name.Equals(
                                                                                           "Ground") ||
                                       IsEditor && SelectedControl != null
                                                && SelectedControl.Name.Equals("Spawn") && GameCreator.Paths.Count == 0
                                       || !IsEditor && SelectedControl != null &&
                                       SelectedControl.Name.Equals("Ground")))
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

                    if (item.Code.Equals(Info))
                    {
                        info = item;
                        break;
                    }
                }

            if (info != null)
                foreach (var item in GameCreator.Ground)
                    if (item.X == info.X && item.Y == info.Y)
                    {
                        InfoManager.UpdateObjectInfoPanelByGameObject(this,
                            item.Tower != null ? (GameObject) item.Tower : item);
                        SelectedObject = item;
                    }

            if (IsEditor && SelectedControl != null && isEmptySpace)
            {
                if (SelectedControl.Name.Equals("Path") && GameCreator.Paths.Count > 0
                                                        && !(GameCreator.Paths[GameCreator.Paths.Count - 1].GetType() ==
                                                             typeof(Base))
                                                        && hint != null)
                {
                    var path = new Path(hint.X, hint.Y, hint.Width, hint.Height, GameCreator.Paths.Count);
                    path.Start(this);
                    GameCreator.Paths.Add(path);
                    RemoveHintMarks();
                    CreateHintMarks();
                }
                else if (SelectedControl.Name.Equals("Spawn")
                         && GameCreator.Paths.Count == 0)
                {
                    var spawn = new Spawn(x, y, Canvas.ActualWidth / GameCreator.X,
                        Canvas.ActualHeight / GameCreator.Y, 0);
                    spawn.Start(this);
                    GameCreator.Paths.Add(spawn);
                }
                else if (SelectedControl.Name.Equals("End") && GameCreator.Paths.Count > 0
                                                            && GameCreator.Paths[GameCreator.Paths.Count - 1]
                                                                .GetType() ==
                                                            typeof(Path) && hint != null)
                {
                    var basePoint = new Base(hint.X, hint.Y, hint.Width, hint.Height, GameCreator.Paths.Count);
                    basePoint.Start(this);
                    GameCreator.Paths.Add(basePoint);
                    RemoveHintMarks();
                }
                else if (SelectedControl.Name.Equals("Ground"))
                {
                    var ground = new Ground(x, y, Canvas.ActualWidth / GameCreator.X,
                        Canvas.ActualHeight / GameCreator.Y, GameCreator.Ground.Count);
                    ground.Start(this);
                    GameCreator.Ground.Add(ground);
                }
            }
            else if (IsEditor && SelectedControl != null && !isEmptySpace)
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
            else if (!IsEditor && SelectedControl != null)
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