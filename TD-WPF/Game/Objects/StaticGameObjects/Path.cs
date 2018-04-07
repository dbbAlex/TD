using System.ComponentModel;
using TD_WPF.Game.Enumerations;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Path : GameObject
    {
        public Path(double x, double y, double width, double height, int index, PathIdentifier pathIdentifier) : base(x, y, width, height)
        {
            Index = index;
            PathIdentifier = pathIdentifier;
        }
        
        public Path(){}

        public int Index { get; set; }
        public PathIdentifier PathIdentifier { get; set; }

        public override void Start(GameControl gameControl)
        {
            if (Active) return;
            switch (PathIdentifier)
            {
                    case PathIdentifier.Path:
                        Image = Resource.path;
                        break;
                    case PathIdentifier.Spawn:
                        Image = Resource.spawn;
                        break;
                    case PathIdentifier.Base:
                        Image = Resource.end;
                        break;
                    case PathIdentifier.Ground:
                        Image = Resource.ground;
                        break;
            }
            base.Start(gameControl);
        }
    }
}