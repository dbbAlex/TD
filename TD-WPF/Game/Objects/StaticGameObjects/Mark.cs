using System.Windows.Media;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Mark : GameObject
    {
        public Mark(float x, float y, float width, float height, Color color, string code) : base(x, y, width, height)
        {
            Code = code;
            Color = color;
            Shape.Fill = new SolidColorBrush(color);
        }

        public Color Color { get; set; }
        public string Code { get; }

        public override void Update(GameControl gameControl)
        {
            if (!Active) return;
            base.Update(gameControl);
            Shape.Fill = new SolidColorBrush(Color);
        }
    }
}