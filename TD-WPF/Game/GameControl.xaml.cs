using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TD_WPF.Game.GameObjects;
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
            Loaded += initialize;
        }

        private void initialize(object sender, RoutedEventArgs e)
        {
            createRowsForControls();
            createControls();

            gameCreator = new GameCreator(this);
            gameCreator.initilizeRandomPath();
            gameCreator.initializeRandomWaves();
            gameManager = new GameManager();
            Dispatcher.InvokeAsync(() => gameManager.run(this));
        }

        #region constants

        public const string HINT = "hint";
        public const string MOUSE_OVER = "over";

        #endregion

        #region attibutes

        public GameCreator gameCreator { get; set; }
        public GameManager gameManager { get; set; }
        public Control selectedControl { get; set; }
        public List<Mark> marks { get; set; } = new List<Mark>();
        public bool isEditor { get; set; } = false;

        #endregion

        #region control creation

        private void createRowsForControls()
        {
            ControlGrid.RowDefinitions.Clear();
            var p = ControlGrid.ActualWidth / 2 / ControlGrid.ActualHeight;
            var last = p;
            for (; last < 1; last += p)
            {
                var gridRow = new RowDefinition();
                gridRow.Height = new GridLength(p, GridUnitType.Star);
                ControlGrid.RowDefinitions.Add(gridRow);
            }

            if (last != 1)
            {
                var gridRow = new RowDefinition();
                gridRow.Height = new GridLength(1 - (last - p), GridUnitType.Star);
                ControlGrid.RowDefinitions.Add(gridRow);
            }
        }

        private void createControls()
        {
            var controls = isEditor
                ? ControlUtils.creatEditorConrtols(this)
                : ControlUtils.createGameControls(this);
            var current = controls.First;

            Action addControls = delegate
            {
                for (var row = 0; row < ControlGrid.RowDefinitions.Count; row++)
                for (var column = 0; column < ControlGrid.ColumnDefinitions.Count; column++)
                {
                    Grid.SetRow(current.Value, row);
                    Grid.SetColumn(current.Value, column);
                    ControlGrid.Children.Add(current.Value);
                    if (current.Next == null)
                        return;
                    current = current.Next;
                }
            };
            addControls();
        }

        #endregion

        #region control event handling

        public void HandleControlEvent(object sender, EventArgs a)
        {
            selectedControl = (Button) sender;
            removeHintMarks();
            createHintMarks();
        }

        #region hint methods

        private void removeHintMarks()
        {
            for (var i = marks.Count - 1; i >= 0; i--)
                if (marks[i].code.Equals(HINT))
                {
                    Canvas.Children.Remove(marks[i].shape);
                    marks.RemoveAt(i);
                }
        }

        private void createHintMarks()
        {
            if (isEditor)
            {
                if ((selectedControl.Name.Equals("weg") || selectedControl.Name.Equals("ziel"))
                    && gameCreator.paths.Count > 0 && gameCreator.paths.Last.Value.GetType() != typeof(Base))
                {
                    var list = GameUtils.GameUtils.PossiblePaths(GameUtils.GameUtils.NextPaths(gameCreator.x,
                            gameCreator.y,
                            (float) Canvas.ActualWidth / gameCreator.x, (float) Canvas.ActualHeight / gameCreator.y,
                            gameCreator.paths.Last.Value.x, gameCreator.paths.Last.Value.y), null, gameCreator.paths,
                        new LinkedList<Path>(gameCreator.ground.Cast<Path>()));

                    foreach (var item in list)
                    {
                        var mark = new Mark(item.x, item.y, item.width, item.height, Color.FromArgb(150, 124, 252, 0),
                            HINT);
                        mark.start(this);
                        marks.Add(mark);
                    }
                }
            }
            else
            {
                if (!selectedControl.Name.Equals("ground") && gameCreator.ground.Count > 0)
                    foreach (var item in gameCreator.ground)
                        if (item.tower == null)
                        {
                            var mark = new Mark(item.x, item.y, item.width, item.height,
                                Color.FromArgb(150, 124, 252, 0), HINT);
                            mark.start(this);
                            marks.Add(mark);
                        }
            }
        }

        #endregion

        #endregion

        #region mouse event handling

        private void MouseMoveOverCanvas(object sender, MouseEventArgs e)
        {
            if (Canvas.IsMouseOver && (isEditor && selectedControl == null || isEditor && selectedControl != null
                                                                                       && selectedControl.Name.Equals(
                                                                                           "ground") ||
                                       isEditor && selectedControl != null
                                                && selectedControl.Name.Equals("spawn") && gameCreator.paths.Count == 0
                                       || !isEditor && selectedControl == null))
            {
                var p = e.GetPosition(Canvas);
                var _x = (float) Math.Floor(p.X / (Canvas.ActualWidth / gameCreator.x));
                var _y = (float) Math.Floor(p.Y / (Canvas.ActualHeight / gameCreator.y));

                var color = Color.FromArgb(150, 124, 252, 0);
                Action work = delegate
                {
                    foreach (var list in new List<LinkedList<Path>>
                    {
                        gameCreator.paths,
                        new LinkedList<Path>(gameCreator.ground.Cast<Path>())
                    })
                    foreach (var item in list)
                        if (_x == item.x && _y == item.y)
                        {
                            color = Color.FromArgb(150, 205, 92, 92);
                            return;
                        }
                };
                work();


                var mark = marks.Find(m => m.code.Equals(MOUSE_OVER));
                if (mark == null)
                {
                    mark = new Mark(_x, _y, (float) Canvas.ActualWidth / gameCreator.x,
                        (float) Canvas.ActualHeight / gameCreator.y, color, MOUSE_OVER);
                    mark.start(this);
                    marks.Add(mark);
                }
                else
                {
                    mark.x = _x;
                    mark.y = _y;
                    mark.color = color;
                }
            }
            else
            {
                var mark = marks.Find(m => m.code.Equals(MOUSE_OVER));
                if (mark != null)
                {
                    marks.Remove(mark);
                    Canvas.Children.Remove(mark.shape);
                }
            }
        }

        private void MouseClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (Canvas.IsMouseOver)
            {
                var p = e.GetPosition(Canvas);
                var _x = (float) Math.Floor(p.X / (Canvas.ActualWidth / gameCreator.x));
                var _y = (float) Math.Floor(p.Y / (Canvas.ActualHeight / gameCreator.y));

                var isEmptySpace = true;
                Action work = delegate
                {
                    foreach (var list in new List<LinkedList<Path>>
                    {
                        gameCreator.paths,
                        new LinkedList<Path>(gameCreator.ground.Cast<Path>())
                    })
                    foreach (var item in list)
                        if (_x == item.x && _y == item.y)
                        {
                            isEmptySpace = false;
                            return;
                        }
                };
                work();

                GameObject hint = null;
                foreach (var item in marks)
                    if (item.code.Equals(HINT) && _x == item.x && _y == item.y)
                    {
                        hint = item;
                        break;
                    }

                if (isEditor && selectedControl != null && isEmptySpace)
                {
                    if (selectedControl.Name.Equals("weg") && gameCreator.paths.Count > 0
                                                           && !(gameCreator.paths.Last.Value.GetType() == typeof(Base))
                                                           && hint != null)
                    {
                        var path = new Path(hint.x, hint.y, hint.width, hint.height);
                        path.start(this);
                        gameCreator.paths.AddLast(path);
                        removeHintMarks();
                        createHintMarks();
                    }
                    else if (selectedControl.Name.Equals("spawn")
                             && gameCreator.paths.Count == 0)
                    {
                        var spawn = new Spawn(_x, _y, (float) Canvas.ActualWidth / gameCreator.x,
                            (float) Canvas.ActualHeight / gameCreator.y);
                        spawn.start(this);
                        gameCreator.paths.AddLast(spawn);
                    }
                    else if (selectedControl.Name.Equals("ziel") && gameCreator.paths.Count > 0
                                                                 && gameCreator.paths.Last.Value.GetType() ==
                                                                 typeof(Path) && hint != null)
                    {
                        var basePoint = new Base(hint.x, hint.y, hint.width, hint.height);
                        basePoint.start(this);
                        gameCreator.paths.AddLast(basePoint);
                        removeHintMarks();
                    }
                    else if (selectedControl.Name.Equals("ground"))
                    {
                        var ground = new Ground(_x, _y, (float) Canvas.ActualWidth / gameCreator.x,
                            (float) Canvas.ActualHeight / gameCreator.y);
                        ground.start(this);
                        gameCreator.ground.AddLast(ground);
                    }
                }
                else if (!isEditor && selectedControl != null)
                {
                    // TODO: buy items before we can build it

                    if (selectedControl.Name.Equals("ground") && isEmptySpace)
                    {
                        var ground = new Ground(_x, _y, (float) Canvas.ActualWidth / gameCreator.x,
                            (float) Canvas.ActualHeight / gameCreator.y);
                        ground.start(this);
                        gameCreator.ground.AddLast(ground);
                    }
                    else if (hint != null && hint != null)
                    {
                        var ground = gameCreator.ground.First(g => g.x == hint.x && g.y == hint.y);
                        if (ground != null)
                        {
                            var tower = new Tower(ground.x, ground.y, ground.width, ground.height, 2F, 5F, 5);
                            tower.start(this);
                            ground.tower = tower;
                        }

                        removeHintMarks();
                        createHintMarks();
                    }
                }
            }
        }

        #endregion
    }
}