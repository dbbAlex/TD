using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Spawn : Path
    {
        public Spawn(double x, double y, double width, double height, int index) : base(x, y, width, height, index)
        {
            Image = Resource.spawn;
        }
    }
}