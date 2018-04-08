using System.Drawing;
using System.Web.Script.Serialization;
using TD_WPF.Game.Enumerations;
using TD_WPF.Properties;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Path : GameObject
    {
        public Path(double x, double y, double width, double height, int index, PathIdentifier pathIdentifier) : base(x,
            y, width, height)
        {
            Index = index;
            PathIdentifier = pathIdentifier;
        }

        public Path()
        {
        }

        public int Index { get; set; }
        public PathIdentifier PathIdentifier { get; set; }

        [ScriptIgnore]
        public override Bitmap Image
        {
            get
            {
                if (PathIdentifier == PathIdentifier.Path)
                    return Resource.path;
                if (PathIdentifier == PathIdentifier.Spawn)
                    return Resource.spawn;
                if (PathIdentifier == PathIdentifier.Base)
                    return Resource.end;
                return Resource.ground;
            }
        }
    }
}