using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Path : GameObject
    {
        public Path(double x, double y, double width, double height, int index) : base(x, y, width, height)
        {
            Index = index;
            Image = Resource.path;
        }
        
        public Path(){}

        public int Index { get; set; }
    }
}