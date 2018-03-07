using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_WPF.Game.Spielobjekte;
using TD_WPF.Game.Spielobjekte.Items;

namespace TD_WPF.Game.Tools
{
    class CollisionDetector
    {
        public static bool collided(Spielobjekt one, Spielobjekt two)
        {
            bool collided = false;

            double oneX = one.x * one.width;
            double oneY = one.y * one.height;

            double twoX = two.x * two.width;
            double twoY = two.x * two.height;

            if(Math.Abs(oneX - oneY) < one.width)
            {
                collided = true;
            }
            else if(Math.Abs(oneY - twoY) < one.height)
            {
                collided = true;
            }

            return collided;
        }
    }
}
