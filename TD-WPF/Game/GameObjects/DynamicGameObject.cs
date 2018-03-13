using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TD_WPF.Game.GameObjects
{
    public abstract class DynamicGameObject : GameObject
    {
        public float speed { get; set; }
        public DynamicGameObject(float x, float y, float width, float height, float speed) : base(x, y, width, height)
        {
            this.speed = speed;
        }
    }
}
