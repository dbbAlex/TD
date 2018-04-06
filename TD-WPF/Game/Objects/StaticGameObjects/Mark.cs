using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Mark : GameObject
    {
        public Mark(double x, double y, double width, double height, Color color, string code) : base(x, y, width,
            height)
        {
            Code = code;
            Color = color;
        }

        public Color Color { private get; set; }
        public string Code { get; }

        public override void Update(GameControl gameControl)
        {
            if (!Active) return;
            base.Update(gameControl);
            Shape.Fill = new SolidColorBrush(Color);
        }

        public override void Start(GameControl gameControl)
        {
            Shape = new Rectangle
            {
                Name = GetType().Name,
                Width = Width,
                Height = Height,
                Fill = new SolidColorBrush(Color)
            };
            Canvas.SetLeft(Shape, X * Width);
            Canvas.SetTop(Shape, Y * Height);
            Active = true;
            gameControl.Canvas.Children.Add(Shape);
        }
    }
}