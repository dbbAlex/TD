using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TD_WPF.Game.GameObjects
{
    public abstract class GameObject
    {
        public GameObject(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public Bitmap image { get; set; }
        public Shape shape { get; set; }

        public virtual void render(GameControl gameControl)
        {
            shape.Width = width;
            shape.Height = height;
            Canvas.SetLeft(shape, x * width);
            Canvas.SetTop(shape, y * height);
        }

        public virtual void start(GameControl gameControl)
        {
            Canvas.SetLeft(shape, x * width);
            Canvas.SetTop(shape, y * height);
            gameControl.Canvas.Children.Add(shape);
        }


        public virtual void update(GameControl gameControl, float deltaTime)
        {
            width = (float) gameControl.Canvas.ActualWidth / gameControl.gameCreator.x;
            height = (float) gameControl.Canvas.ActualHeight / gameControl.gameCreator.y;
        }
    }
}