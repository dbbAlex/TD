using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_WPF.Game.Spielobjekte
{
    class MoveableObject : Spielobjekt
    {
        public Enumerations.Navigation currentDirection { get; set; } = Enumerations.Navigation.East;
        public Enumerations.Navigation nextDirection { get; set; } = Enumerations.Navigation.East;
        public int currentAngle { get; set; } = 0;
        public MoveableObject(double width, double height, double x, double y) : base(width, height, x, y)
        {
        }
    }
}
