using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TD_WPF.Game.GameObjects
{
    public abstract class GameObject
    {
        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public Bitmap image { get; set; }
        public Shape shape { get; set; }

        public GameObject(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void render(GameControl gameControl)
        {
            this.shape.Width = width;
            this.shape.Height = height;
            Canvas.SetLeft(this.shape, this.x * width);
            Canvas.SetTop(this.shape, this.y * height);
        }

        public void start(GameControl gameControl)
        {
            gameControl.Canvas.Children.Add(this.shape);
        }



        public virtual void update(GameControl gameControl)
        {
            this.width = (float)gameControl.Canvas.ActualWidth / gameControl.gameCreator.x;
            this.height = (float)gameControl.Canvas.ActualHeight / gameControl.gameCreator.y;
        }
    }
}
