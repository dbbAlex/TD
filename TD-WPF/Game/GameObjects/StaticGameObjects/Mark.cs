using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TD_WPF.Game.GameObjects.StaticGameObjects
{
    public class Mark : GameObject
    {
        public Color color { get; set; }
        public string code { get; set; }

        public Mark(float x, float y, float width, float height, Color color, string code) : base(x, y, width, height)
        {
            this.code = code;
            this.color = color;
            this.shape = new System.Windows.Shapes.Rectangle()
            {
                Width = this.width,
                Height = this.height,
                Fill = new SolidColorBrush(color)
            };
        }

        public override void update(GameControl gameControl, float deltaTime)
        {
            base.update(gameControl, deltaTime);
            this.shape.Fill = new SolidColorBrush(color);
        }
    }
}
