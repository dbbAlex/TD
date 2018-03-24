using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TD_WPF.Game.GameObjects;
using TD_WPF.Game.GameObjects.DynamicGameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;
using TD_WPF.Game.GameUtils;

namespace TD_WPF.Game
{
    /// <summary>
    ///     Interaktionslogik für GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        public GameControl()
        {
            InitializeComponent();
            Loaded += Initialize;
        }

        #region constants

        private const string Hint = "hint";

        private void Initialize(object sender, RoutedEventArgs e)
        {
            GameCreator = new GameCreator(this);
            GameManager = new GameManager();
            GameCreator.InitilizeRandomGame();
            CreateRowsForControls();
            CreateControls();
            
            Dispatcher.InvokeAsync(() => GameManager.Run(this));
        }

        private const string MouseOver = "over";

        #endregion

        #region attibutes

        public GameCreator GameCreator { get; private set; }
        public GameManager GameManager { get; set; }
        private Control SelectedControl { get; set; }
        public List<Mark> Marks { get; } = new List<Mark>();
        public List<Shot> Shots { get; } = new List<Shot>();
        private bool IsEditor { get; set; } = false;

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
            // info Panel
            var infoPanel = ControlUtils.CreateInoPanel(this);
            Grid.SetRow(infoPanel, 0);
            Grid.SetColumnSpan(infoPanel, 2);
            ControlGrid.Children.Add(infoPanel);
            ControlGrid.RegisterName(infoPanel.Name, infoPanel);
            
            // controls
            var controls = IsEditor
                ? ControlUtils.CreatEditorConrtols(this)
                : ControlUtils.CreateGameControls(this);
            var current = controls.First;

            void AddControls()
            {
                for (var row = 1; row < ControlGrid.RowDefinitions.Count; row++)
                for (var column = 0; column < ControlGrid.ColumnDefinitions.Count; column++)
                {
                    Grid.SetRow(current.Value, row);
                    Grid.SetColumn(current.Value, column);
                    ControlGrid.Children.Add(current.Value);
                    if (current.Next == null) return;
                    current = current.Next;
                }
            }

            AddControls();
        }

        #endregion

        #region control event handling

        public void HandleControlEvent(object sender, EventArgs a)
        {
            SelectedControl = (Button) sender;
            RemoveHintMarks();
            CreateHintMarks();
        }

        #region hint methods

        private void RemoveHintMarks()
        {
            for (var i = Marks.Count - 1; i >= 0; i--)
                if (Marks[i].Code.Equals(Hint))
                {
                    Canvas.Children.Remove(Marks[i].Shape);
                    Marks.RemoveAt(i);
                }
        }

        private void CreateHintMarks()
        {
            if (IsEditor)
            {
                if ((!SelectedControl.Name.Equals("weg") && !SelectedControl.Name.Equals("ziel")) ||
                    GameCreator.Paths.Count <= 0 ||
                    GameCreator.Paths[GameCreator.Paths.Count - 1].GetType() == typeof(Base)) return;
                var list = GameUtils.GameUtils.PossiblePaths(GameUtils.GameUtils.NextPaths(GameCreator.X,
                        GameCreator.Y,
                        (float) Canvas.ActualWidth / GameCreator.X, (float) Canvas.ActualHeight / GameCreator.Y,
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
            else
            {
                if (SelectedControl.Name.Equals("ground") || GameCreator.Ground.Count <= 0) return;
                foreach (var item in GameCreator.Ground)
                    if (item.Tower == null)
                    {
                        var mark = new Mark(item.X, item.Y, item.Width, item.Height,
                            Color.FromArgb(150, 124, 252, 0), Hint);
                        mark.Start(this);
                        Marks.Add(mark);
                    }
            }
        }

        #endregion

        #endregion

        #region mouse event handling

        private void MouseMoveOverCanvas(object sender, MouseEventArgs e)
        {
            if (Canvas.IsMouseOver && (IsEditor && SelectedControl == null || IsEditor && SelectedControl != null
                                                                                       && SelectedControl.Name.Equals(
                                                                                           "ground") ||
                                       IsEditor && SelectedControl != null
                                                && SelectedControl.Name.Equals("spawn") && GameCreator.Paths.Count == 0
                                       || !IsEditor && SelectedControl == null))
            {
                var p = e.GetPosition(Canvas);
                var x = (float) Math.Floor(p.X / (Canvas.ActualWidth / GameCreator.X));
                var y = (float) Math.Floor(p.Y / (Canvas.ActualHeight / GameCreator.Y));

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
                    mark = new Mark(x, y, (float) Canvas.ActualWidth / GameCreator.X,
                        (float) Canvas.ActualHeight / GameCreator.Y, color, MouseOver);
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
            var x = (float) Math.Floor(p.X / (Canvas.ActualWidth / GameCreator.X));
            var y = (float) Math.Floor(p.Y / (Canvas.ActualHeight / GameCreator.Y));

            var isEmptySpace = true;

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
                        isEmptySpace = false;
                        return;
                    }
            }

            Work();

            GameObject hint = null;
            foreach (var item in Marks)
                if (item.Code.Equals(Hint) && x == item.X && y == item.Y)
                {
                    hint = item;
                    break;
                }

            if (IsEditor && SelectedControl != null && isEmptySpace)
            {
                if (SelectedControl.Name.Equals("weg") && GameCreator.Paths.Count > 0
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
                else if (SelectedControl.Name.Equals("spawn")
                         && GameCreator.Paths.Count == 0)
                {
                    var spawn = new Spawn(x, y, (float) Canvas.ActualWidth / GameCreator.X,
                        (float) Canvas.ActualHeight / GameCreator.Y, 0);
                    spawn.Start(this);
                    GameCreator.Paths.Add(spawn);
                }
                else if (SelectedControl.Name.Equals("ziel") && GameCreator.Paths.Count > 0
                                                             && GameCreator.Paths[GameCreator.Paths.Count - 1]
                                                                 .GetType() ==
                                                             typeof(Path) && hint != null)
                {
                    var basePoint = new Base(hint.X, hint.Y, hint.Width, hint.Height, GameCreator.Paths.Count);
                    basePoint.Start(this);
                    GameCreator.Paths.Add(basePoint);
                    RemoveHintMarks();
                }
                else if (SelectedControl.Name.Equals("ground"))
                {
                    var ground = new Ground(x, y, (float) Canvas.ActualWidth / GameCreator.X,
                        (float) Canvas.ActualHeight / GameCreator.Y, GameCreator.Ground.Count);
                    ground.Start(this);
                    GameCreator.Ground.Add(ground);
                }
            }
            else if (!IsEditor && SelectedControl != null)
            {
                // TODO: buy items before we can build it

                if (SelectedControl.Name.Equals("ground") && isEmptySpace)
                {
                    var ground = new Ground(x, y, (float) Canvas.ActualWidth / GameCreator.X,
                        (float) Canvas.ActualHeight / GameCreator.Y, GameCreator.Ground.Count);
                    ground.Start(this);
                    GameCreator.Ground.Add(ground);
                }
                else if (hint != null)
                {
                    var ground = GameCreator.Ground.First(g => g.X == hint.X && g.Y == hint.Y);
                    if (ground != null)
                    {
                        var tower = new Tower(ground.X, ground.Y, ground.Width, ground.Height, 0.7f, 0.9f, 5,
                            ground.Width * 2);
                        tower.Start(this);
                        ground.Tower = tower;
                    }

                    RemoveHintMarks();
                    CreateHintMarks();
                }
            }
        }

        #endregion
    }
}