using System;
using System.Web.Script.Serialization;
using TD_WPF.Game.Enumerations;

namespace TD_WPF.Game.Objects.StaticGameObjects
{
    public class Ground : Path
    {
        public const string Name = "Ground";
        public const int Money = 5;

        public Ground(double x, double y, double width, double height, int index, PathIdentifier pathIdentifier) : base(
            x, y, width, height, index, pathIdentifier)
        {
        }

        public Ground()
        {
        }

        [ScriptIgnore] public Tower Tower { get; set; } = null;

        [ScriptIgnore] public static int UpdateSellMoney => Convert.ToInt32(Math.Ceiling(Money / 2d));

        public void Update(GameControl gameControl, long currentInterval)
        {
            if (!Active) return;
            base.Update(gameControl);
            Tower?.Update(gameControl, currentInterval);
        }

        public override void Render(GameControl gameControl)
        {
            base.Render(gameControl);
            Tower?.Render(gameControl);
        }

        public override void Deaktivate()
        {
            base.Deaktivate();
            Tower?.Deaktivate();
        }

        public override void Destroy(GameControl gameControl)
        {
            base.Destroy(gameControl);
            gameControl.GameCreator.Ground.Remove(this);
        }
    }
}