using System.Windows.Media;
using System.Windows.Shapes;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Mark : GameObject
    {
        public Mark(float x, float y, float width, float height, Color color, string code) : base(x, y, width, height)
        {
            this.code = code;
            this.color = color;
            shape = new Rectangle
            {
                Width = this.width,
                Height = this.height,
                Fill = new SolidColorBrush(color)
            };
        }

        public Color color { get; set; }
        public string code { get; set; }

        public override void update(GameControl gameControl, float deltaTime)
        {
            base.update(gameControl, deltaTime);
            shape.Fill = new SolidColorBrush(color);
        }
    }
}