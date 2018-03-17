using System.Windows.Media;
using System.Windows.Shapes;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Mark : GameObject
    {
        public Mark(float x, float y, float width, float height, Color color, string code) : base(x, y, width, height)
        {
            this.Code = code;
            this.Color = color;
            this.Shape.Fill = new SolidColorBrush(color);
        }

        public Color Color { get; set; }
        public string Code { get; private set; }

        public override void Update(GameControl gameControl)
        {
            base.Update(gameControl);
            this.Shape.Fill = new SolidColorBrush(Color);
        }
    }
}