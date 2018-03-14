using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TD_WPF.Game.GameManagerTask;
using TD_WPF.Game.GameObjects;
using TD_WPF.Game.GameObjects.StaticGameObjects;
using TD_WPF.Game.GameUtils;

namespace TD_WPF.Game
{
    /// <summary>
    /// Interaktionslogik für GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        #region constants
        const string HINT = "hint";
        const string MOUSE_OVER = "over";
        #endregion

        #region attibutes
        public GameCreator gameCreator { get; set; }
        public GameManager gameManager { get; set; }
        public Control selectedControl { get; set; }
        public List<Mark> marks { get; set; } = new List<Mark>();
        public bool isEditor { get; set; } = !false;
        #endregion

        public GameControl()
        {
            InitializeComponent();
            Loaded += initialize;
        }

        private void initialize(object sender, RoutedEventArgs e)
        {
            createRowsForControls();
            createControls();

            this.gameCreator = new GameCreator(this);
            //this.gameCreator.initilizeRandomPath();
            this.gameManager = new GameManager();
            this.Dispatcher.InvokeAsync(() => this.gameManager.run(this));
        }

        #region control creation
        private void createRowsForControls()
        {
            this.ControlGrid.RowDefinitions.Clear();
            double p = this.ControlGrid.ActualWidth / 2 / this.ControlGrid.ActualHeight;
            double last = p;
            for (; last < 1; last += p)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(p, GridUnitType.Star);
                this.ControlGrid.RowDefinitions.Add(gridRow);
            }
            if (last != 1)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(1 - (last - p), GridUnitType.Star);
                this.ControlGrid.RowDefinitions.Add(gridRow);
            }
        }

        private void createControls()
        {
            LinkedList<ContentControl> controls = isEditor ? ControlUtils.creatEditorConrtols(this) 
                : ControlUtils.createGameControls(this);
            LinkedListNode<ContentControl> current = controls.First;

            Action addControls = delegate
            {
                for (int row = 0; row < this.ControlGrid.RowDefinitions.Count; row++)
                {
                    for (int column = 0; column < this.ControlGrid.ColumnDefinitions.Count; column++)
                    {
                        Grid.SetRow(current.Value, row);
                        Grid.SetColumn(current.Value, column);
                        this.ControlGrid.Children.Add(current.Value);
                        if (current.Next == null)
                            return;
                        current = current.Next;
                    }
                }
            };
            addControls();
        }
        #endregion

        #region control event handling
        public void HandleControlEvent(object sender, EventArgs a)
        {
            selectedControl = ((Button)sender);
            removeHintMarks();
            createHintMarks();
        }

        #region hint methods
        private void removeHintMarks()
        {
            for (int i = marks.Count - 1; i >= 0; i--)
            {
                if (marks[i].code.Equals(HINT))
                {
                    this.Canvas.Children.Remove(marks[i].shape);
                    marks.RemoveAt(i);
                }
            }
        }

        private void createHintMarks()
        {
            if (this.isEditor)
            {
                if ((this.selectedControl.Name.Equals("weg") || this.selectedControl.Name.Equals("ziel"))
                    && this.gameCreator.paths.Count > 0 && this.gameCreator.paths.Last.Value.GetType() != typeof(Base))
                {
                    List<Path> list = GameUtils.GameUtils.PossiblePaths(GameUtils.GameUtils.NextPaths(this.gameCreator.x, this.gameCreator.y,
                       (float)this.Canvas.ActualWidth / this.gameCreator.x, (float)this.Canvas.ActualHeight / this.gameCreator.y,
                       this.gameCreator.paths.Last.Value.x, this.gameCreator.paths.Last.Value.y), null, this.gameCreator.paths, new LinkedList<Path>(this.gameCreator.ground.Cast<Path>()));

                    foreach (var item in list)
                    {
                        Mark mark = new Mark(item.x, item.y, item.width, item.height, Color.FromArgb(150, 124, 252, 0), HINT);
                        mark.start(this);
                        marks.Add(mark);

                    }
                }
            }
            else
            {
                if (!this.selectedControl.Name.Equals("ground") && this.gameCreator.ground.Count > 0)
                {
                    foreach (var item in this.gameCreator.ground)
                    {
                        if (item.tower == null)
                        {
                            Mark mark = new Mark(item.x, item.y, item.width, item.height, Color.FromArgb(150, 124, 252, 0), HINT);
                            mark.start(this);
                            marks.Add(mark);
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region mouse event handling
        private void MouseMoveOverCanvas(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Canvas.IsMouseOver && (isEditor && this.selectedControl == null || isEditor && this.selectedControl != null 
                && this.selectedControl.Name.Equals("ground") || isEditor && this.selectedControl != null
                && this.selectedControl.Name.Equals("spawn") && this.gameCreator.paths.Count == 0 
                || !isEditor && this.selectedControl == null))
            {
                
                Point p = e.GetPosition(this.Canvas);
                float _x = (float)Math.Floor(p.X / (this.Canvas.ActualWidth /this.gameCreator.x));
                float _y = (float)Math.Floor(p.Y / (this.Canvas.ActualHeight / this.gameCreator.y));

                Color color = Color.FromArgb(150, 124, 252, 0);
                Action work = delegate
                {
                    foreach (var list in new List<LinkedList<Path>> { this.gameCreator.paths, new LinkedList<Path>(this.gameCreator.ground.Cast<Path>()) })
                    {
                        foreach (var item in list)
                        {
                            if (_x == item.x && _y == item.y)
                            {
                                color = Color.FromArgb(150, 205, 92, 92);
                                return;
                            }
                        }
                    }
                };
                work();
                

                Mark mark = marks.Find(m => m.code.Equals(MOUSE_OVER));
                if(mark == null)
                {
                    mark = new Mark(_x, _y, (float)this.Canvas.ActualWidth / this.gameCreator.x,
                        (float)this.Canvas.ActualHeight / this.gameCreator.y, color, MOUSE_OVER);
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
                Mark mark = marks.Find(m => m.code.Equals(MOUSE_OVER));
                if (mark != null)
                {
                    marks.Remove(mark);
                    this.Canvas.Children.Remove(mark.shape);
                }
            }
    }

        private void MouseClickOnCanvas(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.Canvas.IsMouseOver)
            {
                Point p = e.GetPosition(this.Canvas);
                float _x = (float)Math.Floor(p.X / (this.Canvas.ActualWidth / this.gameCreator.x));
                float _y = (float)Math.Floor(p.Y / (this.Canvas.ActualHeight / this.gameCreator.y));

                bool isEmptySpace = true;
                Action work = delegate
                {
                    foreach (var list in new List<LinkedList<Path>> { this.gameCreator.paths, new LinkedList<Path>(this.gameCreator.ground.Cast<Path>()) })
                    {
                        foreach (var item in list)
                        {
                            if (_x == item.x && _y == item.y)
                            {
                                isEmptySpace = false;
                                return;
                            }
                        }
                    }
                };
                work();

                GameObject hint = null;
                foreach (var item in marks)
                {
                    if(item.code.Equals(HINT) && _x == item.x && _y == item.y)
                    {
                        hint = item;
                        break;
                    }
                }

                if (isEditor && this.selectedControl != null && isEmptySpace)
                {
                    if (this.selectedControl.Name.Equals("weg") && this.gameCreator.paths.Count > 0
                        && !(this.gameCreator.paths.Last.Value.GetType() == typeof(Base))
                        && hint != null)
                    {
                        Path path = new Path(hint.x, hint.y, hint.width, hint.height);
                        path.start(this);
                        this.gameCreator.paths.AddLast(path);
                        removeHintMarks();
                        createHintMarks();
                    }
                    else if (this.selectedControl.Name.Equals("spawn")
                        && this.gameCreator.paths.Count == 0)
                    {
                        Spawn spawn = new Spawn(_x, _y, (float)this.Canvas.ActualWidth / this.gameCreator.x,
                            (float)this.Canvas.ActualHeight / this.gameCreator.y);
                        spawn.start(this);
                        this.gameCreator.paths.AddLast(spawn);
                    }
                    else if (this.selectedControl.Name.Equals("ziel") && this.gameCreator.paths.Count > 0
                        && this.gameCreator.paths.Last.Value.GetType() == typeof(Path) && hint != null)
                    {
                        Base basePoint = new Base(hint.x, hint.y, hint.width, hint.height);
                        basePoint.start(this);
                        this.gameCreator.paths.AddLast(basePoint);
                        removeHintMarks();
                    }
                    else if (this.selectedControl.Name.Equals("ground"))
                    {
                        Ground ground = new Ground(_x, _y, (float)this.Canvas.ActualWidth / this.gameCreator.x,
                            (float)this.Canvas.ActualHeight / this.gameCreator.y);
                        ground.start(this);
                        this.gameCreator.ground.AddLast(ground);
                    }
                }
                else if (!isEditor && this.selectedControl != null)
                {
                    // TODO: buy items before we can build it

                    if (this.selectedControl.Name.Equals("ground") && isEmptySpace)
                    {
                        Ground ground = new Ground(_x, _y, (float)this.Canvas.ActualWidth / this.gameCreator.x,
                            (float)this.Canvas.ActualHeight / this.gameCreator.y);
                        ground.start(this);
                        this.gameCreator.ground.AddLast(ground);
                    }
                    else if (hint != null && hint != null)
                    {
                        Ground ground = this.gameCreator.ground.First(g => g.x == hint.x && g.y == hint.y);
                        if(ground != null)
                        {
                            Tower tower = new Tower(ground.x, ground.y, ground.width, ground.height, 2F, 5F, 5);
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
